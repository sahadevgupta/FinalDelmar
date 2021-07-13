using FFImageLoading;
using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Services;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Loyalty.ScanSend;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.MemberContacts;
using Newtonsoft.Json;
using Prism;
using Prism.Commands;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class ScanSendPageViewModel : ViewModelBase
    {
        private ObservableCollection<ScanSend> _ImageList;
        public ObservableCollection<ScanSend> ImageList
        {
            get { return _ImageList; }
            set { SetProperty(ref _ImageList, value); }
        }
        private string _Remark;
        public string Remark
        {
            get { return _Remark; }
            set { SetProperty(ref _Remark, value); }
        }

        private bool _isPrescriptionViewVisible;
        public bool IsPrescriptionViewVisible
        {
            get { return _isPrescriptionViewVisible; }
            set { SetProperty(ref _isPrescriptionViewVisible, value); }
        }

        public DelegateCommand ProceedCommand { get; set; }
        public DelegateCommand PrescriptionCommand { get; set; }
        public bool CanNavigate { get;  set; }

        IScanSendManager _scanSendManager;

        public ScanSendPageViewModel(INavigationService navigationService,IScanSendManager scanSendManager) : base(navigationService)
        {
            _scanSendManager = scanSendManager;
            ImageList = new ObservableCollection<ScanSend>();
            ProceedCommand = new DelegateCommand(async() => await UploadScanSend());
            PrescriptionCommand = new DelegateCommand(OnPrescription_Tapped);
        }

        private void OnPrescription_Tapped()
        {
            IsPrescriptionViewVisible = true;
        }

        internal async void NavigateToCameraView()
        {
            await NavigationService.NavigateAsync(nameof(CameraPage), null, true, false);

        }

        private async Task UploadScanSend()
        {
            IsPageEnabled = true;
            try
            {
                var Id = Guid.NewGuid().ToString(); 
                ImageList.ForEach(x => { x.CreationDate = DateTime.UtcNow; x.pComment = string.IsNullOrEmpty(Remark)? string.Empty : Remark; x.Description = Remark; x.pOrderNo = Id; }) ;
                var IsSuccess = await _scanSendManager.CreateScanSend(ImageList.ToList());
                
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async() =>
                {
                    var msg = IsSuccess ? "Uploaded Successfully!!" : "Unable to communicate with server";
                   await App.dialogService.DisplayAlertAsync("Alert!!", msg, "OK");
                    if(IsSuccess)
                      await NavigationService.GoBackToRootAsync();
                });

               
               
            }
            catch (Exception)
            {

                
            }

            IsPageEnabled = false;
        }

        private void GetFileData(App arg1, List<Tuple<byte[], string>> arg2)
        {
            if (CanNavigate)
                return;
            CanNavigate = true;
            var i = ImageList.Count();
            AddImage(arg2, i + 1);

            CanNavigate = false;


        }

        internal async Task TakePickure()
        {
            IsPageEnabled = true;
           


            var file = await ImageHelper.TakePictureAsync();
            if (file != null)
            {

                //if (file.GetStream().ToByteArray().Length > 2097152)
                //{
                //    await MaterialDialog.Instance.AlertAsync(AppResources.txtImageSizeExceed, AppResources.txtImageSizeError, AppResources.ApplicationOk);
                //    return;
                //}

                var extension = Path.GetExtension(file.Path);
                var scansend = new ScanSend
                {
                    id = ImageList.Count() + 1,
                    imageExtension = extension.Replace(".", ""),
                    ImagedBase64 = Convert.ToBase64String(file.GetStream().ToByteArray()),
                     ContactNo = AppData.Device.CardId
                };

                ImageList.Add(scansend);
            }

            IsPageEnabled = false;
        }

        bool IsSizeExceeds;
        private int AddImage(List<Tuple<byte[], string>> imgInfo, int i)
        {
          
            foreach (var item in imgInfo)
            {
                //if(item.Item1.Length > 2097152)
                //{
                //    IsSizeExceeds = true;
                //    continue;
                //}
                var scansend = new ScanSend();
                scansend.ImagedBase64 = Convert.ToBase64String(item.Item1);
                scansend.id = i + 1;
                scansend.imageExtension = item.Item2;
                scansend.ContactNo = AppData.Device.CardId;
                ImageList.Add(scansend);
                i++;
            }
            //if(IsSizeExceeds)
            //  MaterialDialog.Instance.AlertAsync(AppResources.txtImgListSizeExceed, AppResources.txtImageSizeError, AppResources.ApplicationOk);

            return i;
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            if (parameters.ContainsKey("images"))
            {
                var imgData = parameters.GetValue<List<Tuple<byte[], string>>>("images");
                int i = 0;
                i = AddImage(imgData, i);
            }
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            MessagingCenter.Subscribe<App, List<Tuple<byte[], string>>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", GetFileData);
            var navigationMode = parameters.GetNavigationMode();
            if (navigationMode == NavigationMode.Back)
            {
                
                var imgData = parameters.GetValue<List<Tuple<byte[], string>>>("images");
                var scansend = new ScanSend
                {
                    id = ImageList.Count() + 1,
                    imageExtension = imgData[0].Item2,
                    ImagedBase64 = Convert.ToBase64String(imgData[0].Item1),
                    ContactNo = AppData.Device.CardId
                };

                ImageList.Add(scansend);


                   
            }

        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            MessagingCenter.Unsubscribe<App, List<Tuple<byte[], string>>>((App)Xamarin.Forms.Application.Current, "ImagesSelected");
        }


    }
}
