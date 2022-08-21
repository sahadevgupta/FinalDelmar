using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormsLoyalty.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(CollectionView), typeof(NoBounceRenderer))]
namespace FormsLoyalty.iOS.Renderers
{
    public class NoBounceRenderer : CollectionViewRenderer
    {
        public NoBounceRenderer()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<GroupableItemsView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
                Controller.CollectionView.Bounces = false;
        }
    }
}
