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

namespace App12
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SpriteVisual _spriteVisual;
        private CompositionPropertySet _sharedProperties;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += this.MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var compositor = Window.Current.Compositor;

            _sharedProperties = compositor.CreatePropertySet();
            _sharedProperties.InsertScalar("Radius", 200);
            _sharedProperties.InsertScalar("Angle", 0);
            _sharedProperties.InsertScalar("OffsetX", (float)VisualElement.ActualWidth / 2.0f);
            _sharedProperties.InsertScalar("OffsetY", (float)VisualElement.ActualHeight / 2.0f);
            var offsetExpression = compositor.CreateExpressionAnimation(
                "Vector3(" +
                    "sharedProperties.OffsetX + sharedProperties.Radius * Cos(ToRadians(sharedProperties.Angle)) ," +
                    "sharedProperties.OffsetY+ sharedProperties.Radius * Sin(ToRadians(sharedProperties.Angle)), " +
                    "0)");
            offsetExpression.SetReferenceParameter("sharedProperties", _sharedProperties);

            _spriteVisual = compositor.CreateSpriteVisual();
            _spriteVisual.Brush = compositor.CreateColorBrush(Colors.Red);
            _spriteVisual.Size = new Vector2(50, 50);
            _spriteVisual.AnchorPoint = new Vector2(0.5f, 0.5f);
            //_spriteVisual.Offset = new Vector3(25f, (float)VisualElement.ActualHeight / 2.0f, 0);

            ElementCompositionPreview.SetElementChildVisual(VisualElement, _spriteVisual);

            var rotateExpression = compositor.CreateExpressionAnimation("sharedProperties.Angle");
            rotateExpression.SetReferenceParameter("sharedProperties", _sharedProperties);
            _spriteVisual.StartAnimation("Offset", offsetExpression);
            _spriteVisual.StartAnimation(nameof(Visual.RotationAngleInDegrees), rotateExpression);
        }

        private void OffsetSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //_spriteVisual.Offset = new Vector3(25f + (float)e.NewValue, (float)VisualElement.ActualHeight / 2.0f, 0);
            _sharedProperties.InsertScalar("Angle", (float)e.NewValue);
        }
    }
}
