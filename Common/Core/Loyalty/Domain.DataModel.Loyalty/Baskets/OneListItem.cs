﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using System.ComponentModel;
using Newtonsoft.Json;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Baskets
{
    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Loy/2017")]
    public class OneListItem : Entity, IDisposable,INotifyPropertyChanged
    {
        public OneListItem(string id) : base(id)
        {
            OneListId = string.Empty;
            ItemId = string.Empty;
            VariantId = string.Empty;
            UnitOfMeasureId = string.Empty;
            BarcodeId = string.Empty;
            CreateDate = DateTime.Now;
            Detail = string.Empty;
            Quantity = 0M;
            DisplayOrderId = 1;
            NetPrice = 0M;
            Price = 0M;
            NetAmount = 0M;
            TaxAmount = 0M;
            DiscountAmount = 0M;
            DiscountPercent = 0M;
            OnelistItemDiscounts = new List<OneListItemDiscount>();
            VariantRegistration = new VariantRegistration();
        }

        public OneListItem() : this(string.Empty)
        {
        }

        public OneListItem(LoyItem item, decimal qty) : this("")
        {
            ItemId = item.Id;
            ItemDescription = item.Description;
            UnitOfMeasureId = item.SelectedUnitOfMeasure?.Id;
            Image = item.DefaultImage;
            Quantity = qty;
            Price = item.AmtFromVariantsAndUOM(item.SelectedVariant?.Id, item.SelectedUnitOfMeasure?.Id);
            Detail = item.Details;
            VariantRegistration = item.SelectedVariant;
            if (item.SelectedVariant != null)
            {
                VariantId = item.SelectedVariant.Id;
                VariantDescription = item.SelectedVariant.ToString();
            }
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
                if (OnelistItemDiscounts != null)
                    OnelistItemDiscounts.Clear();
            }
        }

        [DataMember(IsRequired = true)]
        public string ItemId { get; set; }
        [DataMember]
        public string Detail { get; set; }
        [DataMember]
        public string ItemDescription { get; set; }
        [DataMember]
        public string UnitOfMeasureId { get; set; }
        [DataMember]
        public string UnitOfMeasureDescription { get; set; }
        [DataMember]
        public string VariantId { get; set; }
        [DataMember]
        public string VariantDescription { get; set; }

        
        private decimal _quantity;
        [DataMember(IsRequired = true)]
        public decimal Quantity
        {
            get { return _quantity; }
            set 
            { 
                _quantity = value;

                OnPropertyChanged("Quantity");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public string BarcodeId { get; set; }
        [DataMember]
        public int DisplayOrderId { get; set; }

        [DataMember]
        public decimal NetPrice { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public decimal NetAmount { get; set; }

        private decimal _amount;
        [DataMember]
        public decimal Amount
        {
            get { return _amount; }
            set 
            { 
                _amount = value;
                OnPropertyChanged("Amount");
            }
        }

        private string _newPrice;
        [JsonIgnore]
        public string NewPrice
        {
            get { return _newPrice; }
            set
            {
                _newPrice = value;
                OnPropertyChanged("NewPrice");
            }
        }

        [DataMember]
        public decimal TaxAmount { get; set; }
        [DataMember]
        public decimal DiscountAmount { get; set; }
        [DataMember]
        public decimal DiscountPercent { get; set; }
        [DataMember]
        public List<OneListItemDiscount> OnelistItemDiscounts { get; set; } // decimal got truncated
        [DataMember]
        public ImageView Image { get; set; }
        [DataMember]
        public VariantRegistration VariantRegistration { get; set; }

        public string OneListId { get; set; }

        public decimal GetDiscount()
        {
            decimal amt = 0M;
            foreach (OneListItemDiscount item in OnelistItemDiscounts)
            {
                amt += item.DiscountAmount;
            }
            return amt;
        }

        public bool HaveTheSameItemAndVariant(OneListItem itemToCompare)
        {
            if (itemToCompare == null)
                return false;

            // Compare item
            if (ItemId != itemToCompare.ItemId)
                return false;

            // Compare variants
            if (VariantId != itemToCompare.VariantId)
                return false;

            // Compare UOMs
            if (UnitOfMeasureId != itemToCompare.UnitOfMeasureId)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public virtual object Clone()
        {
            // Shallow copy here should be enough (just keep the references to item, variant, uom)
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format(@"Id:{0}  Qty:{1}  Item:{2}  CreateDate:{3}  Variant:{4}  Uom:{5}  BarcodeId:{6}",
                 Id, Quantity, ItemId, CreateDate, VariantId, UnitOfMeasureId, BarcodeId);
        }
    }

    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Loy/2017")]
    public class OneListPublishedOffer : Entity, IDisposable
    {
        public OneListPublishedOffer() : this(string.Empty)
        {
        }

        public OneListPublishedOffer(string id) : base(id)
        {
            OneListId = "";
            CreateDate = DateTime.Now;
            DisplayOrderId = 1;
            Type = OfferDiscountType.Unknown;
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
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DateTime CreateDate { get; set; }
        [DataMember]
        public int DisplayOrderId { get; set; }

        //not a data member
        public string OneListId { get; set; }
        
        [DataMember]
        public OfferDiscountType Type { get; set; }
    }
}
