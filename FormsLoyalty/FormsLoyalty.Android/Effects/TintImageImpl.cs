using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FormsLoyalty.Droid.Effects;
using FormsLoyalty.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName(TintImage.EffectGroupName)]
[assembly: ExportEffect(typeof(TintImageImpl), TintImage.EffectName)]
namespace FormsLoyalty.Droid.Effects
{
    public class TintImageImpl : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var effect = (TintImage)Element.Effects.FirstOrDefault(e => e is TintImage);

                if (effect == null || !(Control is ImageView image) )
                    return;

                var filter = new PorterDuffColorFilter(effect.TintColor.ToAndroid(), PorterDuff.Mode.SrcIn);
                image.SetColorFilter(filter);
                
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnDetached() { }
    }
}