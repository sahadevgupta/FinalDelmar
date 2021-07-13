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
        MainTabbedPageViewModel _viewModel;
        public event EventHandler OnTabSelected;
        private bool _isTabPageVisible;
        public static readonly BindableProperty SelectedTabIndexProperty =
           BindableProperty.Create(
               nameof(SelectedTabIndex),
               typeof(int),
               typeof(MainTabbedPage), 0,
               BindingMode.TwoWay, null
               );
       
        public int SelectedTabIndex
        {
            get { return (int)GetValue(SelectedTabIndexProperty); }
            set { SetValue(SelectedTabIndexProperty, value); }
        }

        public MainTabbedPage()
        {
            InitializeComponent();
           
            _viewModel = BindingContext as MainTabbedPageViewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.CheckCartCount(null);
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
                Page.CurrentPage.Navigation.PopAsync();
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
        protected void tabChanged(object sender, EventArgs args)
    {
        //The children are navigation pages so we can easily pop to root of previous selected page 
        //if you have something else than NavigationPages make sure to get the good reference 
        //Keep an integer as reference to the currently selected page index within your class 
        //Pop to root for previous index 


        ((NavigationPage)this.Children[SelectedTabIndex]).PopToRootAsync(); //assign page to index 
        this.SelectedTabIndex = this.Children.IndexOf(this.CurrentPage);
    }
    }
}
