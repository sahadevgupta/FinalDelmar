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

[assembly: ExportRenderer(typeof(OtpEntry), typeof(OtpRenderer))]
namespace FormsLoyalty.iOS.Renderers
{
    public class UIBackwardsTextField : UITextField
    {
        // A delegate type for hooking up change notifications.
        public delegate void DeleteBackwardEventHandler(object sender, EventArgs e);

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event DeleteBackwardEventHandler OnDeleteBackward;

        public void OnDeleteBackwardPressed()
        {
            if (OnDeleteBackward != null)
            {
                OnDeleteBackward(null, null);
            }
        }

        public UIBackwardsTextField()
        {
            BorderStyle = UITextBorderStyle.None;
            ClipsToBounds = true;
        }

        public override void DeleteBackward()
        {
            base.DeleteBackward();
            OnDeleteBackwardPressed();
        }
    }

    public class OtpRenderer : EntryRenderer, IUITextFieldDelegate
    {
        private IElementController ElementController => Element as IElementController;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            if (Element == null)
            {
                return;
            }
            if (Control != null)
            {
                Control.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 0);
                Control.BorderStyle = UITextBorderStyle.None;
            }
            UIBackwardsTextField textField = new UIBackwardsTextField();
            textField.EditingChanged += OnEditingChanged;
            textField.OnDeleteBackward += (sender, a) =>
            {
                if (string.IsNullOrWhiteSpace(Control.Text))
                {
                    OtpEntry newentry = (OtpEntry)Element;
                    newentry.OnBackspacePressed();
                }
            };
            SetNativeControl(textField);
            base.OnElementChanged(e);
        }

        private void OnEditingChanged(object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);
        }
    }
}