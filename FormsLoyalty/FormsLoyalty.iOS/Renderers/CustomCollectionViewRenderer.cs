using FormsLoyalty.Controls;
using FormsLoyalty.iOS.Renderers;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CollectionView), typeof(CustomCollectionViewRenderer))]
[assembly: ExportRenderer(typeof(CustomCollectionView), typeof(CustomCollectionViewRenderer))]
namespace FormsLoyalty.iOS.Renderers
{
    public class CustomCollectionViewRenderer : CollectionViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<GroupableItemsView> e)
        {
            base.OnElementChanged(e);
            if (Control !=null &&  Control.PreferredFocusEnvironments[0] is UICollectionView collectionView)
            {
                CollectionView cv = (CollectionView)e.NewElement;

                //UICollectionView control = (UICollectionView)Control.PreferredFocusEnvironments[0];

                if (collectionView.EffectiveUserInterfaceLayoutDirection == UIUserInterfaceLayoutDirection.RightToLeft)
                {

                    #region Header
                    if (cv.Header != null && cv.Header is VisualElement)
                    {
                        var headerView = (VisualElement)cv.Header;
                        headerView.ScaleX = -1;
                        headerView.ScaleY = 1;
                    }
                    else if (cv.Header is string)
                    {
                        cv.Header = new Label()
                        {
                            Text = (string)cv.EmptyView,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            ScaleX = -1,
                            ScaleY = 1
                        };
                    }
                    #endregion

                    #region EmptyView
                    if (cv.EmptyView != null && cv.EmptyView is VisualElement)
                    {
                        var emptyView = (VisualElement)cv.EmptyView;
                        emptyView.ScaleX = -1;
                        emptyView.ScaleY = 1;
                    }
                    else if (cv.EmptyView is string)
                    {
                        cv.EmptyView = new Label()
                        {
                            Text = (string)cv.EmptyView,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            ScaleX = -1,
                            ScaleY = 1
                        };
                    }

                    #endregion

                    #region Footer

                    if (cv.Footer != null && cv.Footer is VisualElement)
                    {
                        var footerView = (VisualElement)cv.Footer;
                        footerView.ScaleX = -1;
                        footerView.ScaleY = 1;
                    }
                    else if (cv.Footer is string)
                    {
                        cv.Footer = new Label()
                        {
                            Text = (string)cv.EmptyView,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            ScaleX = -1,
                            ScaleY = 1
                        };
                    }
                    #endregion


                }

            }
        }
    }
}