using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FormsLoyalty.Models
{
    public class TourCarouselContent : BindableBase
    {
        private bool _SkipButtonIsVisible;
        public bool SkipButtonIsVisible
        {
            get { return _SkipButtonIsVisible; }
            set { SetProperty(ref _SkipButtonIsVisible, value); }
        }
       
        public ICommand SkipButtonCommand { get; set; }

        public string MainText { get; set; }

        public bool ImageIsVisible { get; set; }
        public ImageSource ImageSource { get; set; }

        //public string MainButtonText { get; set; }
        //public ICommand MainButtonCommand { get; set; }
    }
}
