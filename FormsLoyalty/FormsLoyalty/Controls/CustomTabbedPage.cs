using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Controls
{
    public class CustomTabbedPage : TabbedPage
    {
        public static readonly BindableProperty IsHiddenProperty =
            BindableProperty.Create(nameof(IsHidden), typeof(bool), typeof(CustomTabbedPage), false);

        public bool IsHidden 
        {
            get { return (bool)GetValue(IsHiddenProperty); }
            set { SetValue(IsHiddenProperty, value); }
        }

        public static readonly BindableProperty PushTransitionEnabledProperty =
            BindableProperty.Create(nameof(PushTransitionEnabled), typeof(bool), typeof(CustomTabbedPage), false);

        public bool PushTransitionEnabled
        {
            get { return (bool)GetValue(PushTransitionEnabledProperty); }
            set { SetValue(PushTransitionEnabledProperty, value); }
        }

        public static readonly BindableProperty CurrentPageParameterProperty =
            BindableProperty.Create(nameof(CurrentPageParameter), typeof(bool), typeof(CustomTabbedPage), default);

        public bool CurrentPageParameter
        {
            get { return (bool)GetValue(CurrentPageParameterProperty); }
            set { SetValue(CurrentPageParameterProperty, value); }
        }

    }
}
