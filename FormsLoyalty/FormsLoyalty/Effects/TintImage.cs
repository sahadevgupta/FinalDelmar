using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Effects
{
    public class TintImage : RoutingEffect
    {
        public const string EffectGroupName = "MyApp";
        public const string EffectName = "TintImage";
        public Color TintColor { get; private set; }
        public TintImage(Color color) : base($"{EffectGroupName}.{EffectName}")
        {
            TintColor = color;
        }

    }
}
