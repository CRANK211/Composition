using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EffectsDemoApp.Effects;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EffectsDemoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaCapture _mediaCapture;
        bool _isPreviewing;
        DisplayRequest _displayRequest = new DisplayRequest();
        private SpriteVisual _effectVisual;

        public EffectsSource EffectsSource { get; set; } = new EffectsSource();

        public MainPage()
        {
            this.InitializeComponent();

            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += this.Current_Resuming;
            Loaded += OnLoaded;
            EffectUiHostBackground.SizeChanged += (sender, args) => ResizeVisual();
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            BuildBackground();
            await StartPreviewAsync();
        }

        private void BuildBackground()
        {
            var compositor = Window.Current.Compositor;
            _effectVisual = compositor.CreateSpriteVisual();

            //var graphicsEffect = new GaussianBlurEffect
            //{
            //    BlurAmount = 34,
            //    BorderMode = EffectBorderMode.Hard,
            //    Source = new CompositionEffectSourceParameter("Background")
            //};

            var graphicsEffect = new BlendEffect
            {
                Mode = BlendEffectMode.SoftLight,
                Background = new ColorSourceEffect
                {
                    Color = Colors.LightCyan
                },

                Foreground = new GaussianBlurEffect
                {
                    BlurAmount = 34,
                    BorderMode = EffectBorderMode.Hard,
                    Source = new CompositionEffectSourceParameter("Background")
                }
            };

            var effectFactory = compositor.CreateEffectFactory(graphicsEffect);
            var effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("Background", compositor.CreateBackdropBrush());

            _effectVisual.Brush = effectBrush;

            // set the size!

            ResizeVisual();

            ElementCompositionPreview.SetElementChildVisual(EffectUiHostBackground, _effectVisual);
        }

        private void ResizeVisual()
        {
            if (_effectVisual == null)
                return;

            _effectVisual.Size = new Vector2((float)EffectUiHostBackground.ActualWidth, 
                (float)EffectUiHostBackground.ActualHeight);
        }

        private async void Current_Resuming(object sender, object e)
        {
            await StartPreviewAsync();
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }

        private async Task StartPreviewAsync()
        {
            try
            {

                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync();

                PreviewControl.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();
                _isPreviewing = true;

                _displayRequest.RequestActive();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                System.Diagnostics.Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed. {0}", ex.Message);
            }
        }

        private async Task CleanupCameraAsync()
        {
            if (_mediaCapture != null)
            {
                if (_isPreviewing)
                {
                    await _mediaCapture.StopPreviewAsync();
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PreviewControl.Source = null;
                    if (_displayRequest != null)
                    {
                        _displayRequest.RequestRelease();
                    }

                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                });
            }

        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var effectUiWrapper = e.AddedItems[0] as EffectUiWrapper;
            CleanupEffectUiHost();

            var effectui = effectUiWrapper.Create();
            EffectUiHost.Children.Add(effectui as UserControl);
            effectui.TargetElement = PreviewControl;
            effectui.ApplyEffect();
        }

        private void CleanupEffectUiHost()
        {
            if (EffectUiHost.Children.Any())
            {
                foreach (IEffectUI child in EffectUiHost.Children)
                {
                    child.RemoveEffect();
                }

                EffectUiHost.Children.Clear();
            }
        }
    }
}
