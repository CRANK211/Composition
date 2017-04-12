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
using Microsoft.Graphics.Canvas.Effects;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EffectsDemoApp.Effects
{
    public sealed partial class AnimatedEffectControl : BaseEffectControl
    {
        private SpriteVisual _animatedVisual;

        public AnimatedEffectControl()
        {
            this.InitializeComponent();
        }

        protected override void BuildBrush()
        {
            TargetElement.SizeChanged += this.TargetElement_SizeChanged;
            var compositor = Window.Current.Compositor;
            _animatedVisual = compositor.CreateSpriteVisual();
            _animatedVisual.Size = new Vector2(200, 200);
            _animatedVisual.AnchorPoint = new Vector2(0.5f, 0.5f);

            var graphicsEffect = new BlendEffect
            {
                Mode = BlendEffectMode.SoftLight,
                Background = new ColorSourceEffect
                {
                    Color = Colors.Red
                },
                Foreground = new GaussianBlurEffect
                {
                    Name = "BlurEffect",
                    BorderMode = EffectBorderMode.Hard,
                    Source = new CompositionEffectSourceParameter("Background")
                }
            };

            var effectFactory = compositor.CreateEffectFactory(graphicsEffect, new [] {"BlurEffect.BlurAmount"});
            _effectBrush = effectFactory.CreateBrush();
            _effectBrush.Properties.InsertScalar("BlurEffect.BlurAmount", 20);
            _effectBrush.SetSourceParameter("Background", compositor.CreateBackdropBrush());

            _animatedVisual.Brush = _effectBrush;
            PositionVisual();


            var rotateAnimation = compositor.CreateScalarKeyFrameAnimation();
            var easing = compositor.CreateLinearEasingFunction();
            rotateAnimation.InsertKeyFrame(1.0f, 359, easing);
            rotateAnimation.Duration = TimeSpan.FromSeconds(2);
            rotateAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            _animatedVisual.StartAnimation(nameof(Visual.RotationAngleInDegrees), rotateAnimation);

            var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(0f, _animatedVisual.Offset, easing);
            offsetAnimation.InsertKeyFrame(0.25f, _animatedVisual.Offset + new Vector3(300, 0, 0), easing);
            offsetAnimation.InsertKeyFrame(0.75f, _animatedVisual.Offset + new Vector3(-300, 0, 0), easing);
            offsetAnimation.InsertKeyFrame(1f, _animatedVisual.Offset, easing);
            offsetAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            offsetAnimation.Duration = TimeSpan.FromSeconds(2);
            _animatedVisual.StartAnimation(nameof(Visual.Offset), offsetAnimation);

            var blurAnimation = compositor.CreateScalarKeyFrameAnimation();
            blurAnimation.InsertKeyFrame(0f, 0, easing);
            blurAnimation.InsertKeyFrame(0.5f, 20, easing);
            blurAnimation.InsertKeyFrame(1f, 0, easing);
            blurAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            blurAnimation.Duration = TimeSpan.FromSeconds(2);
            _effectBrush.Properties.StartAnimation("BlurEffect.BlurAmount", blurAnimation);


            ElementCompositionPreview.SetElementChildVisual(TargetElement, _animatedVisual);
        }

        private void TargetElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionVisual();
        }

        private void PositionVisual()
        {
            if (_animatedVisual == null)
            {
                return;
            }

            _animatedVisual.Offset = new Vector3((float)TargetElement.ActualWidth / 2.0f, (float)TargetElement.ActualHeight / 2.0f, 0);
        }

        protected override void CustomRemoveEffect()
        {
            if (_animatedVisual != null)
            {
                _animatedVisual.Dispose();
                _animatedVisual = null;
            }

            TargetElement.SizeChanged -= this.TargetElement_SizeChanged;

        }
    }
}
