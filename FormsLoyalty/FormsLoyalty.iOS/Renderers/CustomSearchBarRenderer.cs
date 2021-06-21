using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SearchBar = Xamarin.Forms.SearchBar;
using FormsLoyalty.iOS.Renderers;

//[assembly: ExportRenderer(typeof(SearchBar), typeof(CustomSearchBarRenderer))]
namespace FormsLoyalty.iOS.Renderers
{
    public class CustomSearchBarRenderer : SearchBarRenderer
    {
        private UISearchBar NativeSearchBar => (UISearchBar)Control;
        private SearchBar XamarinSearchBar => (SearchBar)Element;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.SearchBar> e)
        {
            base.OnElementChanged(e);
            if (Control!=null)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    var color = Element.BackgroundColor;
                    NativeSearchBar.SearchTextField.BackgroundColor = color.ToUIColor();
                }
                    
            }
        }
    }
}