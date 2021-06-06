using FormsLoyalty.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.Controls.Stepper
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StepBarComponent : ContentView
    {
        public StepBarComponent(ViewModelBase vm)
        {
            InitializeComponent();
            BindingContext = vm;
            collectionView.ItemTemplate = new DataTemplate(() => new StepBarViewCell(vm));
        }
    }
}