using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.PopUpView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SnackBarView : PopupPage
    {
        public static BindableProperty MessagePropertry = BindableProperty.Create(nameof(Message), typeof(string), typeof(SnackBarView), null, BindingMode.TwoWay);

        public string Message
        {
            get => (string)GetValue(MessagePropertry);
            set => SetValue(MessagePropertry, value);
        }


        public SnackBarView()
        {
            InitializeComponent();
            messagelbl.SetBinding(Label.TextProperty, new Binding(nameof(Message), BindingMode.TwoWay, source: this));
        }
    }
}