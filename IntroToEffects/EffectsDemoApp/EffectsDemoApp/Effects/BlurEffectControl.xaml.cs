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
    public sealed partial class BlurEffectControl : BaseEffectControl
    {
        public BlurEffectControl()
        {
            this.InitializeComponent();
        }

        protected override void BuildBrush()
        {
            var compositor = Window.Current.Compositor;
            _effectVisual = compositor.CreateSpriteVisual();

            var graphicsEffect = new GaussianBlurEffect
            {
                Name = "BlurEffect",
                BlurAmount = (float) BlurSlider.Value,
                BorderMode = EffectBorderMode.Hard,
                Source = new CompositionEffectSourceParameter("Background")
            };

            var effectFactory = compositor.CreateEffectFactory(graphicsEffect, new [] {"BlurEffect.BlurAmount"});
            _effectBrush = effectFactory.CreateBrush();
            _effectBrush.SetSourceParameter("Background", compositor.CreateBackdropBrush());

            _effectVisual.Brush = _effectBrush;
            ResizeVisual();
            ElementCompositionPreview.SetElementChildVisual(TargetElement, _effectVisual);
        }

        private void BlurSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _effectBrush?.Properties.InsertScalar("BlurEffect.BlurAmount", (float)e.NewValue);
        }
    }
}
