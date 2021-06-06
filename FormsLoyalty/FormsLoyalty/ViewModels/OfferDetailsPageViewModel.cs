using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using Prism;
using Prism.Ioc;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using FormsLoyalty.Helpers;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using FormsLoyalty.Views;
using FormsLoyalty.Models;

namespace FormsLoyalty.ViewModels
{
    public class OfferDetailsPageViewModel : ViewModelBase
    {
        private PublishedOffer _selectedOffer;
        public PublishedOffer selectedOffer
        {
            get { return _selectedOffer; }
            set { SetProperty(ref _selectedOffer, value); }
        }

        private ObservableCollection<LoyItem> _relatedItems = new ObservableCollection<LoyItem>();
        public ObservableCollection<LoyItem> relatedItems
        {
            get { return _relatedItems; }
            set { SetProperty(ref _relatedItems, value); }
        }

        #region Command
        public DelegateCommand ShowPreviewCommand => new DelegateCommand(async () =>
        {
            if (string.IsNullOrEmpty(selectedOffer.Images[0].Image) || selectedOffer.Images[0].Image.ToLower().Contains("noimage".ToLower()))
                return;

            await NavigationService.NavigateAsync(nameof(ImagePreviewPage), new NavigationParameters { { "previewImage", selectedOffer.Images[0].Image }, { "images", selectedOffer.Images } });
        });
        #endregion
        public OfferDetailsPageViewModel(INavigationService navigationService):base(navigationService)
        {
        }

        /// <summary>
        /// Add or Remove offer from QrCode
        /// </summary>
        internal void AddRemoveOffer()
        {
            var model = new OfferModel();
            model.ToggleOffer(selectedOffer);
        }

        /// <summary>
        /// Load Offer's Items
        /// </summary>
        private void LoadRelatedItems()
        {
            IsPageEnabled = true;
            try
            {
                var service = new ItemService(new LoyItemRepository());

                Task.Run(async () =>
                {
                    var loyItems = await service.GetItemsByPublishedOfferIdAsync(selectedOffer.Id, Int32.MaxValue);

                    relatedItems = new ObservableCollection<LoyItem>(loyItems);

                    foreach (var item in relatedItems)
                    {
                        if (item.Images.Count > 0)
                        {
                            var img = await ImageHelper.GetImageById(item.Images[0].Id, new ImageSize(396, 396));
                            item.Images[0].Image = img.Image;
                        }
                        else
                            item.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };
                    }

                });
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }
            IsPageEnabled = false;

        }

        /// <summary>
        /// This method is used when offer's item is clicked. It navigates to 
        /// Item Page
        /// </summary>
        /// <param name="loyItem"></param>
        /// <returns></returns>
        internal async Task NavigateToItemPage(LoyItem loyItem)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", loyItem } });
            IsPageEnabled = false;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            selectedOffer = parameters.GetValue<PublishedOffer>("offer");
            selectedOffer.PropertyChanged += SelectedOffer_PropertyChanged;
            LoadRelatedItems();
        }

        private void SelectedOffer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }
    }
}
