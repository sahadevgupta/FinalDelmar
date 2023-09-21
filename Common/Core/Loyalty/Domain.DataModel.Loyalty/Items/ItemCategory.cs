using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Base;
using System.ComponentModel;
using Newtonsoft.Json;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Items
{
    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Loy/2017")]
    public class ItemCategory : Entity, IDisposable,INotifyPropertyChanged
    {
        public ItemCategory(string id) : base(id)
        {
            Id = id;
            Description = string.Empty;
            ArabicDescription = string.Empty;
            ProductGroups = new List<ProductGroup>();
            Images = new List<ImageView>();
        }

        public ItemCategory() : this(string.Empty)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string ArabicDescription { get; set; }


        [DataMember]
        public List<ProductGroup> ProductGroups { get; set; }
        

        private List<ImageView> _images;
        [DataMember]
        public List<ImageView> Images
        {
            get { return _images; }
            set { _images = value; }
        }

        private bool _isSelected;
        [JsonIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }



    }
}
 