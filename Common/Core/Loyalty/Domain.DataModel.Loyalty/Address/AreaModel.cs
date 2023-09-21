using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Address
{
    public class AreaModel
    {
        [JsonProperty("Area")]
        public string Area { get; set; }
        [JsonProperty("City")]
        public string City { get; set; }
        [JsonProperty("timestamp")]
        public string Time { get; set; }
    }

    public class CitiesModel
    {
        [JsonProperty("Country")]
        public string Country { get; set; }
        [JsonProperty("City")]
        public string City { get; set; }
        [JsonProperty("timestamp")]
        public string Time { get; set; }
    }
}
