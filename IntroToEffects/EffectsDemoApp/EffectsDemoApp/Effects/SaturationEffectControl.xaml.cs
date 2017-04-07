using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.Effects;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EffectsDemoApp.Effects
{
    public sealed partial class SaturationEffectControl : BaseEffectControl
    {
        public SaturationEffectControl()
        {
            this.InitializeComponent();
        }

        protected override void BuildBrush()
        {
            var compositor = Window.Current.Compositor;
            _effectVisual = compositor.CreateSpriteVisual();

            var graphicsEffect = new SaturationEffect()
            {
                Name = "SaturationEffect",
                Saturation = (float) SaturationSlider.Value,
                Source = new CompositionEffectSourceParameter("Background")
            };

            var effectFactory = compositor.CreateEffectFactory(graphicsEffect, new [] { "SaturationEffect.Saturation" });
            _effectBrush = effectFactory.CreateBrush();
            _effectBrush.SetSourceParameter("Background", compositor.CreateBackdropBrush());

            _effectVisual.Brush = _effectBrush;
            ResizeVisual();
            ElementCompositionPreview.SetElementChildVisual(TargetElement, _effectVisual);
        }

        private void Slider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _effectBrush?.Properties.InsertScalar("SaturationEffect.Saturation", (float)e.NewValue);
        }
    }
}
