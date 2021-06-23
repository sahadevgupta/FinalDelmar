using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.PopUpView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoreOptionsPopUpView : Popup
    {
        public ObservableCollection<MoreOptionModel> MoreOptionsList { get; set; }
        public MoreOptionsPopUpView(List<MoreOptionModel> moreOptions)
        {
            MoreOptionsList = new ObservableCollection<MoreOptionModel>(moreOptions);
            InitializeComponent();
            BindingContext = this;
        }
        void MoreOptions_Selected(System.Object sender, System.EventArgs e)
        {
            //MoreOptionClicked?.Invoke(sender, e);

            //PopupNavigation.Instance.PopAllAsync();
        }
    }
}