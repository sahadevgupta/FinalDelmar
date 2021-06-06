using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Processors;
using Xamarin.Forms;

namespace FormsLoyalty.Models
{

    public class GMFileInfo : BindableBase
    {
        private ImageSource _Data;
        public ImageSource Data
        {
            get { return _Data; }
            set { SetProperty(ref _Data, value); }
        }
        public byte[] bytes { get; set; }
        public string extension { get; set; }
        public int id { get; set; }

    }


}
