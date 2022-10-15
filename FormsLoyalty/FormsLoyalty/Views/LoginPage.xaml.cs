using Xamarin.Forms;
using XF.Material.Forms.UI;

namespace FormsLoyalty.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
           
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            loginView.BindingContext = BindingContext;
        }
       
        private void MaterialTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (MaterialTextField)sender;
            if (string.IsNullOrEmpty(view.Text))
            {
               
                    view.HasError = true;
                
            }
            if (!string.IsNullOrEmpty(view.Text) && view.HasError)
            {
                    view.HasError = false;
            }

            if (view.Text.Length>11)
            {
                view.Text = e.OldTextValue;
            }

            if (view.Text.Length >= 3 && view.Text.Length <= 11 )
            {
                var _digit = view.Text.Substring(0, 3);
                if (!(_digit.Equals("011") || _digit.Equals("012") || _digit.Equals("015") || _digit.Equals("010")))
                {
                    view.HasError = true;
                    view.ErrorText = "Mobile Number not in correct format.First 3 digit should be 010,011,015 or 012";

                }
                else
                    view.HasError = false;
            }
        }
    }
}
