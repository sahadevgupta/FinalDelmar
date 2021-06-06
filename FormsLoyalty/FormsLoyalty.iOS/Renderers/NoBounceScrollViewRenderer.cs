using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormsLoyalty.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(ScrollView), typeof(NoBounceScrollViewRenderer))]

namespace FormsLoyalty.iOS.Renderers
{
    public class NoBounceScrollViewRenderer : ScrollViewRenderer
    {

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            UpdateScrollView();
        }

        private void UpdateScrollView()
        {
            ContentInset = new UIKit.UIEdgeInsets(0, 0, 0, 0);
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                ContentInsetAdjustmentBehavior = UIKit.UIScrollViewContentInsetAdjustmentBehavior.Never;
            Bounces = false;
            ScrollIndicatorInsets = new UIKit.UIEdgeInsets(0, 0, 0, 0);
        }
    }
}