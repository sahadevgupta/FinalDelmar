using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class ContactUsPageViewModel : BindableBase
    {
        private ObservableCollection<DrawerMenuItem> _contactMethods;
        public ObservableCollection<DrawerMenuItem> ContactMethods
        {
            get { return _contactMethods; }
            set { SetProperty(ref _contactMethods, value); }
        }


        public DelegateCommand<DrawerMenuItem> ContactMethodCommand { get; set; }
        public ContactUsPageViewModel()
        {
            ContactMethodCommand = new DelegateCommand<DrawerMenuItem>(async(obj) => await OnContactMethod_Tapped(obj));

            ContactMethods = new ObservableCollection<DrawerMenuItem>
            {
                new DrawerMenuItem{ Id =1, Title = "19955" , Image = "iconCall" },
                 new DrawerMenuItem{Id =2, Title = "https://www.facebook.com/DelmarWeAttalla/" , Image = "iconFacebook" },
                  new DrawerMenuItem{Id =3, Title = "https://www.instagram.com/delmarweattalla/" , Image = "Instagram" },
                   new DrawerMenuItem{Id =4, Title = "01002199551" , Image = "WhatsApp" },
            };

        }

        private async Task OnContactMethod_Tapped(DrawerMenuItem obj)
        {
            switch (obj.Id)
            {
                case 1:
                    PhoneDialer.Open(obj.Title);
                    break;
                case 2:

                    try
                    {
                        var fbUri = $"fb://page/324975484267147";
                        await Launcher.OpenAsync(fbUri);
                    }
                    catch (Exception)
                    {

                        await Launcher.OpenAsync(obj.Title);
                    }

                   
                    break;
                case 3:
                   
                    await Launcher.OpenAsync(obj.Title);
                    break;
                case 4:
                    var uri = $"whatsapp://send?phone=+2{obj.Title}";
                    await Launcher.OpenAsync(uri);
                    break;
                default:
                    break;
               
            }
        }
    }
}
