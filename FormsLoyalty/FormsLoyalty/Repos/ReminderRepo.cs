using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using Infrastructure.Data.SQLite.DB;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                        if (noOfDays.Day >= date.day)
                        {
                            medicineReminders.Add(reminder);
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
                item.FrequencyTimes = new List<FrequencyTime>(DBHelper.DBConnection.Table<FrequencyTime>().Where(x => x.MedicineReminderId == item.ID));
            }
        }

        public void UpdateReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies)
        {
            lock (locker)
            {
                DBHelper.DBConnection.Update(medicineReminder);
                DBHelper.DBConnection.UpdateAll(frequencies);
            }
        }

        public void DeleteReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies)
        {
            lock (locker)
            {
                DBHelper.DBConnection.Delete<MedicineReminder>(medicineReminder.ID);
                foreach (var frequencyTime in frequencies)
                {
                    DBHelper.DBConnection.Delete<FrequencyTime>(frequencyTime.ID);
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
    }
}
