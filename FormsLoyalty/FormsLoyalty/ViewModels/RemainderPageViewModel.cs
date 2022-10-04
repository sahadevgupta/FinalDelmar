﻿using FormsLoyalty.Models;
using FormsLoyalty.Repos;
using FormsLoyalty.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class RemainderPageViewModel : ViewModelBase
    {
        

        private ObservableCollection<ReminderDate> _reminders;
        public ObservableCollection<ReminderDate> dates
        {
            get { return _reminders; }
            set { SetProperty(ref _reminders, value); }
        }

        private ReminderDate _selectedReminder;
        public ReminderDate SelectedDate
        {
            get { return _selectedReminder; }
            set { SetProperty(ref _selectedReminder, value); }
        }

        private Dictionary<string,List<MedicineReminder>> _medicineRemindersDict;
        public Dictionary<string,List<MedicineReminder>> MedicineReminderDict
        {
            get { return _medicineRemindersDict; }
            set { SetProperty(ref _medicineRemindersDict, value); }
        }

      

        readonly IReminderRepo _reminderRepo;

        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand<object> EditCommand { get; set; }
        public RemainderPageViewModel(INavigationService navigationService,IReminderRepo reminderRepo) : base(navigationService)
        {
            _reminderRepo = reminderRepo;
            EditCommand = new DelegateCommand<object>(async(data) => await EditReminder(data));

            LoadData();
        }

        public void DeleteReminder(string key, MedicineReminder reminder)
        {

            if (reminder.FrequencyTimes.Count >1)
            {
                var data = key.Split(':');
                int hours;
                if (data[1].Contains("PM"))
                {
                    var hr = Convert.ToInt32(data[0]);
                    
                    hours = hr < 12 ? hr + 12 : hr;
                }
                else
                    hours = Convert.ToInt32(data[0]);

                if (hours == 12)
                    hours = 0;

                var minutes = Convert.ToInt32(data[1].Substring(0, 2));

                var time = new TimeSpan(hours, minutes, 0);

                var record = reminder.FrequencyTimes.FirstOrDefault(x => x.Time == time);
                _reminderRepo.DeleteReminderFrequency(reminder, record);
            }
            else
             _reminderRepo.DeleteReminder(reminder, reminder.FrequencyTimes);

            GetReminder();
        }

        private async Task EditReminder(object data)
        {
           await NavigationService.NavigateAsync(nameof(AddReminderPage), new NavigationParameters { { "reminder", data as MedicineReminder } });
        }

        private void LoadData()
        {
            dates = new ObservableCollection<ReminderDate>();


            var datetimes = GetDates(DateTime.Now.Year, DateTime.Now.Month);
            foreach (var item in datetimes)
            {
                var reminder = new ReminderDate();
                reminder.day = item.Day;
                reminder.DayofWeek = item.DayOfWeek.ToString().Substring(0, 3);
                reminder.DateTime = item;
                if (DateTime.Now.Day == item.Day)
                {
                    reminder.IsSelected = true;
                }

                dates.Add(reminder);

            }
            
            SelectedDate = dates.First(x => x.IsSelected);

            GetReminder();
        }

        internal async Task NavigateToAddReminderPage()
        {
           await NavigationService.NavigateAsync(nameof(AddReminderPage));
        }

        public List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(d => new DateTime(year, month, d)).ToList();
        }
        internal void GetReminder()
        {

            var tempDict = new Dictionary<string, List<MedicineReminder>>();
            var reminders = _reminderRepo.GetReminder(SelectedDate);
            foreach (var reminder in reminders)
            {
                foreach (var FrequencyTime in reminder.FrequencyTimes)
                {
                    reminder.DoseQty = FrequencyTime.Qty;
                    var timeformat = ChangeTimeFormat(FrequencyTime.Time);
                    if (tempDict.ContainsKey(timeformat))
                    {
                        tempDict[timeformat].Add(reminder);
                    }
                    else
                        tempDict.Add(timeformat, new List<MedicineReminder> { reminder });
                }
               
            }
           
            MedicineReminderDict = new Dictionary<string, List<MedicineReminder>>(tempDict.OrderBy(x => x.Key));
        }
        /// <summary>
        ///  This method changes time format with respect to 12hr format
        /// </summary>
        /// <param name="time"></param>
        /// <example>8:00 Am</example>
        /// <returns>time in string format</returns>

        public string ChangeTimeFormat(TimeSpan time)
        {
            var hours = time.Hours;
            var minutes = time.Minutes;
            var amPmDesignator = "AM";
            if (hours == 0)
                hours = 12;
            else if (hours == 12)
                amPmDesignator = "PM";
            else if (hours > 12)
            {
                hours -= 12;
                amPmDesignator = "PM";
            }
            var formattedTime =
              String.Format("{0}:{1:00} {2}", hours, minutes, amPmDesignator);

            return formattedTime;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var navigationMode = parameters.GetNavigationMode();
            switch (navigationMode)
            {
                case NavigationMode.Back:
                    GetReminder();
                    break;
                case NavigationMode.New:
                    break;
                case NavigationMode.Forward:
                    break;
                case NavigationMode.Refresh:
                    break;
                default:
                    break;
            }
        }
    }

    public class ReminderDate : BindableBase
    {
        public int  day { get; set; }
        public string DayofWeek { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public DateTime DateTime { get; set; }
    }

    
}
