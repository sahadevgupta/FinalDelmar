using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.ScanSend
{
    public class ScanSend 
    {
        [JsonIgnore]
        public int id { get; set; }
        public string pOrderNo { get; set; }
        public string pComment { get; set; }
        public string Description;
        public string status;
        public string ContactNo;
        public Uri url;
        public DateTime CreationDate;

        private string _ImagedBase64;

        public string ImagedBase64
        {
            get { return _ImagedBase64; }
            set { _ImagedBase64 = value; }
        }

        public string imageExtension;


        public ScanSend()
        {

        }

    }

}
