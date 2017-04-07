using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace EffectsDemoApp.Effects
{
    public interface IEffectUI
    {
        FrameworkElement TargetElement { get; set; }

        void ApplyEffect();

        void RemoveEffect();
    }
}
