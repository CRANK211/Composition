using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectsDemoApp.Effects
{
    public class EffectUiWrapper
    {
        public string EffectName { get; set; }

        public Type UserControlType { get; set; }

        public IEffectUI Create()
        {
            return Activator.CreateInstance(UserControlType) as IEffectUI;
        }
    }
}
