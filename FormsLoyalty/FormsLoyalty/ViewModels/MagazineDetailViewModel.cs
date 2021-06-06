using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using LSRetail.Omni.Domain.DataModel.Loyalty.Magazine;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class MagazineDetailViewModel : ViewModelBase
    {
        private string _url;
        public string URL
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public MagazineDetailViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        public async override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            var magazine = parameters.GetValue<MagazineModel>("magazine");
            Title = magazine.Description;
           var url = $"{FormsLoyalty.Utils.AppData.magazineURL}/media/{magazine.URL}";

            await LoadData(url);
        }

        private async Task LoadData(string url)
        {
            var localPath = string.Empty;
            IsPageEnabled = true;
            try
            {
                  if (Device.RuntimePlatform == Device.Android)
                    {
                        var dependency = DependencyService.Get<ILocalFileProvider>();

                        if (dependency == null)
                        {
                           await App.dialogService.DisplayAlertAsync("Error loading PDF", "Computer says no", "OK");

                            return;
                        }

                        var fileName = Guid.NewGuid().ToString();

                        // Download PDF locally for viewing
                        using (var httpClient = new HttpClient())
                        {
                            var pdfStream = Task.Run(() => httpClient.GetStreamAsync(url)).Result;

                            localPath = await Task.Run(async () => await dependency.SaveFileToDisk(pdfStream, $"{fileName}.pdf"));
                        }

                        if (string.IsNullOrWhiteSpace(localPath))
                        {
                          await  App.dialogService.DisplayAlertAsync("Error loading PDF", "Computer says no", "OK");

                            return;
                        }
                    }

                    if (Device.RuntimePlatform == Device.Android)
                        URL = $"file:///android_asset/pdfjs/web/viewer.html?file={"file:///" + WebUtility.UrlEncode(localPath)}";
                    else
                        URL = url;
               
               
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }

            IsPageEnabled = false;
        }
    }
}
