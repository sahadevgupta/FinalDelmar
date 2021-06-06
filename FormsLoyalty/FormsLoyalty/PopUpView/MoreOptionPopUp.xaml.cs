using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FormsLoyalty.ViewModels;
using Rg.Plugins.Popup.Services;

namespace FormsLoyalty.PopUpView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoreOptionPopUp : PopupPage
    {
        private ViewModelBase vm;
        public event EventHandler MoreOptionClicked;
        public double headerHeigth { get; set; }
        public double FinalGridHeight { get; set; }
        public ObservableCollection<MoreOptionModel> MoreOptionsList { get; set; }
        public MoreOptionPopUp(List<MoreOptionModel> moreOptions,double topMargin)
        {
            headerHeigth = Height;
            FinalGridHeight = headerHeigth + 100;
            MoreOptionsList = new ObservableCollection<MoreOptionModel>(moreOptions);
            InitializeComponent();
            BindingContext = this;
            popUpView.Margin = new Thickness(10, topMargin, 10, 0);
           // BindableLayout.SetItemsSource(moreStack, moreOptions);
            //vm = new ViewModelBase();
        }
        void BackGround_Tapped(System.Object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PopAllAsync();
        }
        void MoreOptions_Selected(System.Object sender, System.EventArgs e)
        {
            MoreOptionClicked?.Invoke(sender, e);
            
            PopupNavigation.Instance.PopAllAsync();
        }
    }
    public class MoreOptionModel
    {
        public string OptionName { get; set; }
        public Page targetType { get; set; }
    }
}