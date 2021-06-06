using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class QRCodePageViewModel : ViewModelBase
    {

        private ImageSource _qrCode;
        public ImageSource qrCode
        {
            get { return _qrCode; }
            set { SetProperty(ref _qrCode, value); }
        }

        private ObservableCollection<OfferGroup> _offers = new ObservableCollection<OfferGroup>();
        public ObservableCollection<OfferGroup> offers
        {
            get { return _offers; }
            set { SetProperty(ref _offers, value); }
        }

        public QRCodePageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        private void LoadData()
        {
            var qrCodeBytes = FormsLoyalty.Utils.Utils.QrCodeUtils.GenerateQRCode(false);

            qrCode = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));

            SetItems();
        }

        public void SetItems()
        {
            

            if (EnabledItems.HasCoupons)
            {
               var coupons =  AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Selected && x.Code == OfferDiscountType.Coupon).ToList();
                if (coupons.Any())
                {
                    offers.Add(new OfferGroup(
                                  AppResources.ResourceManager.GetString("ActionbarCoupons", AppResources.Culture),
                                  new List<PublishedOffer>(OfferWithImage(coupons))
                              
                              ));
                }
            }

            if (EnabledItems.HasOffers)
            {

                var puboffers = AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Type == OfferType.PointOffer && x.Code != OfferDiscountType.Coupon && x.Selected).ToList();

                if (puboffers.Any())
                {
                    offers.Add(new OfferGroup(
                                  AppResources.ResourceManager.GetString("ActionbarOffers", AppResources.Culture),
                                  new List<PublishedOffer>(OfferWithImage(puboffers))

                              ));
                }
            }

           
        }

        private List<PublishedOffer> OfferWithImage(List<PublishedOffer> Offfers)
        {
            foreach (var offer in Offfers)
            {
                if (offer.Images.Count > 0)
                {
                    try
                    {
                        Task.Run(async () =>
                        {
                            var imageView = await ImageHelper.GetImageById(offer.Images[0].Id, new ImageSize(396, 396));
                            offer.Images[0].Image = imageView.Image;
                        });
                    }
                    catch (Exception)
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                        {
                            await MaterialDialog.Instance.SnackbarAsync(message: "Unable to fetch image.",
                                                   msDuration: MaterialSnackbar.DurationLong);
                        });
                    }

                }
            }
            return Offfers;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            LoadData();

            
        }

       
    }

    
}
