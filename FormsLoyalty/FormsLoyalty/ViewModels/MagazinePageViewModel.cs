using FormsLoyalty.Models;
using FormsLoyalty.Services;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using MagazineModel = LSRetail.Omni.Domain.DataModel.Loyalty.Magazine.MagazineModel;

namespace FormsLoyalty.ViewModels
{
    public class MagazinePageViewModel : ViewModelBase
    {
        private ObservableCollection<MagazineModel> _magazines;
        public ObservableCollection<MagazineModel> Magazines
        {
            get { return _magazines; }
            set { SetProperty(ref _magazines, value); }
        }

       

        public MagazinePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Task.Run(async() =>
            {
               await LoadMagazine();
            });
           
        }

        private async  Task LoadMagazine()
        {
            IsPageEnabled = true;
            try
            {

                //string token = await _magazineManager.GetAuthorizationToken("sahadev", "Linked@2020");

                //var magazines = await _magazineManager.GetMagazinesByToken(token);

                //foreach (var item in magazines.OrderBy(x =>x.created_at))
                //{
                //    item.ImageSource = $"{AppData.magazineURL}media/mageplaza/blog/post/{item.image}";

                //    var strlength = item.content.Length;

                //    int firstStringPosition = item.content.IndexOf("=&quot;");
                //    int secondStringPosition = item.content.IndexOf("&quot;}}");

                //    var endIndex = secondStringPosition - (firstStringPosition + 7);

                //    var str = item.content.Substring(firstStringPosition + 7, endIndex);
                //    item.Url = str;

                //}

               var magazines = await new CommonModel().GetMagazineAsync();

                  
                    Magazines = new ObservableCollection<MagazineModel>(magazines);
                   
                
            }
            catch (Exception ex)
            {
               await App.dialogService.DisplayAlertAsync("Error!!", ex.Message, "OK");
                IsPageEnabled = false;
            }
            IsPageEnabled = false;
        }

        internal async void NavigateToDetail(MagazineModel magazine)
        {
           // var url = $"{FormsLoyalty.Utils.AppData.magazineURL}/media/{magazine.Url}";

            var result = await Launcher.TryOpenAsync(new Uri(magazine.URL));

            //await NavigationService.NavigateAsync(nameof(MagazineDetail), new NavigationParameters { { "magazine", magazine } });
        }
    }
}
