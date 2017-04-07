using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EffectsDemoApp.Effects
{
    public abstract class BaseEffectControl : UserControl, IEffectUI
    {
        protected CompositionEffectBrush _effectBrush;
        protected SpriteVisual _effectVisual;

        private FrameworkElement _targetElement;

        public FrameworkElement TargetElement
        {
            get { return _targetElement; }
            set
            {
                _targetElement = value;
                if (_targetElement != null)
                {
                    _targetElement.SizeChanged += TargetElementOnSizeChanged;
                }
            }
        }

        private void TargetElementOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            ResizeVisual();
        }

        protected void ResizeVisual()
        {
            if (_effectVisual == null)
                return;

            _effectVisual.Size = new Vector2((float)_targetElement.ActualWidth, (float)_targetElement.ActualHeight);
        }

        public void ApplyEffect()
        {
            BuildBrush();
        }

        protected abstract void BuildBrush();

        public void RemoveEffect()
        {
            _targetElement.SizeChanged -= TargetElementOnSizeChanged;

            if (_effectVisual != null)
            {
                _effectVisual.Dispose();
                _effectBrush = null;
            }

            if (_effectBrush != null)
            {
                _effectBrush.Dispose();
                _effectBrush = null;
            }

            CustomRemoveEffect();
        }

        protected virtual void CustomRemoveEffect()
        { }
    }
}
