using FormsLoyalty.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Picker = Xamarin.Forms.Picker;

[assembly: ExportRenderer(typeof(Picker), typeof(CustomPickerRenderer))]
namespace FormsLoyalty.iOS.Renderers
{
    public class CustomPickerRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			var element = (Picker)this.Element;

			if (this.Control != null && this.Element != null )
			{
				var downarrow = UIImage.FromBundle("arrow_down");
				Control.RightViewMode = UITextFieldViewMode.Always;
				var imageView = new UIImageView(downarrow);
				imageView.Frame = new CoreGraphics.CGRect(0, 0, 5, 20);
				Control.RightView = imageView;
				Control.BorderStyle = UITextBorderStyle.None;



			}
		}
	}
}