using FormsLoyalty.Helpers;
using FormsLoyalty.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FormsLoyalty.ViewModels
{
    public class DemonstrationPageViewModel : ViewModelBase
    {
        private ObservableCollection<TourCarouselContent> _demoContents;
        public ObservableCollection<TourCarouselContent> DemoContents
        {
            get { return _demoContents; }
            set { SetProperty(ref _demoContents, value); }
        }

        private int _carouselPostion;
        public int CarouselPosition
        {
            get { return _carouselPostion; }
            set { SetProperty(ref _carouselPostion, value); }
        }

        private bool _isFromHelp;
        public bool IsFromHelp
        {
            get { return _isFromHelp; }
            set { SetProperty(ref _isFromHelp, value); }
        }

        public DelegateCommand SkipCommand { get; set; }
        public DemonstrationPageViewModel(INavigationService navigationService) : base(navigationService)
        {
           PopulateCarousel();
            SkipCommand = new DelegateCommand(async () => await GotoMainPage());
        }
        internal async void SlideToNextPage(string text)
        {
            if (text.ToLower().Contains("Next".ToLower()))
            {
                CarouselPosition = (CarouselPosition + 1) % DemoContents.Count;
            }
            else
                await GotoMainPage();

        }

        async Task GotoMainPage()
        {
            if (IsFromHelp)
            {
                await NavigationService.GoBackAsync();
            }
            else
            await NavigationService.NavigateAsync("app:///MainTabbedPage?selectedTab=MainPage");
        }

        private void PopulateCarousel()
        {
            DemoContents = new ObservableCollection<TourCarouselContent>
            {
                 new TourCarouselContent
                 {
                     ImageSource = MobileAppImageResourceHelper.GetImageSource("Demo-Slide2.png"),
                     SkipButtonIsVisible = true,
                     MainText=AppResources.txtDemo1
                 },
                 new TourCarouselContent
                 {
                     ImageSource = MobileAppImageResourceHelper.GetImageSource("Demo-Slide1.png"),
                     SkipButtonIsVisible = true,
                     MainText=AppResources.txtDemo2
                 },
                 new TourCarouselContent
                 {
                     ImageSource = MobileAppImageResourceHelper.GetImageSource("Demo-Slide3.png"),
                     SkipButtonIsVisible = false,
                     MainText=AppResources.txtDemo3
                 },
                 //new TourCarouselContent
                 //{
                 //    ImageSource = MobileAppImageResourceHelper.GetImageSource("Ecommerce.png"),
                 //    SkipButtonIsVisible = false,
                 //    //MainText="Here, You can manage your shopping cart"
                 //}
            };
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            IsFromHelp = parameters.GetValue<bool>("FromHelp");
        }
    }
}
