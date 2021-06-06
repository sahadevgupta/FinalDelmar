using FormsLoyalty.Helpers;
using FormsLoyalty.Interfaces;
using FormsLoyalty.Utils;
using FormsLoyalty.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;

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
