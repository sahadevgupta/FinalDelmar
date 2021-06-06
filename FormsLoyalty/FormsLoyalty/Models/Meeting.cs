using Newtonsoft.Json;
using Prism.Mvvm;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FormsLoyalty.Models
{
    
	/// <summary>   
	/// Represents custom data properties to add reminder.   
	/// </summary>   
	public class MedicineReminder : BindableBase
	{
        [PrimaryKey]
        public string ID { get; set; }
        public string EventName { get; set; }
		public string unit { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
        [Ignore]
		public Color color { get; set; }

        private bool _allDay;
        public bool AllDay
        {
            get { return _allDay; }
            set { SetProperty(ref _allDay, value); }
        }

        private int _noOfDays;
        public int NoOfDays
        {
            get { return _noOfDays; }
            set { SetProperty(ref _noOfDays, value); }
        }

        private decimal _doseQty;
        [Ignore]
        public decimal DoseQty
        {
            get { return _doseQty; }
            set { SetProperty(ref _doseQty, value); }
        }

        private bool _isReminder;
        public bool IsReminder
        {
            get { return _isReminder; }
            set { SetProperty(ref _isReminder, value); }
        }

        public int Frequency { get; set; }
        private string _specificDays;
        public string SpecificDays
        {
            get { return _specificDays; }
            set { SetProperty(ref _specificDays, value); }
        }
        private List<FrequencyTime> _frequencyTimes;
        [JsonIgnore]
        [Ignore]
        public List<FrequencyTime> FrequencyTimes
        {
            get { return _frequencyTimes; }
            set { SetProperty(ref _frequencyTimes, value); }
        }
    }

	public class FrequencyTime:BindableBase
    {
        [PrimaryKey]
        public string ID { get; set; }
        
        public string MedicineReminderId { get; set; } // medicineReminder primary key
        private decimal _qty = 1;
        public decimal Qty
        {
            get { return _qty; }
            set { SetProperty(ref _qty, value); }
        }
        private TimeSpan _time;
        public TimeSpan Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }
    }
}
