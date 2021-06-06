using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Members;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.QrCode.Internal;

namespace FormsLoyalty.ViewModels
{
    public class AccountProfilePageViewModel : ViewModelBase
    {

        private ObservableCollection<ProfileGroup> _ProfileGroup;
        public ObservableCollection<ProfileGroup> ProfileGroup
        {
            get { return _ProfileGroup; }
            set { SetProperty(ref _ProfileGroup, value); }
        }

        public bool LoadAccountProfiles { get; set; }

        public DelegateCommand CreateCommand { get; set; }
        public MemberContact MemberContact { get; private set; }
        public string Username { get; private set; }

        private bool _editAccount;
        public bool editAccount
        {
            get { return _editAccount; }
            set { SetProperty(ref _editAccount, value); }
        }

        public bool FromItemPage { get; private set; }

        MemberContactModel model;
        public AccountProfilePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            CreateCommand = new DelegateCommand(async() => await CreateUpdateMemberContact());
            model = new MemberContactModel();
        }

        private async Task CreateUpdateMemberContact()
        {
            IsPageEnabled = true;

            var profile = ProfileGroup.Where(x => x.isChecked).Select(x => x.Profile);

            MemberContact.Profiles = new List<Profile>(profile);

           

            if (MemberContact.Cards.Count <= 0)
            {
                MemberContact.Cards.Add(new Card()
                {
                    Id = AppData.Device.CardId
                });
            }






            if (editAccount)
            {
                if (MemberContactAttributes.Manage.Username)
                {
                    MemberContact.UserName = AppData.Device.UserLoggedOnToDevice.UserName;
                }
                else
                {
                    MemberContact.UserName = MemberContact.Email;
                }

                MemberContact.Password = AppData.Device.UserLoggedOnToDevice.Password;



                try
                {
                    var success = await model.UpdateMemberContact(MemberContact);

                    if (success)
                    {
                        //await NavigationService.GoBackAsync(new NavigationParameters { { "updated", true } });
                        await NavigationService.NavigateAsync("../../");
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    UpdateMemberContactError(ex);
                }
            }
            else
            {
                // sign up
                if (MemberContactAttributes.Manage.Username)
                {
                    MemberContact.UserName = Username;
                }
                else
                {
                    MemberContact.UserName = MemberContact.Email;
                }



                try
                {

                    var success = await model.CreateMemberContact(MemberContact);

                    if (success)
                    {
                        if (FromItemPage)
                        {
                            MessagingCenter.Send((App)Xamarin.Forms.Application.Current, "LoggedIn");
                            await NavigationService.NavigateAsync("../../../");
                        }
                        else
                        await NavigationService.NavigateAsync("app:///HomeMasterDetailPage");
                    }
                    else
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                        {
                            await App.dialogService.DisplayAlertAsync("Error!!", AppData.Msg, "OK");
                        });
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    IsPageEnabled = false;
                    CreateMemberContactError(ex);
                }
            }
            IsPageEnabled = false;
        }
        private async void UpdateMemberContactError(Exception ex)
        {

            if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailInvalid)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Email Invaild", "OK");
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailExists)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Email already Exists", "OK");
            }


        }

        private async void CreateMemberContactError(Exception ex)
        {

            if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.UserNameInvalid || ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.UserNameExists)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Username Invaild or Username already exists", "OK");
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.PasswordInvalid)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Password Invaild", "OK");
            }
            else if (ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailExists || ex is LSOmniException && ((LSOmniException)ex).StatusCode == StatusCode.EmailInvalid)
            {
                await App.dialogService.DisplayAlertAsync("Error!!", "Email Invaild or Email already exists", "OK");
            }


        }

        private async Task LoadProfilesFromWs()
        {
            IsPageEnabled = true;
            ProfileGroup = new ObservableCollection<ProfileGroup>();
           
                try
                {
                    var model = new ProfileModel();
                    if (LoadAccountProfiles)
                    {
                        var data = await model.ProfilesGetByCardId(AppData.Device.CardId);
                        LoadProfiles(data);
                    }
                    else
                    {
                        var profiles = await model.ProfilesGetAll();
                        LoadProfiles(profiles);
                    }
                }
                catch (Exception ex)
                {
                Crashes.TrackError(ex);
                DependencyService.Get<INotify>().ShowToast("Unable to load profiles");
                }

            IsPageEnabled = false;
               
            
        }
        private void LoadProfiles(List<Profile> profiles)
        {
            foreach (var profile in profiles)
            {
                var profileExist = MemberContact.Profiles.FirstOrDefault(x => x.Id == profile.Id);
                if (profileExist!=null)
                {
                    ProfileGroup.Add(new ViewModels.ProfileGroup { Profile = profile, isChecked = true });
                }
                else
                ProfileGroup.Add(new ViewModels.ProfileGroup { Profile = profile });
            }

            if (LoadAccountProfiles)
            {
                AppData.Device.UserLoggedOnToDevice.Profiles = profiles;
            }

        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            parameters.Add("edit", editAccount);
        }
        public async override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            MemberContact = parameters.GetValue<MemberContact>("contact");
            Username = parameters.GetValue<string>("username");
            editAccount = parameters.GetValue<bool>("edit");
            FromItemPage = parameters.GetValue<bool>("itemPage");
            await LoadProfilesFromWs();
        }
    }

    public class ProfileGroup : BindableBase
    {
        private bool _isChecked;
        public bool isChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
        }

        private Profile profile;
        public Profile Profile
        {
            get { return profile; }
            set { SetProperty(ref profile, value); }
        }
    }
}
