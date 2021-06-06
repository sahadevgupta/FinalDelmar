using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Helpers
{
    public static class MobileAppImageResourceHelper
    {
        public static ImageSource GetImageSource(string resource)
        {
            if (resource == null)
            {
                return null;
            }

            var imageSource = ImageSource.FromResource($"FormsLoyalty.Resources.{resource}");

            return imageSource;
        }
    }

    [ContentProperty(nameof(Resource))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Resource { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var imageSource = MobileAppImageResourceHelper.GetImageSource(this.Resource);
            return imageSource;
        }
    }

}
