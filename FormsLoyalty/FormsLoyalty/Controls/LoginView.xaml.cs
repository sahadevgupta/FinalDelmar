using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI;

namespace FormsLoyalty.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentView
    {
        public static readonly BindableProperty MobileNumberProperty =
         BindableProperty.Create(
            propertyName: "MobileNumber",
             returnType: typeof(string),
             declaringType: typeof(LoginView),
             defaultValue: null,
             defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty CommandProperty =
           BindableProperty.Create(
              propertyName: "Command",
              returnType: typeof(ICommand),
             declaringType: typeof(LoginView),
             defaultValue: null
            
             );

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                propertyName: "CommandParameter",
                returnType: typeof(object),
                declaringType: typeof(LoginView),
                defaultValue: null
              );


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }


        public string MobileNumber
        {
            get { return (string)GetValue(MobileNumberProperty); }
            set { SetValue(MobileNumberProperty, value); }
        }
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public LoginView()
        {
            InitializeComponent();
            //numberText.SetBinding(MaterialTextField.TextProperty, new Binding(nameof(MobileNumber), BindingMode.TwoWay, source: this));
        }

        // Helper method for invoking commands safely
        public static void Execute(ICommand command)
        {
            if (command == null) return;
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }


        private void MaterialTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (MaterialTextField)sender;
            if (string.IsNullOrEmpty(view.Text))
            {

                view.HasError = true;
                view.ErrorText = "Mobile Number cannot be empty";
            }
            else
            {

                if (!string.IsNullOrEmpty(view.Text))
                {
                    if (view.HasError)
                    {
                        view.HasError = false;
                        view.ErrorText = string.Empty;
                    }
                }

                if (view.Text.Length > 11)
                {
                    view.Text = e.OldTextValue;
                }

               
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            new Command(() => Execute(Command));
        }
    }
}