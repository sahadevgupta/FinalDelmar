using CoreGraphics;
using FormsLoyalty.Controls;
using FormsLoyalty.iOS.Renderers;
using Foundation;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(DropdownView), typeof(ExtendedDropdownRenderer))]
namespace FormsLoyalty.iOS.Renderers
{
    public class ExtendedDropdownRenderer : ViewRenderer<DropdownView, IosDropDownBox>
    {
        private static readonly int baseHeight = 10;
        private object debug;
        private string fontFamily;

        public IosDropDownBox Box { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DropdownRenderer"/>
        /// </summary>
        public ExtendedDropdownRenderer()
        {
            //Frame = new RectangleF(0, 20, 320, 40);
        }
        /// <inheritdoc />
        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            SizeRequest baseResult = base.GetDesiredSize(widthConstraint, heightConstraint);
            Foundation.NSString testString = new Foundation.NSString("Tj");
            CoreGraphics.CGSize testSize = testString.GetSizeUsingAttributes(new UIStringAttributes { Font = Control.Font });
            double height = baseHeight + testSize.Height;
            height = Math.Round(height);
            return new SizeRequest(new Xamarin.Forms.Size(baseResult.Request.Width, height));
        }
        protected override void OnElementChanged(ElementChangedEventArgs<DropdownView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null && Control != null)
            {
                Control.SuggestionChosen -= OnItemSelected;
                Control.ShowSuggestionList -= Control_ShowSuggestionList;
            }
            if (e.NewElement != null && Control == null)
            {
                Box = CreateNativeControl();
                SetNativeControl(Box);

                // DrawNativeControl();

                Control.SetPlaceholderTextColor(Color.LightGray);
                UpdatePlaceholderText();
                UpdateIsEnabled();
                UpdateStyle();
                UpdateSelectedIndex();

                Control.SuggestionChosen += OnItemSelected;
                Control.ShowSuggestionList += Control_ShowSuggestionList;
            }
        }

        private void Control_ShowSuggestionList(object sender, bool args)
        {
           // Element.ShowDropdown = args;
        }

        private void UpdateStyle()
        {
            var data = Element.InputViewStyle.Setters;
            foreach (var item in data)
            {
                if (item.Property == Label.FontFamilyProperty)
                {
                    fontFamily = item.Value.ToString();
                    //autoCompleteTextView.SetTypeface(tf, Android.Graphics.TypefaceStyle.Normal);
                }

                if (item.Property == Label.FontSizeProperty)
                {
                    var value = item.Value as Xamarin.Forms.OnIdiom<double>;
                    double size = Device.Idiom == TargetIdiom.Phone ? value.Phone : value.Tablet;
                    //Control.Font = UIFont.FromName(fontFamily, (nfloat)size);

                    Control.Font = UIFont.PreferredBody;
                }

                if (item.Property == Label.TextColorProperty)
                {
                    var resource = item.Value as Xamarin.Forms.Internals.DynamicResource;
                    var color = (Xamarin.Forms.Color)Xamarin.Forms.Application.Current.Resources[resource.Key];
                    Control.SetTextColor(color);
                }
            }
        }


        private void UpdateSelectedIndex()
        {
            //if (Element.SelectedItem != null)
            //{
            Box.Text = FormatType(Element.SelectedItem, Element.DisplayMemberPath);
            //}
        }
        private void OnItemSelected(object sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            if (Element.SelectedItem != e.SelectedItem)
            {
                Element.SelectedItem = e.SelectedItem;
            }
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Control == null)
            {
                return;
            }
            else if (e.PropertyName == nameof(DropdownView.PlaceholderText))
            {
                UpdatePlaceholderText();
            }
            else if (e.PropertyName == nameof(DropdownView.DisplayMemberPath))
            {
                UpdateDisplayMemberPath();
            }
            else if (e.PropertyName == nameof(DropdownView.IsEnabled))
            {
                UpdateIsEnabled();
            }
            else if (e.PropertyName == nameof(DropdownView.ItemSource))
            {
                UpdateItemsSource();
            }
            else if (e.PropertyName == DropdownView.CanEditProperty.PropertyName)
            {

            }
            else if (e.PropertyName == DropdownView.ShowDropdownProperty.PropertyName)
            {
                Control.ShowHideDropdown(Control.InputTextField);
                Control.IsSuggestionListOpen = Element.ShowDropdown;
            }
            else if (e.PropertyName == DropdownView.SelectedItemProperty.PropertyName)
            {
                UpdateSelectedIndex();
            }

            base.OnElementPropertyChanged(sender, e);
        }
        private void UpdateIsEnabled()
        {
            Control.UserInteractionEnabled = Element.IsEnabled;
        }
        private void UpdateDisplayMemberPath()
        {
            Control.SetItems(Element.ItemSource?.OfType<object>(), (o) => FormatType(o, Element?.DisplayMemberPath));
        }
        private void UpdateItemsSource()
        {
            Control.SetItems(Element.ItemSource?.OfType<object>(), (o) => FormatType(o, Element?.DisplayMemberPath));
            Control.IsSuggestionListOpen = Element.ShowDropdown;
        }
        private static string FormatType(object instance, string memberPath)
        {
            if (!string.IsNullOrEmpty(memberPath))
                return instance?.GetType().GetProperty(memberPath)?.GetValue(instance)?.ToString() ?? "";
            else
                return instance?.ToString() ?? "";
        }
        private void UpdatePlaceholderText() => Control.PlaceholderText = Element.PlaceholderText;
        protected override IosDropDownBox CreateNativeControl()
        {
            //var box = 


            return new IosDropDownBox();
        }
        bool IsInitialized = false;
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            if (!IsInitialized)
            {
                DrawNativeControl();
                IsInitialized = true;
            }

        }

        private void DrawNativeControl()
        {
            var scrollView = GetParentScrollView(Control);

            var viewController = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            while (viewController != null && viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            var ctrl = viewController;

            var relativePosition = UIApplication.SharedApplication.KeyWindow;
            var relativeFrame = Box.Superview.ConvertRectToView(Box.Frame, relativePosition);
            Box.Draw(ctrl, Layer, scrollView, relativeFrame);

            //Control.IsSuggestionListOpen = Element.ShowDropdown;
            UpdateItemsSource();
        }

        private static UIScrollView GetParentScrollView(UIView element)
        {
            if (element.Superview == null) return null;
            // var scrollView = element.Superview.Superview.Superview.Superview.Superview.Superview.Superview as UIScrollView;

            var scrollView = element.Superview as UIScrollView;
            return scrollView ?? null;
        }
    }
}

