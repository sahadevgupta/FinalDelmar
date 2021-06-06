using System;
using Xamarin.Forms;

namespace FormsLoyalty.Views
{
    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
           await Navigation.PopAsync();
        }
    }
}
