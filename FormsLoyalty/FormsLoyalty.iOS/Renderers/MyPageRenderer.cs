using FormsLoyalty.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(Page), typeof(MyPageRenderer))]
namespace FormsLoyalty.iOS.Renderers
{
    internal class MyPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            Page page = Element as Page;
           // page.BackgroundColor = Color.WhiteSmoke;
            //page.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }
    }
}
