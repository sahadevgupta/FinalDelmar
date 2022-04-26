using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FormsLoyalty.ViewModels
{
    public class ItemSearchPageViewModel : ViewModelBase
    {
        private string _SearchKey;
        public string SearchKey
        {
            get { return _SearchKey; }
            set 
            { 
                SetProperty(ref _SearchKey, value);
                if (!string.IsNullOrEmpty(value))
                {
                    OnSearchQuery();
                }
            
            }
        }

        private ObservableCollection<LoyItem> _Items;
        public ObservableCollection<LoyItem> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }

        private int pageSize = 7;
        private int pageNumber = 1;
        private int lastSearchLength;

        public DelegateCommand<LoyItem> SelectedCommand { get; set; }
        public ItemSearchPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            SelectedCommand = new DelegateCommand<LoyItem>(async (data) => await NaviagteToItemPage(data));
        }

        private async Task NaviagteToItemPage(LoyItem data)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(ItemPage), new NavigationParameters { { "item", data } });
            IsPageEnabled = false;
        }

        public async void OnSearchQuery()
        {
            IsPageEnabled = true;
            if (SearchKey.Length > 2)
            {
                if (SearchKey.Length > lastSearchLength)
                  await  LoadSearch();
            }

            lastSearchLength = SearchKey.Length;

            IsPageEnabled = false;
        }

        private async Task LoadSearch()
        {
            try
            {
                var model = new ItemModel();
                var items = await model.GetItemsByPage(pageSize, pageNumber, string.Empty, string.Empty, SearchKey,false,string.Empty);
                Items = new ObservableCollection<LoyItem>(items);

                //LoadImages();
            }
            catch (Exception)
            {
                IsPageEnabled = false;  
               
            }
        }

        private void LoadImages()
        {
            try
            {
                Task.Run(async() =>
                {
                    foreach (var item in Items)
                    {
                        if (item.Images.Count > 0)
                        {
                            var imgView = await ImageHelper.GetImageById(item.Images[0].Id, new LSRetail.Omni.Domain.DataModel.Base.Retail.ImageSize(110, 110));
                            item.Images[0].Image = imgView.Image;
                        }
                        else
                            item.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };

                       
                    }
                });
            }
            catch (Exception)
            {

                
            }
        }
    }
}
