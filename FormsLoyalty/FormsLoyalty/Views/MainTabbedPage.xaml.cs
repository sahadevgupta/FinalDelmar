using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.PopUpView;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
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
            hometab.IconImageSource = new FontImageSource() { FontFamily = "FontAwesome" , Glyph = FormsLoyalty.Resources.FontAwesomeIcons.Home };
            categorytab.IconImageSource = new FontImageSource() { FontFamily = "FontAwesome", Glyph = FormsLoyalty.Resources.FontAwesomeIcons.ThLarge };
            carttab.IconImageSource = new FontImageSource() { FontFamily = "FontAwesome", Glyph = FormsLoyalty.Resources.FontAwesomeIcons.ShoppingCart };
            carttabios.IconImageSource = new FontImageSource() { FontFamily = "FontAwesome", Glyph = FormsLoyalty.Resources.FontAwesomeIcons.ShoppingCart };
            //moretab.IconImageSource = new FontImageSource() { FontFamily = "FontAwesome", Glyph = FormsLoyalty.Resources.FontAwesomeIcons.Bars };

        }

        bool value = true;
        protected  override bool OnBackButtonPressed()
        {
            var selectedTabIndex = this.Children.IndexOf(this.CurrentPage);
            
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
            else
            {
                if (selectedTabIndex == 0)
                {
                   
                        var pageSatatus = true;
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            var page = new ExistPopupView();
                            page.ProceedClicked += async(s, e) =>
                            {
                                await Task.Delay(500);
                                DependencyService.Get<IAppSettings>().SwitchToBackground();
                            };
                            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page, true);
                        });
                        //DependencyService.Get<INotify>().ShowToast(AppResources.txtAppExist);
                        //value = false;
                        //Task.Delay(2000).ContinueWith((s, e) => { value = true; }, null);
                      
                }
                else
                {
                    Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                       await App.navigationService.NavigateAsync("app:///NavigationPage/MainTabbedPage");
                    });
                   
                }

                return true;
            }

           
        }
    
    }
}
