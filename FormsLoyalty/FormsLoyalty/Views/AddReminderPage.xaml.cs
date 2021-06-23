using FormsLoyalty.Models;
using FormsLoyalty.PopUpView;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace FormsLoyalty.Views
{
    public partial class AddReminderPage : ContentPage
    {
        AddReminderPageViewModel _viewModel;
        public AddReminderPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as AddReminderPageViewModel;
        }

        private async void days_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var radioBtn = ((RadioButton)sender);

            if (radioBtn.Content.ToString().Equals(AppResources.txtSpecificdaysweek) && radioBtn.IsChecked)
            {
                await SetSpecificDaysReminder();

            }
            else if (radioBtn.IsChecked)
            {
                if (_viewModel != null)
                {
                    _viewModel.selectedDays = new List<int>();
                    _viewModel.medicineReminder.SpecificDays = string.Empty;
                }

                
                dayslbl.Text = string.Empty;
            }


        }

        private async Task SetSpecificDaysReminder()
        {

            var simpleDialogConfiguration = new MaterialConfirmationDialogConfiguration
            {
                // BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.PRIMARY),
                // TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                ButtonAllCaps = false,
                
                CornerRadius = 8,
                ControlSelectedColor = Color.FromHex("#b72228"),
                ControlUnselectedColor = Color.Black.MultiplyAlpha(0.66),
                TintColor = Color.FromHex("#b72228")
                // ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32)
            };

            var result = await MaterialDialog.Instance.SelectChoicesAsync(title: AppResources.txtPickDays,
                                                                                     choices: App.choices,
                                                                                     selectedIndices: _viewModel.selectedDays,
                                                                                     AppResources.ApplicationOk, AppResources.ApplicationCancel,simpleDialogConfiguration);
            if (result != null && result.Count() < 7)
            {
                var sortedList = result.OrderBy(x => x).ToList();
                _viewModel.selectedDays = sortedList;


                for (int i = 0; i < sortedList.Count(); i++)
                {
                    if (i == 0)
                    {
                        dayslbl.Text = App.choices[sortedList[i]];
                    }
                    else
                        dayslbl.Text += $",{App.choices[sortedList[i]]}";
                }



                _viewModel.medicineReminder.SpecificDays = string.Join(",", result);
            }
            else
            {
                if (string.IsNullOrEmpty(_viewModel.medicineReminder.SpecificDays))
                {
                    everydaybtn.IsChecked = true;
                }
            }
                
        }

        private void duration_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var radioBtn = ((RadioButton)sender);
            if (radioBtn.Content.ToString().Equals(AppResources.txtNumbersofdays) && radioBtn.IsChecked)
            {
                InvokePopUp(AppResources.txtSetNumberOfDays,"duration");

            }
            else if (radioBtn.IsChecked)
            {
                if (_viewModel != null)
                {
                    _viewModel.medicineReminder.NoOfDays = 0;
                }


            }

        }

        private async void InvokePopUp(string title,string  fromPage,object bindingContext =null)
        {
            FrequencyTime freqTime = new FrequencyTime();
            if (fromPage.ToLower().Contains("dose".ToLower()))
            {
                freqTime = bindingContext as FrequencyTime;
            }

            var popup = new DosePopUp(fromPage,_viewModel.medicineReminder.NoOfDays,freqTime.Qty);
            popup.Text = title;
            popup.SetTapped += (s, e) =>
            {
                if (fromPage.ToLower().Contains("dose".ToLower()))
                {
                    freqTime.Qty = Convert.ToDecimal(e);
                }
                else
                    _viewModel.medicineReminder.NoOfDays = Convert.ToInt32(e);
            };

            popup.cancelTapped += (s, e) =>
            {
                if (fromPage.ToLower().Contains("duration".ToLower()))
                {
                    if(_viewModel.medicineReminder.NoOfDays == 0)
                      ongoingSwitch.IsChecked = true;
                }
            };

            await Navigation.PushPopupAsync(popup);
        }

        private async void ScheduleTime_Tapped(object sender, System.EventArgs e)
        {
            var stack = (StackLayout)sender;
            stack.Opacity = 0;

            await stack.FadeTo(1, 250);
            InvokePopUp(AppResources.txtHowMuchTake,"dose",stack.BindingContext);
            stack.Opacity = 1;
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            _viewModel.AddMedicineScheduleAndReminder();
        }

        private async void specificdays_Tapped(object sender, EventArgs e)
        {
            var view = (Label)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);

            await SetSpecificDaysReminder();
            view.Opacity = 1;
        }

        private async void noOfDays_Tapped(object sender, EventArgs e)
        {
            var view = (Label)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);
            InvokePopUp(AppResources.txtSetNumberOfDays, "duration");
            view.Opacity = 1;
        }
    }
    
}
