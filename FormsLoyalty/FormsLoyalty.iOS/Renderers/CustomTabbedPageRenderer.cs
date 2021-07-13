using CoreAnimation;
using CoreGraphics;
using FormsLoyalty.Controls;
using FormsLoyalty.iOS.Renderers;
using Foundation;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
//[assembly: ExportRenderer(typeof(CustomTabbedPage), typeof(CustomTabbedPageRenderer))]
namespace FormsLoyalty.iOS.Renderers
{
    public class CustomTabbedPageRenderer:TabbedRenderer
    {
        private bool disposed;
        private const int tabBarheight = 50;
        private const float animationDuration = 0.1f;
        private  readonly string animationType = "easeIn";

        TabBarIndicatorView IndicatorView;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (IndicatorView is object) return;

            var tabBarController = (UITabBarController) this.ViewController;
            IndicatorView = new TabBarIndicatorView(tabBarController, 4, 4, .4);

            tabBarController.TabBar.AddSubview(IndicatorView);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e?.OldElement == null)
            {
                this.Tabbed.PropertyChanged += Tabbed_PropertyChanged;
            }

            if (e?.NewElement is object)
            {
                var tabBarController = (UITabBarController)this.ViewController;
                if (null!=tabBarController)
                {
                    AddTransitionBetweenMenu();
                }
            }
        }

        private void AddTransitionBetweenMenu()
        {
            ShouldSelectViewController = (tabController,controller) =>
            {
                if (SelectedViewController is null || controller.Equals(SelectedViewController)) return true;

                UIView fromView = SelectedViewController.View;
                UIView toView = controller.View;

                UIView.Transition(fromView, toView, .3, UIViewAnimationOptions.TransitionCrossDissolve, null);

                return true;
            };
        }

        private void Tabbed_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CustomTabbedPage.IsHiddenProperty.PropertyName)
            {
                this.OnTabBarHidden((this.Element as CustomTabbedPage).IsHidden);
            }
            else if(e.PropertyName == "CurrentPage")
            {
                var element = this.Element as CustomTabbedPage;
                if (element.PushTransitionEnabled)
                {
                    IndicatorView.SelectedIndex((int)this.SelectedIndex);
                    AddPushTransition();

                    element.PushTransitionEnabled = false;
                }
                else
                    IndicatorView.SelectedIndex((int)this.SelectedIndex);
            }
        }

        private void AddPushTransition()
        {
            var transition = new CATransition
            {
                 Duration = 0.25,
                 Type = CAAnimation.TransitionPush,
                 TimingFunction = CAMediaTimingFunction.FromName(new NSString(animationType)),
                 Subtype = CAAnimation.TransitionFromRight
            };
            Platform.GetRenderer(Tabbed).NativeView.Layer.AddAnimation(transition, null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.disposed = true;
        }

        private void OnTabBarHidden(bool isHidden)
        {
            if (this.disposed || this.Element == null || this.TabBar == null) return;

            Device.BeginInvokeOnMainThread(async() =>
            {
                await this.SetTabBarVisibilityAsync(isHidden);
            });
        }

        private async Task SetTabBarVisibilityAsync(bool hide)
        {
            this.TabBar.Opaque = false;
            if (hide)
            {
                this.TabBar.Alpha = default;
            }

            this.UpdateFrame(hide);

            //Show/Hide Tabbar
            this.TabBar.Hidden = hide;
            this.RestoreFonts();

            //Animate Appearing
            if (!hide)
            {
                await UIView.AnimateAsync(animationDuration, () => this.TabBar.Alpha = 1);
            }
            this.TabBar.Opaque = true;

            this.ResizeViewController();
            this.RestoreFonts();
        }

        private void ResizeViewController()
        {
            foreach (var child in this.ChildViewControllers)
            {
                child.View.SetNeedsLayout();
                child.View.SetNeedsDisplay();
            }
        }

        private void RestoreFonts()
        {
            //To restore custom fonts
            foreach (var item in this.TabBar.Items)
            {
                var text = item.Title;
                item.Title = string.Empty;
                item.Title = text;
            }
        }

        private void UpdateFrame(bool hide)
        {
            var tabFrame = this.TabBar.Frame;
            tabFrame.Height = hide ? default : tabBarheight;
            this.TabBar.Frame = tabFrame;

            if (UIDevice.CurrentDevice.CheckSystemVersion(13,default))
            {
                var hiddenView = new CGAffineTransform(default, default, default, default, default, tabFrame.Height);
                TabBar.Transform = hide ? hiddenView : new CGAffineTransform(1,default,default,1,default,default);
            }
        }
    }

    public class TabBarIndicatorView : UIView
    {
        readonly UITabBarController tabBarController;
        readonly double animationDuration;
        readonly double spacing;
        const int zero = default;
        public TabBarIndicatorView(UITabBarController tabBarController,double itemHeight,double spacing,double animationDuration)
        {

            UIView tabView = tabBarController?.TabBar.Items?[zero].ValueForKey(NSString.FromData("view", NSStringEncoding.UTF8)) as UIView;
            if (tabView is null)
            {
                return;
            }

            this.tabBarController = tabBarController;
            this.animationDuration = animationDuration;
            this.spacing = spacing;

            this.Frame = new CoreGraphics.CGRect(-1, zero, tabView.Frame.Size.Width * 3 - 10, itemHeight);

            SetGradient();
        }

        public void SelectedIndex(int index)
        {
            UIView.Animate(animationDuration, () =>
            {
                var tabView = tabBarController.TabBar.Items?[index].ValueForKey(NSString.FromData("view", NSStringEncoding.UTF8)) as UIView;

                if (tabView is null)
                {
                    return;
                }

                this.Frame = new CoreGraphics.CGRect(x: tabView.Frame.X + spacing / 2 - 4, y: zero, width: tabView.Frame.Size.Width, height: this.Frame.Size.Height);

            });
        }

        private void SetGradient()
        {
            var gradientLayer = new CAGradientLayer();
            var startColor = Color.FromHex( "#92C023");
            var endColor = Color.FromHex("#008FBE");
            gradientLayer.Colors = new[] { startColor.ToUIColor().CGColor, endColor.ToUIColor().CGColor };
            gradientLayer.Locations = new NSNumber[] { zero, 1 };
            gradientLayer.Frame = this.Frame;

            this.BackgroundColor = UIColor.Clear;
            this.Layer.AddSublayer(gradientLayer);
        }
    }
}