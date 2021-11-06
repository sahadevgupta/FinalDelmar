using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.Dialogs;

namespace FormsLoyalty.Views
{
    public partial class MainTabbedPage : TabbedPage
    {
        
        public MainTabbedPage()
        {
            InitializeComponent();
           
           
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
           
        }

        bool value = true;
        protected override bool OnBackButtonPressed()
        {
            var selectedTabIndex = this.Children.IndexOf(this.CurrentPage);
            //if (selectedTabIndex == 0)
            //{
            //    var Page = (this.Children[selectedTabIndex] as NavigationPage);

            //    if (Page.CurrentPage != Page.RootPage)
            //    {
            //        Page.CurrentPage.Navigation.PopAsync();
            //        return true;
            //    }
            //}
            //else if (selectedTabIndex == 1)
            //{
            //    var currentPage = (this.Children[selectedTabIndex] as NavigationPage).CurrentPage;
            //    if (!currentPage.Title.Equals(AppResources.txtCategory))
            //    {
            //        currentPage.Navigation.PopAsync();
            //        return true;
            //    }
            //}
            //else if (selectedTabIndex == 2)
            //{
            //    var currentPage = (this.Children[selectedTabIndex] as NavigationPage).CurrentPage;
            //    if ( !currentPage.Title.Equals(AppResources.CartTab))
            //    {
            //        currentPage.Navigation.PopAsync();
            //        return true;
            //    }
            //}
            //else if (selectedTabIndex == 3)
            //{
            //    var currentPage = (this.Children[selectedTabIndex] as NavigationPage).CurrentPage;
            //    if (!currentPage.Title.Equals(AppResources.OrdersTab))
            //    {
            //        currentPage.Navigation.PopAsync();
            //        return true;
            //    }
            //}
            //else if (selectedTabIndex == 4)
            //{
            //    var currentPage = (this.Children[selectedTabIndex] as NavigationPage).CurrentPage;
            //    if (!currentPage.Title.Equals(AppResources.MoreTab))
            //    {
            //        currentPage.Navigation.PopAsync();
            //        return true;
            //    }
            //}

            var Page = (this.Children[selectedTabIndex] as NavigationPage);

            if (Page.CurrentPage != Page.RootPage)
            {
                if (selectedTabIndex == 2)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var result = await this.DisplayAlert("Alert!", AppResources.txtExist, AppResources.ApplicationYes, AppResources.ApplicationNo);
                        if (result)
                        {
                            await Page.CurrentPage.Navigation.PopAsync();
                        }// or anything else
                    });
                }
                else
                {
                    Page.CurrentPage.Navigation.PopAsync();
                }
                
                return true;
            }


            if (value)
            {
               
                DependencyService.Get<INotify>().ShowToast(AppResources.txtAppExist);
                value = false;
                Task.Delay(2000).ContinueWith((s, e) => { value = true; }, null);
                return true;
            }
            else
                return false;
        }
    
    }
}
