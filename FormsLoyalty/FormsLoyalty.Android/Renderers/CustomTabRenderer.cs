﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms.Platform.Android;
using FormsLoyalty.Droid.Renderers;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Tabs;
using FormsLoyalty.Views;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using TabbedPage = Xamarin.Forms.TabbedPage;
using FormsLoyalty.Helpers;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(MainTabbedPage), typeof(CustomTabRenderer))]
namespace FormsLoyalty.Droid.Renderers
{
    public class CustomTabRenderer : TabbedPageRenderer, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private MainTabbedPage _page;
        private const int DelayBeforeTabAdded = 50;
        protected readonly Dictionary<Element, BadgeView> BadgeViews = new Dictionary<Element, BadgeView>();
        private TabLayout _topTabLayout;
        private LinearLayout _topTabStrip;
        private ViewGroup _bottomTabStrip;
        public CustomTabRenderer(Context context):base(context)
        {

        }

       

        public void OnNavigationItemReselected(IMenuItem item)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            // make sure we cleanup old event registrations
            Cleanup(e.OldElement);
            Cleanup(Element);

            var tabCount = InitLayout();
            for (var i = 0; i < tabCount; i++)
            {
                AddTabBadge(i);
            }

            Element.ChildAdded += OnTabAdded;
            Element.ChildRemoved += OnTabRemoved;

            if (e.NewElement != null)
            {
                _page = (MainTabbedPage)e.NewElement;
            }
            else
            {
                _page = (MainTabbedPage)e.OldElement;
            }
         

        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if(e.PropertyName == TabBadge.BadgeTextProperty.PropertyName)
            {

            }
        }

        private void AddTabBadge(int tabIndex)
        {
            var page = Element.GetChildPageWithBadge(tabIndex);

            var placement = Element.OnThisPlatform().GetToolbarPlacement();
            var targetView = placement == ToolbarPlacement.Bottom ? _bottomTabStrip?.GetChildAt(tabIndex) : _topTabLayout?.GetTabAt(tabIndex).CustomView ?? _topTabStrip?.GetChildAt(tabIndex);
            if (!(targetView is ViewGroup targetLayout))
            {
                Console.WriteLine("Plugin.Badge: Badge target cannot be null. Badge not added.");
                return;
            }

            var badgeView = targetLayout.FindChildOfType<BadgeView>();

            if (badgeView == null)
            {
                var imageView = targetLayout.FindChildOfType<ImageView>();
                if (placement == ToolbarPlacement.Bottom)
                {
                    // create for entire tab layout
                    badgeView = BadgeView.ForTargetLayout(Context, imageView);
                }
                else
                {
                    //create badge for tab image or text
                    badgeView = BadgeView.ForTarget(Context, imageView?.Drawable != null
                        ? (Android.Views.View)imageView
                        : targetLayout.FindChildOfType<TextView>());
                }
            }

            BadgeViews[page] = badgeView;
            badgeView.UpdateFromElement(page);

            page.PropertyChanged -= OnTabbedPagePropertyChanged;
            page.PropertyChanged += OnTabbedPagePropertyChanged;
        }
        private int InitLayout()
        {
            switch (this.Element.OnThisPlatform().GetToolbarPlacement())
            {
                case ToolbarPlacement.Default:
                case ToolbarPlacement.Top:
                    _topTabLayout = ViewGroup.FindChildOfType<TabLayout>();
                    if (_topTabLayout == null)
                    {
                        Console.WriteLine("Plugin.Badge: No TabLayout found. Badge not added.");
                        return 0;
                    }

                    _topTabStrip = _topTabLayout.FindChildOfType<LinearLayout>();
                    return _topTabLayout.TabCount;
                case ToolbarPlacement.Bottom:
                    _bottomTabStrip = ViewGroup.FindChildOfType<BottomNavigationView>()?.GetChildAt(0) as ViewGroup;
                    if (_bottomTabStrip == null)
                    {
                        Console.WriteLine("Plugin.Badge: No bottom tab layout found. Badge not added.");
                        return 0;
                    }

                    return _bottomTabStrip.ChildCount;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Cleanup(TabbedPage page)
        {
            if (page == null)
            {
                return;
            }

            foreach (var tab in page.Children.Select(c => c.GetPageWithBadge()))
            {
                tab.PropertyChanged -= OnTabbedPagePropertyChanged;
            }

            page.ChildRemoved -= OnTabRemoved;
            page.ChildAdded -= OnTabAdded;

            BadgeViews.Clear();
            _topTabLayout = null;
            _topTabStrip = null;
            _bottomTabStrip = null;
        }
        protected virtual void OnTabbedPagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is Element element))
                return;

            if (BadgeViews.TryGetValue(element, out var badgeView))
            {
                badgeView.UpdateFromPropertyChangedEvent(element, e);
            }
        }
        private void OnTabRemoved(object sender, ElementEventArgs e)
        {
            e.Element.PropertyChanged -= OnTabbedPagePropertyChanged;
            BadgeViews.Remove(e.Element);
        }

        private async void OnTabAdded(object sender, ElementEventArgs e)
        {
            await Task.Delay(DelayBeforeTabAdded);

            if (!(e.Element is Page page))
                return;

            AddTabBadge(Element.Children.IndexOf(page));
        }

        protected override void Dispose(bool disposing)
        {
            Cleanup(Element);

            base.Dispose(disposing);
        }

        bool BottomNavigationView.IOnNavigationItemSelectedListener.OnNavigationItemSelected(IMenuItem item)
        {
            base.OnNavigationItemSelected(item);

            // item.ItemId is the position
           
                _page.CurrentPage.Navigation.PopToRootAsync();
           
           

            return true;
        }

        
    }
}