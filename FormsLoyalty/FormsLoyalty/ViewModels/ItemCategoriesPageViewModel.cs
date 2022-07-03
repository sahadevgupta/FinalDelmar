using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using FormsLoyalty.Utils;
using FormsLoyalty.Views;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.Services.Loyalty.Items;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using Microsoft.AppCenter.Crashes;
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
        bool IsInitialized = false;
        private void ItemCategoriesPageViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (!IsNavigated)
            {
                if (IsActive)
                {
                    if (!IsInitialized)
                    {
                        IsInitialized = true;
                        LoadData();
                    }
                    else
                    {
                        SelectedCategory = itemCategories.FirstOrDefault();
                    }
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

        private void LoadData()
        {
            Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(async () =>
            {
                IsPageEnabled = true;
                try
                {
                    await LoadCategories();

                    SelectedCategory = itemCategories.FirstOrDefault();
                    GetItemProductGroups();
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
                finally
                {
                    IsPageEnabled = false;
                }
            });

            
           
        }

        private async Task LoadCategories()
        {
            if (AppData.ItemCategories == null)
            {

                var service = new ItemService(new LoyItemRepository());
                var cat = await service.GetItemCategoriesAsync();
                itemCategories = new ObservableCollection<ItemCategory>(cat);
                //loadDataWithImage(itemCategories);

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

       

        /// <summary>
        /// This method used to fetch the product group of the selected category
        /// </summary>
        internal void GetItemProductGroups()
        {
             Products = new ObservableCollection<ProductGroup>(SelectedCategory?.ProductGroups);    
            
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
