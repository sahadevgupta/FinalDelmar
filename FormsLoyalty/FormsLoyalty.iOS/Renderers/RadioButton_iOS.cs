using CoreGraphics;
using FormsLoyalty.Controls;
using FormsLoyalty.iOS.Renderers;
using Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//[assembly:ExportRenderer(typeof(CustomRadioButton),typeof(RadioButton_iOS))]
namespace FormsLoyalty.iOS.Renderers
{
    public class RadioButton_iOS : ViewRenderer<CustomRadioButton, RadioButtonView>
    {
        private UIColor _defaultTextColor;
        public override void DrawRect(CGRect area, UIViewPrintFormatter formatter)
        {
            base.DrawRect(area, formatter);
        }
        protected override void OnElementChanged(ElementChangedEventArgs<CustomRadioButton> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var checkBox = new RadioButtonView(Bounds);
                _defaultTextColor = checkBox.TitleColor(UIControlState.Normal);
                checkBox.TouchUpInside += (s, args) => Element.IsChecked = Control.Checked;

                SetNativeControl(checkBox);
            }
            if (this.Element == null) return;

            BackgroundColor = this.Element.BackgroundColor.ToUIColor();


            Control.LineBreakMode = UILineBreakMode.CharacterWrap;
            Control.VerticalAlignment = UIControlContentVerticalAlignment.Top;
            Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            Control.Text = this.Element.Content.ToString();
            Control.Checked = this.Element.IsChecked;

            // UpdateTextColor();
        }
        private void ResizeText()
        {
            var text = this.Element.Content.ToString();

            var bounds = this.Control.Bounds;

            var width = this.Control.TitleLabel.Bounds.Width;

            var height = text.StringHeight(this.Control.Font, width);

            var minHeight = string.Empty.StringHeight(this.Control.Font, width);

            var requiredLines = Math.Round(height / minHeight, MidpointRounding.AwayFromZero);

            var supportedLines = Math.Round(bounds.Height / minHeight, MidpointRounding.ToEven);

            if (supportedLines == requiredLines)
            {
                return;
            }

            bounds.Height += (float)(minHeight * (requiredLines - supportedLines));
            this.Control.Bounds = bounds;
            this.Element.HeightRequest = bounds.Height;


        }
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            ResizeText();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "Checked":
                    Control.Checked = (bool)Element.IsChecked;
                    break;
                case "Text":
                    Control.Text = Element.Content.ToString();
                    break;
                case "TextColor":
                    UpdateTextColor();
                    break;
                case "Element":
                    break;

            }
        }

        private void UpdateTextColor()
        {
            Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Normal);
            Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Selected);
        }
    }
    public static class StringExtensions
    {
        public static nfloat StringHeight(this string text, UIFont font, nfloat width)
        {

            var nativeString = new NSString(text);

            var rect = nativeString.GetBoundingRect(
                new CGSize(width, float.MaxValue),
                NSStringDrawingOptions.OneShot,
                new UIStringAttributes() { Font = font },
                null);

            return rect.Height;
        }

    }

    [Register("RadioButtonView")]
    public class RadioButtonView : UIButton
    {
        public RadioButtonView()
        {
            Initialize();
        }

        public RadioButtonView(CGRect bounds) : base(bounds)
        {
            Initialize();
        }


        public bool Checked
        {
            set { this.Selected = value; }
            get { return this.Selected; }
        }

        public string Text
        {
            set { this.SetTitle(value, UIControlState.Normal); }
        }

        void Initialize()
        {
            this.Frame = new CGRect(0, 20, 200, 30);
            // this.AdjustEdgeInsets();
            this.ApplyStyle();
            this.TitleEdgeInsets = new UIEdgeInsets(0, 10, 0, 0);
            SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            this.SetTitleColor(UIColor.DarkTextColor, UIControlState.Selected);
            this.TouchUpInside += (sender, args) => this.Selected = !this.Selected;
        }

        void AdjustEdgeInsets()
        {
            const float inset = 8f;

            this.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            //this.ImageEdgeInsets = new UIEdgeInsets(0f, 6f, 0f, 0f);
            this.TitleEdgeInsets = new UIEdgeInsets(2f, 6f, 0f, 0f);
        }

        void ApplyStyle()
        {

            var checkedImage =  UIImage.FromBundle("checked.png");
            checkedImage.ApplyTintColor(Color.FromHex("#b72228").ToUIColor());

            var uncheckedImage = UIImage.FromBundle("unchecked.png");
            checkedImage.ApplyTintColor(Color.FromHex("#b72228").ToUIColor());

            this.SetImage(checkedImage, UIControlState.Selected);
            this.SetImage(uncheckedImage, UIControlState.Normal);
        }
    }
}