using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Base;
using System.ComponentModel;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Items
{
    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Loy/2017")]
    public class ProductGroup : Entity, IDisposable,INotifyPropertyChanged
    {
        public ProductGroup(string id) : base(id)
        {
            Description = string.Empty;
            ArabicDescription = string.Empty;
            ItemCategoryId = string.Empty;
            Items = new List<LoyItem>();
            Images = new List<ImageView>();
        }

        public ProductGroup() : this(string.Empty)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Items != null)
                    Items.Clear();
                if (Images != null)
                    Images.Clear();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string ItemCategoryId { get; set; }

        [DataMember]
        public string ArabicDescription { get; set; }



        private List<LoyItem> _items;
        [DataMember]
        public List<LoyItem> Items
        {
            get { return _items; }
            set 
            {
                _items = value;
                OnPropertyChanged("Image");
            }
        }
        [DataMember]
        public List<ImageView> Images { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
 