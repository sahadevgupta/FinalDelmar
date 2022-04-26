using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using Infrastructure.Data.SQLite.DB;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Repos
{
    public class ReminderRepo : IReminderRepo
    {
        private object locker = new object();
        public ReminderRepo()
        {
            DBHelper.OpenDBConnection();
            if (DBHelper.DBConnection != null)
            {
                var ReminderInfo = DBHelper.DBConnection.GetTableInfo(nameof(MedicineReminder));
                if (!ReminderInfo.Any())
                {
                    DBHelper.DBConnection.CreateTable<MedicineReminder>();
                }

                var FreqTimeInfo = DBHelper.DBConnection.GetTableInfo(nameof(FrequencyTime));
                if (!FreqTimeInfo.Any())
                {
                    DBHelper.DBConnection.CreateTable<FrequencyTime>();
                }
                
            }
        }

        public void AddReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies)
        {
            lock (locker)
            {
               

                DBHelper.DBConnection.Insert(medicineReminder);
                DBHelper.DBConnection.InsertAll(frequencies);
            }
        }

        public List<MedicineReminder> GetReminder(ReminderDate date)
        {
            List<MedicineReminder> medicineReminders = new List<MedicineReminder>();
            var reminders = GetTable();
            foreach (var reminder in reminders)
            {
                if (reminder.From.Day <= date.day)
                {
                    if (reminder.AllDay && reminder.NoOfDays > 0)
                    {
                        var remDate = reminder.From;
                        var noOfDays = remDate.AddDays(reminder.NoOfDays);
                        if (noOfDays.Month >= date.DateTime.Month)
                        {
                            medicineReminders.Add(reminder);
                        }
                        else
                        {
                            var freqs =  GetFrequenctTimeForReminderById(reminder.ID);
                            DeleteReminder(reminder, freqs);
                        }

                    }
                    else if (reminder.AllDay)
                    {
                        medicineReminders.Add(reminder);
                    }
                    else if (!string.IsNullOrEmpty(reminder.SpecificDays))
                    {
                        var frequencyIndex = reminder.SpecificDays.Split(",");
                        foreach (var index in frequencyIndex)
                        {
                            if (App.choices[Convert.ToInt32(index)] == date.DayofWeek)
                            {
                                medicineReminders.Add(reminder);
                            }
                        }
                    }
                    else
                        medicineReminders.Add(reminder);
                }

            }

            AssignFrequencyTimeToReminder(medicineReminders);


            return medicineReminders;
        }

        /// <summary>This method used to assign the no. of frequencies 
        /// to the remnder
        /// </summary> 
        /// <param name="medicineReminders"></param>
        /// <return></return>
        private void AssignFrequencyTimeToReminder(List<MedicineReminder> medicineReminders)
        {
            foreach (var item in medicineReminders)
            {
                var items = DBHelper.DBConnection.Table<FrequencyTime>().ToList();
                item.FrequencyTimes = new List<FrequencyTime>(DBHelper.DBConnection.Table<FrequencyTime>().Where(x => x.MedicineReminderId == item.ID));
            }
        }

        private List<FrequencyTime> GetFrequenctTimeForReminderById(string reminderId)
        {
            return DBHelper.DBConnection.Table<FrequencyTime>().Where(x => x.MedicineReminderId == reminderId).ToList();
        }

        public void UpdateReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies)
        {
            lock (locker)
            {
                DBHelper.DBConnection.Update(medicineReminder);
                DBHelper.DBConnection.InsertAll(frequencies);
            }
        }

        public void DeleteFrequencyTime(IList<FrequencyTime> frequencies)
        {
            foreach (var frequencyTime in frequencies)
            {

                DBHelper.DBConnection.Delete<FrequencyTime>(frequencyTime.ID);
            }
        }

        public void DeleteReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies)
        {
            lock (locker)
            {
                DBHelper.DBConnection.Delete<MedicineReminder>(medicineReminder.ID);
                DeleteAllNotification(new ObservableCollection<FrequencyTime>(frequencies));
                DeleteFrequencyTime(frequencies);

            }

        }
        public void DeleteReminderFrequency(MedicineReminder medicineReminder, FrequencyTime frequency)
        {
            lock (locker)
            {
                if (frequency.MedicineReminderId.Equals(medicineReminder.ID))
                {
                    DeleteNotification(frequency.NotificationId);
                    DBHelper.DBConnection.Delete<FrequencyTime>(frequency.ID);
                }
                    
               

            }
        }

        public TableQuery<MedicineReminder> GetTable()
        {
            var reminders = DBHelper.DBConnection.Table<MedicineReminder>();

            return AssignReminderTime(reminders);
        }

        private TableQuery<MedicineReminder> AssignReminderTime(TableQuery<MedicineReminder> reminders)
        {
            foreach (var item in reminders)
            {
                var freqTime = DBHelper.DBConnection.Table<FrequencyTime>().Where(x => x.MedicineReminderId == item.ID);
                if (freqTime.Any())
                {
                    item.FrequencyTimes = new List<FrequencyTime>(freqTime);
                }

            }

            return reminders;
        }

        public void DeleteAllNotification(IList<FrequencyTime> frequencies)
        {
            DependencyService.Get<INotify>().DeleteAllReminderNotification(frequencies.Select(x => x.NotificationId).ToList());
            DeleteFrequencyTime(frequencies);
        }

        public void DeleteNotification(int notificationId)
        {
            DependencyService.Get<INotify>().DeleteReminderNotification(notificationId);
        }
    }
}
