using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectsDemoApp.Effects
{
    public class EffectsSource : ObservableCollection<EffectUiWrapper>
    {
        public EffectsSource()
        {
            Add(new EffectUiWrapper { EffectName = "Blur Effect", UserControlType = typeof(BlurEffectControl) });
            Add(new EffectUiWrapper { EffectName = "Saturation Effect", UserControlType = typeof(SaturationEffectControl) });
        }
    }
}
