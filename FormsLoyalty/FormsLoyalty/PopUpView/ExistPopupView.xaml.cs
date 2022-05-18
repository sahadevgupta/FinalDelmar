using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FormsLoyalty.PopUpView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExistPopupView : PopupPage
	{
		
		public event EventHandler ProceedClicked;
		public ExistPopupView ()
		{
			InitializeComponent ();
		}

		void ClosePopup()
        {
			Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
		}
        private void OnCancelledClicked(object sender, EventArgs e)
        {
			ClosePopup();
		}

        private void OnProceedClicked(object sender, EventArgs e)
        {
			ProceedClicked?.Invoke(this, e);
			ClosePopup();

		}
    }
}