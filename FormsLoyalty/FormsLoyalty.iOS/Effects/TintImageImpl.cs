using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormsLoyalty.Effects;
using FormsLoyalty.iOS.Effects;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ResolutionGroupName(TintImage.EffectGroupName)]
[assembly: ExportEffect(typeof(TintImageImpl), TintImage.EffectName)]
namespace FormsLoyalty.iOS.Effects
{
    public class TintImageImpl : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var effect = (TintImage)Element.Effects.FirstOrDefault(e => e is TintImage);

                if (effect == null || !(Control is UIImageView image))
                    return;

                image.Image = image.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                image.TintColor = effect.TintColor.ToUIColor();
            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnDetached() { }
    }
}