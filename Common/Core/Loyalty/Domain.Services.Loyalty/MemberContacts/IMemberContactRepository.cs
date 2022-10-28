
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;

namespace LSRetail.Omni.Domain.Services.Loyalty.MemberContacts
{
    public interface IMemberContactRepository
    {
        MemberContact UserLogon(string userName, string password, string deviceId);
        MemberContact UserLogonWithFaceBook(string FacebookID, string FaceBookEmail, string deviceId);
        MemberContact UserLogonWithGoogle(string GoogleID, string GoogleEmail, string deviceId);
        bool Logout(string userName, string deviceId);
        MemberContact CreateMemberContact(MemberContact memberContact);
        MemberContact UpdateMemberContact(MemberContact memberContact);
        long MemberContactGetPointBalance(string cardId);
        bool ChangePassword(string userName, string newPassword, string oldPassword);
        bool ResetPin(string contactId, string newPin);
        MemberContact ContactGetByCardId(string contactId);
        bool DeviceSave(string deviceId, string deviceFriendlyName, string platform, string osVersion, string manufacturer, string model);
		string ForgotPassword(string userNameOrEmail);
		bool ResetPassword(string userName, string resetCode, string newPassword);
        int AddNewAddress(string CardId, Address NewAddress);
        bool SendFCMToken(string fcmToken, string deviceId);
    }
}
