using FormsLoyalty.Models;
using FormsLoyalty.ViewModels;
using SQLite;
using System.Collections.Generic;

namespace FormsLoyalty.Repos
{
    public interface IReminderRepo
    {
        void AddReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies);
        void DeleteReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies);
        void DeleteReminderFrequency(MedicineReminder medicineReminder, FrequencyTime frequency);
        List<MedicineReminder> GetReminder(ReminderDate date);
        TableQuery<MedicineReminder> GetTable();
        void UpdateReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies);
        void DeleteAllNotification(IList<FrequencyTime> frequencies);

        void DeleteNotification(int notificationId);
    }
}