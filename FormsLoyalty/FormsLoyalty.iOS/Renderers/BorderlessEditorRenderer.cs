﻿using FormsLoyalty.iOS.Renderers;
using Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(Editor), typeof(BorderlessEditorRenderer))]

namespace FormsLoyalty.iOS.Renderers
{
    public class BorderlessEditorRenderer : EditorRenderer
    {
        public static void Init() { }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Control.Layer.BorderWidth = 0;
        }
    }
}