using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App7
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LoadedImageSurface _loadedImageSurface;
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ApplyComposition();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ApplyComposition()
        {
            var compositor = Window.Current.Compositor;

            var backVisual = ElementCompositionPreview.GetElementVisual(FancyGrid);
            //backVisual.Offset = new Vector3(100, 100, 0);

            ElementCompositionPreview.SetIsTranslationEnabled(FancyGrid, true);
            var fancyGridPropertySet = backVisual.Properties;
            fancyGridPropertySet.InsertVector3("Translation", new Vector3(100, 100, 0));

            backVisual.RotationAngleInDegrees = 45;
            backVisual.CenterPoint = new Vector3((float)FancyGrid.ActualWidth / 2.0f, (float)FancyGrid.ActualHeight / 2.0f, 0);

            var sprite1 = compositor.CreateSpriteVisual();
            sprite1.Size = new Vector2(50, 50);
            sprite1.Offset = new Vector3(50, 50, 0);
            sprite1.Brush = compositor.CreateColorBrush(Colors.Green);

            var sprite2 = compositor.CreateSpriteVisual();
            sprite2.Size = new Vector2(50, 50);
            sprite2.Offset = new Vector3(25, 25, 0);
            sprite2.Brush = compositor.CreateColorBrush(Colors.Purple);


            var container = compositor.CreateContainerVisual();

            container.Children.InsertAtTop(sprite1);
            container.Children.InsertAtBottom(sprite2);

            ElementCompositionPreview.SetElementChildVisual(FancyGrid, container);

            _loadedImageSurface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///Assets/JustEnoughBikes.jpg"));
            _loadedImageSurface.LoadCompleted += (sender, args) =>
            {
                if (args.Status == LoadedImageSourceLoadStatus.Success)
                {
                    var brush = compositor.CreateSurfaceBrush(_loadedImageSurface);
                    brush.Stretch = CompositionStretch.UniformToFill;
                    var imageVisual = compositor.CreateSpriteVisual();
                    imageVisual.Size = new Vector2(400, 400);
                    imageVisual.Brush = brush;
                    container.Children.InsertAtTop(imageVisual);
                }

            };

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_loadedImageSurface != null)
            {
                _loadedImageSurface.Dispose();
                _loadedImageSurface = null;
            }
        }
    }
}
