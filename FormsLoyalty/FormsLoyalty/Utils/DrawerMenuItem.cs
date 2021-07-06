using FormsLoyalty.ConstantValues;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Utils
{
    public class DrawerMenuItem : BindableBase
    {
        public int Id { get; set; }
        public string Title { get; set; }

        private string _subTitle;
        public string SubTitle
        {
            get { return _subTitle; }
            set { SetProperty(ref _subTitle, value); }
        }
       
        public string Image { get; set; }
       
        public bool IsVisible { get; set; }
        public bool IsLoading { get; set; }
       
        public int ActivityType { get; set; }

    }

    public class SecondaryTextDrawerMenuItem : DrawerMenuItem
    {
        public string SecondaryText { get; set; }
    }
}
