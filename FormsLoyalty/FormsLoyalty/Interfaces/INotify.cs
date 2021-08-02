using LSRetail.Omni.Domain.DataModel.Base.Retail;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormsLoyalty.Interfaces
{
    public interface INotify
    {
        void ShowLocalNotification(string title, string body);
        void ShowToast(string description);
        void ShowSnackBar(string description);
        void ShowLocalNotification(string title, string body, int id, DateTime notifyTime);
        string getDeviceUuid();

        string GetImageUri(string encodedImage,string name);

        void SetReminder(string title, string body, int id, TimeSpan time, DateTime startDate, string days);

        void DeleteReminderNotification(int notificationId);
        void DeleteAllReminderNotification(List<int> notificationIds);

        void ChangeTabBarFlowDirection(bool rtl);

    }
}
