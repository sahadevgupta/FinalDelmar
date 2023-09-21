using LSRetail.Omni.Domain.DataModel.Base.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Magazine
{
    public class MagazineModel :  IDisposable
    {
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
        [JsonProperty("Status")]
        public bool Status { get; set; }
        [JsonProperty("URL")]
        public string URL { get; set; }
        [JsonProperty("timeStamp")]
        public string timeStamp { get; set; }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
    }
}
