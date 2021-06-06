using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FormsLoyalty.Controls;
using FormsLoyalty.Droid.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OtpEntry), typeof(OtpRenderer
    ))]
namespace FormsLoyalty.Droid.Renderers
{
    class OtpRenderer : EntryRenderer
    {
        public OtpRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                return;
            }
            Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.Action == KeyEventActions.Down)
            {
                if (e.KeyCode == Keycode.Del)
                {
                    if (string.IsNullOrWhiteSpace(Control.Text))
                    {
                        OtpEntry entry = (OtpEntry)Element;
                        entry.OnBackspacePressed();
                    }
                }
            }

            return base.DispatchKeyEvent(e);
        }
    }
}