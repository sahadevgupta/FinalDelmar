using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
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
    public partial class VariantPopUp : PopupPage
    {
        public bool IsSaved;
        public VariantRegistration variantRegistration;
        public decimal Qty { get; set; }

        public Action Delete { get; }

        


        public VariantPopUp(decimal Quantity,Action onDelete = null)
        {
            InitializeComponent();
            if (onDelete == null)
            {
                DeleteBtn.IsVisible = false;
            }

            Delete = onDelete;
            Qty = Quantity;
        }

       

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is LoyItem item)
            {
                BindableLayout.SetItemsSource(stack, item.VariantsExt);
                stepper.Text = Convert.ToInt32(Qty);
            }
        }
        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = ((Picker)sender).BindingContext as VariantExt;

            var selected = ((Picker)sender).SelectedItem as LSRetail.Omni.Domain.DataModel.Base.Retail.DimValue;


           var count = picker.Values.Where(x => x.IsSelected);
            if (count.Any())
            {
                count.First().IsSelected = false;
            }
            selected.IsSelected = true;
        }

        private async void okBtn_Clicked(object sender, EventArgs e)
        {
            IsSaved = true;
            Qty = Convert.ToDecimal(stepper.Text);
            if (BindingContext is LoyItem item)
            {
                variantRegistration = VariantRegistration.GetVariantRegistrationFromVariantExts(item.VariantsExt, item.VariantsRegistration);
               
            }
            await ClosePopUp();
            
        }

        private async void CancelBtn_Clicked(object sender, EventArgs e)
        {
            IsSaved = false;

            await ClosePopUp();
        }

        private async Task ClosePopUp()
        {
            await Navigation.PopPopupAsync();
        }

        protected override bool OnBackgroundClicked()
        {
            IsSaved = false;
            return base.OnBackgroundClicked();
            
        }
        protected  override bool OnBackButtonPressed()
        {
            Navigation.PopPopupAsync();
            return base.OnBackButtonPressed();
        }

        private async void DeleteBtn_Clicked(object sender, EventArgs e)
        {
            Delete?.Invoke();
            await ClosePopUp();
        }
    }
}