using FormsLoyalty.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Views.CheckoutStepperView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewSubmitView : ContentView
    {
        public ReviewSubmitView(CheckoutPageViewModel viewmodel)
        {
            InitializeComponent();
            BindingContext  = viewmodel;

        }
    }
}