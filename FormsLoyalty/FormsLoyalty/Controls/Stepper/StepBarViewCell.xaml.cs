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
    public partial class StepBarViewCell : ContentView
    {
        private readonly ViewModelBase vm;
        public StepBarViewCell(ViewModelBase viewmodel)
        {
            vm = viewmodel;
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
            {
                progress.SetBinding(ProgressBar.ProgressProperty, "ProgressValue");
            }
            else
            {
                progress.SetBinding(ProgressBar.ProgressProperty, "ProgressValue", BindingMode.TwoWay);
            }

        }
        

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is StepBarModel selectedmodel)
            {

                if (selectedmodel.Status == StepBarStatus.Completed)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await progress.ProgressTo(1, 800, Easing.Linear);
                        selectedmodel.ProgressValue = 1;
                    });
                }
                else
                {
                    progress.Progress = 0;
                }

                if (!selectedmodel.IsNotLast)
                {
                    mainGrid.ColumnDefinitions.RemoveAt(0);
                }
            }

        }
        private void StepTapped_Tapped(object sender, EventArgs e)
        {
            if (BindingContext is StepBarModel selectedmodel && (selectedmodel.Status == StepBarStatus.Completed || selectedmodel.Status == StepBarStatus.InProgress))
            {
                vm.OnStepTapped(BindingContext as StepBarModel);
            }
        }
    }
}