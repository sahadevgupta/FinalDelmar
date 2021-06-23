using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Repos;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class AddReminderPageViewModel : ViewModelBase
    {
        private MedicineReminder _medicineReminder;
        public MedicineReminder medicineReminder
        {
            get { return _medicineReminder; }
            set { SetProperty(ref _medicineReminder, value); }
        }

        private ObservableCollection<FrequencyTime> _frequencies;
        public ObservableCollection<FrequencyTime> frequencies
        {
            get { return _frequencies; }
            set { SetProperty(ref _frequencies, value); }
        }

        #region Medicine Unit

        private ObservableCollection<string> _units;
        public ObservableCollection<string> Units
        {
            get { return _units; }
            set { SetProperty(ref _units, value); }
        }

        private string _selectedUnit;
        public string SelectedUnit
        {
            get { return _selectedUnit; }
            set { SetProperty(ref _selectedUnit, value); }
        }
        #endregion


        private int _selectedFreqIndex;
        public int SelectedFreqIndex
        {
            get { return _selectedFreqIndex; }
            set 
            { 
                SetProperty(ref _selectedFreqIndex, value);
                 AddFrequencytime(value);
               
            }
        }

        private List<string> _doseFrequencies;
        public List<string> DoseFrequencies
        {
            get { return _doseFrequencies; }
            set { SetProperty(ref _doseFrequencies, value); }
        }

        public List<int> selectedDays = new List<int>();

        IReminderRepo _reminderRepo;

        public AddReminderPageViewModel(INavigationService navigationService,IReminderRepo reminderRepo) : base(navigationService)
        {
            _reminderRepo = reminderRepo;
            Units = new ObservableCollection<string> 
            { 
                AppResources.txtAmpules,AppResources.txtBigspoons,AppResources.txtCapsules,AppResources.txtDrops,
                AppResources.txtGrams,AppResources.txtIU,
                AppResources.txtMcg,AppResources.txtMg,AppResources.txtmL,
                AppResources.txtPieces,AppResources.txtPills

            };

            

            medicineReminder = new MedicineReminder { Frequency = 1, From = DateTime.UtcNow.Date, AllDay = true, IsReminder = true };
        }
        /// <summary>   
        /// Add the time, to receive reminder
        /// </summary>   
        private void AddFrequencytime(int value)
        {
            if (value == 0)
            {
                frequencies = new ObservableCollection<FrequencyTime> { new FrequencyTime { ID = Guid.NewGuid().ToString(), Time = new TimeSpan(8, 00, 0) } };
            }
            else if (value == 1)
            {

                frequencies = new ObservableCollection<FrequencyTime> { new FrequencyTime { ID = Guid.NewGuid().ToString(), Time = new TimeSpan(8, 00, 0) },
                                                                        new FrequencyTime { ID = Guid.NewGuid().ToString(), Time = default } };
            }
            else
            {
                frequencies = new ObservableCollection<FrequencyTime> { new FrequencyTime { ID = Guid.NewGuid().ToString(), Time = new TimeSpan(8, 00, 0) },
                                                                        new FrequencyTime { ID = Guid.NewGuid().ToString(), Time = default },
                                                                        new FrequencyTime { ID = Guid.NewGuid().ToString(), Time = new TimeSpan(23, 00, 0) }};
            }

        }

        /// <summary>   
        /// Represents to add reminder to get local notification.   
        /// </summary>   
        internal void AddMedicineScheduleAndReminder()
        {
            IsPageEnabled = true;
            try
            {
                medicineReminder.ID = Guid.NewGuid().ToString();
                medicineReminder.unit = SelectedUnit;

                frequencies.ToList().ForEach(x => x.MedicineReminderId = medicineReminder.ID);

                _reminderRepo.AddReminder(medicineReminder, frequencies.ToList());

                if (medicineReminder.IsReminder)
                {
                    foreach (var item in frequencies)
                    {
                        var random = new Random();
                        int no = random.Next();

                        DependencyService.Get<INotify>().SetReminder(medicineReminder.EventName, $"Hi! It's time to take your {SelectedUnit}", no, item.Time, medicineReminder.From, medicineReminder.SpecificDays);
                    }
                }
                NavigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                App.dialogService.DisplayAlertAsync("Error!!", ex.Message, "OK");
            }
            finally
            {
                IsPageEnabled = false;
            }     
            
        }

        /// <summary>
        /// Initial loading of data
        /// </summary>

        private void LoadData()
        {
            SelectedUnit = Units.First();

            //frequencies = new ObservableCollection<FrequencyTime> { new FrequencyTime { Id = Guid.NewGuid().ToString() } };
            DoseFrequencies = new List<string>
            {
                AppResources.txtReminderList1,
                AppResources.txtReminderList2,
                AppResources.txtReminderList3
            };
            SelectedFreqIndex = DoseFrequencies.Count;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            if (parameters.TryGetValue("reminder",out MedicineReminder reminder))
            {
                medicineReminder = reminder;
                frequencies = new ObservableCollection<FrequencyTime>(reminder.FrequencyTimes);
                SelectedUnit = reminder.unit;
                
            }
            else
              LoadData();
        }

        
    }
}
