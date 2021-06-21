using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace FormsLoyalty.ViewModels
{
    public class OffersPageViewModel : ViewModelBase
    {

        private ObservableCollection<OfferGroup> _offers = new ObservableCollection<OfferGroup>();
        public ObservableCollection<OfferGroup> offers
        {
            get { return _offers; }
            set { SetProperty(ref _offers, value); }
        }
        private int _count = 2;
        public int count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        private int _horizontalSpac = 10;
        public int HorizontalSpac
        {
            get { return _horizontalSpac; }
            set { SetProperty(ref _horizontalSpac, value); }
        }

        private int _VerticalSpac = 10;
        public int VerticalSpac
        {
            get { return _VerticalSpac; }
            set { SetProperty(ref _VerticalSpac, value); }
        }


        public DelegateCommand ToolBarCommand { get; set; }
        public OffersPageViewModel(INavigationService navigationService ): base(navigationService)
        {
            ToolBarCommand = new DelegateCommand(async () => await OnToolbarItemClicked());
        }

        /// <summary>
        /// Add or Remove offer from QrCode
        /// </summary>
        /// <param name="publishedOffer"></param>
        internal void AddRemoveOffer(PublishedOffer publishedOffer)
        {
            IsPageEnabled = true;
            var model = new OfferModel();
            model.ToggleOffer(publishedOffer);
            IsPageEnabled = false;
        }

        /// <summary>
        /// Navigate to QrCode
        /// </summary>
        /// <returns></returns>
        private async Task OnToolbarItemClicked()
        {
            IsPageEnabled = true;
                await NavigationService.NavigateAsync(nameof(QRCodePage));
            IsPageEnabled = false;
        }

        /// <summary>
        /// Navigates to offer Detail page
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal async Task NavigateToOfferDetail(PublishedOffer data)
        {
            IsPageEnabled = true;
           await NavigationService.NavigateAsync(nameof(OfferDetailsPage),new NavigationParameters { { "offer",data} });
            IsPageEnabled = false;
        }
        /// <summary>
        /// Loaad Offer
        /// </summary>
        internal void LoadOffers()
        {
            //offers = new ObservableCollection<OfferGroup>();
            var temp = new ObservableCollection<OfferGroup>();
            IsPageEnabled = true;

            try
            {
                if (AppData.Device.UserLoggedOnToDevice == null)
                {
                    Task.Run(async () =>
                    {
                        await NavigationService.NavigateAsync("NavigationPage/LoginPage");
                    });

                    return;
                }

                var publishedOffers = AppData.Device.UserLoggedOnToDevice.PublishedOffers.Where(x => x.Code != OfferDiscountType.Coupon);

                var pointOfffer = publishedOffers.Where(x => x.Type == OfferType.PointOffer).ToList();
                var clubOfffer = publishedOffers.Where(x => x.Type == OfferType.Club).ToList();
                var memberOffer = publishedOffers.Where(x => x.Type == OfferType.SpecialMember).ToList();
                var generalOffer = publishedOffers.Where(x => x.Type == OfferType.General).ToList();

                if (pointOfffer.Any())
                {
                    temp.Add(new OfferGroup(
                    AppResources.ResourceManager.GetString("PointOffers", AppResources.Culture),
                    new List<PublishedOffer>(OfferWithImage(pointOfffer))

                ));
                }

                if (clubOfffer.Any())
                {
                    temp.Add(new OfferGroup(
                    AppResources.ResourceManager.GetString("ClubOffers", AppResources.Culture),
                    new List<PublishedOffer>(OfferWithImage(clubOfffer))

                ));
                }

                if (memberOffer.Any())
                {
                    temp.Add(new OfferGroup(
                  AppResources.ResourceManager.GetString("MemberOffers", AppResources.Culture),
                  new List<PublishedOffer>(OfferWithImage(memberOffer))

              ));
                }

                if (generalOffer.Any())
                {
                    
                        var data = new OfferGroup(AppResources.GeneralOffers, new List<PublishedOffer>(OfferWithImage(generalOffer)) );
                        temp.Add(data);
                   
                    
                }

                offers = new ObservableCollection<OfferGroup>(temp);
            }
            catch (Exception ex)
            {


            }

            finally
            {
                IsPageEnabled = false;
            }

        }

        /// <summary>
        /// Get Offers Image
        /// </summary>
        /// <param name="Offfers"></param>
        /// <returns></returns>
        private List<PublishedOffer> OfferWithImage(List<PublishedOffer> Offfers)
        {
            try
            {
                Task.Run(async () =>
                {
                    foreach (var offer in Offfers)
                    {
                        if (offer.Images.Count > 0)
                        {
                            var imageView = await ImageHelper.GetImageById(offer.Images[0].Id, new ImageSize(396, 396));
                            offer.Images[0].Image = imageView.Image;
                        }
                        else
                            offer.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };
                    }
                });
            }
            catch (Exception)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async() =>
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: "Unable to fetch image.",
                                           msDuration: MaterialSnackbar.DurationLong);
                });
                
            }

            return Offfers;
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public  override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
        }

       
    }

    public class OfferGroup : List<PublishedOffer>
    {
        public string Name { get; private set; }

        public OfferGroup(string name, List<PublishedOffer> offers) : base(offers)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
