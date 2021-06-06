using Rg.Plugins.Popup.Extensions;
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
    public partial class DosePopUp : PopupPage
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
       nameof(Text),
       typeof(string),
       typeof(DosePopUp),
       null,
       BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set { SetValue(TextProperty, value); }
        }

        public string FromMethod { get; }

        public event EventHandler<string> SetTapped;
        public event EventHandler cancelTapped;

        public DosePopUp(string From,int days,decimal doseQty)
        {
            InitializeComponent();
            FromMethod = From;
            if (From.ToLower().Contains("dose".ToLower()))
            {
                var qty = Convert.ToString(doseQty);
                doseQtyEntry.Text = qty;
                _stepperDose.Increment = 0.25;
                doseStack.IsVisible = true;
                QtyStack.IsVisible = false;
            }
            else
            {
                var str = Convert.ToString(days);
                qtyEntry.Text = days > 0 ? str : "30";
                _stepper.Increment = 1;

                QtyStack.IsVisible = true;
                doseStack.IsVisible = false;
            }
            titlelbl.SetBinding(Label.TextProperty, new Binding(nameof(this.Text), BindingMode.TwoWay, source: this));

        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            cancelTapped?.Invoke(this, e);
            ClosePopUp();
        }

        private void okBtn_Clicked(object sender, EventArgs e)
        {
            if (FromMethod.ToLower().Contains("dose".ToLower()))
            {
                SetTapped?.Invoke(this, doseQtyEntry.Text);
            }
            else
                SetTapped?.Invoke(this, qtyEntry.Text);

            ClosePopUp();
        }
        public void ClosePopUp()
        {
            Navigation.PopPopupAsync(true);
        }

        private void _stepperDose_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var value = Convert.ToDouble(doseQtyEntry.Text);

            var qty = value + e.NewValue;

            doseQtyEntry.Text = qty.ToString();
        }
    }
}