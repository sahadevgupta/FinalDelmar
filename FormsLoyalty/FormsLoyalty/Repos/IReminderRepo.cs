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
        List<MedicineReminder> GetReminder(ReminderDate date);
        TableQuery<MedicineReminder> GetTable();
        void UpdateReminder(MedicineReminder medicineReminder, List<FrequencyTime> frequencies);
    }
}