using FormsLoyalty.Helpers;
using FormsLoyalty.Repos;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsLoyalty.ViewModels
{
    public class TermsConditionPageViewModel : ViewModelBase
    {
        private string _terms;
        public string Terms
        {
            get { return _terms; }
            set { SetProperty(ref _terms, value); }
        }

        private HtmlWebViewSource _source;
        public HtmlWebViewSource HtmlSource
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        IScanSendRepo _termsRepo;
        public TermsConditionPageViewModel(INavigationService navigationService, IScanSendRepo termRepo ) :base(navigationService)
        {
            _termsRepo = termRepo;
            GetTermsData();
        }

        private void GetTermsData()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                IsPageEnabled = true;
                try
                {

                    if (string.IsNullOrEmpty(Settings.TermsConditions))
                    {
                        var termsConditions = await _termsRepo.GetTermsCondition();
                        Terms = termsConditions.Trim('\r', '\n');
                        Settings.TermsConditions = Terms;
                    }


                    HtmlSource = new HtmlWebViewSource();
                    HtmlSource.Html = Settings.TermsConditions;


                }
                catch (Exception)
                {


                }
                finally
                {
                    IsPageEnabled = false;
                }
            });
        }
    }
}
