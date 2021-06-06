using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using Newtonsoft.Json;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace FormsLoyalty.ViewModels
{
    public class ItemCategoriesPageViewModel : MainTabbedPageViewModel
    {

        private ObservableCollection<ItemCategory> _itemCategories;
        public ObservableCollection<ItemCategory> itemCategories
        {
            get { return _itemCategories; }
            set { SetProperty(ref _itemCategories, value); }
        }

        private ObservableCollection<ProductGroup> _productGrps;
        public ObservableCollection<ProductGroup> Products
        {
            get { return _productGrps; }
            set { SetProperty(ref _productGrps, value); }
        }

        private ItemCategory _selectedCategory;
        public ItemCategory SelectedCategory
        {
            get { return _selectedCategory; }
            set { SetProperty(ref _selectedCategory, value); }
        }

        public ImageSize imageSize { get; set; }
        public ImageModel imageModel { get; set; }


        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand CurrentCategoryChangedCommand { get; set; }
        public bool IsNavigated { get; private set; }

        public ItemCategoriesPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //SetImageSize(context);
            imageModel = new ImageModel();
           

            SearchCommand = new DelegateCommand(async () => await GoToSearchPage());
            CurrentCategoryChangedCommand = new DelegateCommand(GetItemProductGroups);

            IsActiveChanged += ItemCategoriesPageViewModel_IsActiveChanged;
        }

        private void ItemCategoriesPageViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (!IsNavigated)
            {
                if (IsActive)
                {
                    LoadData();
                }
                else
                {
                    IsNavigated = false;
                    SelectedCategory = null;
                }
                    
            }
           
        }

        private async Task GoToSearchPage()
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(ItemSearchPage));
            IsPageEnabled = false;
        }

        private async void LoadData()
        {
            IsPageEnabled = true;
            try
            {
                await LoadCategories();

                SelectedCategory = itemCategories.FirstOrDefault();
                GetItemProductGroups();
            }
            catch (Exception)
            {

                IsPageEnabled = false;
            }

            IsPageEnabled = false;
           
        }

        private async Task LoadCategories()
        {
            if (AppData.ItemCategories == null)
            {

                var service = new ItemService(new LoyItemRepository());
                var cat = await service.GetItemCategoriesAsync();
                itemCategories = new ObservableCollection<ItemCategory>(cat);
                loadDataWithImage(itemCategories);

                AppData.ItemCategories = new List<ItemCategory>(itemCategories);
            }
            else
                itemCategories = new ObservableCollection<ItemCategory>(AppData.ItemCategories);
        }




        internal async void NavigateToItemPage(ProductGroup obj)
        {
            IsPageEnabled = true;
            await NavigationService.NavigateAsync(nameof(ItemGroupPage), new NavigationParameters { { "prodGroup", obj } });
            IsPageEnabled = false;
        }

        private void SetImageSize(object context)
        {
            
        }

       

        private void loadDataWithImage(ObservableCollection<ItemCategory> itemCategories)
        {
            try
            {
                Task.Run(async() =>
                {
                    foreach (var itemCategory in itemCategories)
                    {
                        if (itemCategory.Images.Count > 0)
                        {
                            var image = await ImageHelper.GetImageById(itemCategory.Images[0].Id, new ImageSize(396, 396));
                            itemCategory.Images[0].Image = image.Image;
                        }
                        else
                        itemCategory.Images = new List<ImageView> { new ImageView { Image = "noimage.png" } };

                    }
                });
            }
            catch (Exception)
            {

            }
           
        }

        /// <summary>
        /// This method used to fetch the product group of the selected category
        /// </summary>
        internal void GetItemProductGroups()
        {
            try
            {
                
                Products = new ObservableCollection<ProductGroup>(SelectedCategory.ProductGroups);

                Task.Run(async() =>
                {
                    foreach (var item in Products)
                    {
                        if (item.Images.Count > 0)
                        {
                            var image = await ImageHelper.GetImageById(item.Images[0].Id, new ImageSize(396, 396));
                            if (image!=null)
                            {
                                item.Images[0].Image = image.Image;
                            }
                            
                        }
                        else
                            item.Images = new List<ImageView> { new ImageView { Image = "noimage" } };

                    }
                });

               
            }
            catch (Exception)
            {

                
            }
            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var navigationMode = parameters.GetNavigationMode();
            switch (navigationMode)
            {
                case NavigationMode.Back:
                    IsNavigated = true;
                    break;
                case NavigationMode.New:
                    break;
                case NavigationMode.Forward:
                    break;
                case NavigationMode.Refresh:
                    break;
                default:
                    break;
            }

        }

       

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            IsNavigated = parameters.GetValue<bool>("fromPage");
            var cat = parameters.GetValue<ItemCategory>("item");
            if (cat != null)
            {
                Task.Run(async() =>
                {
                    await LoadCategories();
                    SelectedCategory = cat;
                    GetItemProductGroups();
                });
                
            }
           
            
        }
    }
}
