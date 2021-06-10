using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.PopUpView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoreOptionsView : PopupPage
    {
        public event EventHandler MoreOptionClicked;
        public ObservableCollection<MoreOptionModel> MoreOptionsList { get; set; }

        public MoreOptionsView(List<MoreOptionModel> moreOptions, double height)
        {
            MoreOptionsList = new ObservableCollection<MoreOptionModel>(moreOptions);
            InitializeComponent();
            BindingContext = this;
          
            popUpView.Margin = new Thickness(0, height, 0, 0);
           
        }
        void BackGround_Tapped(System.Object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
        async void MoreOptions_Selected(System.Object sender, System.EventArgs e)
        {
            var view = (View)sender;
            view.Opacity = 0;
            await view.FadeTo(1, 250);
            MoreOptionClicked?.Invoke(sender, e);
            view.Opacity = 1;
           await PopupNavigation.Instance.PopAllAsync();
        }
    }
}