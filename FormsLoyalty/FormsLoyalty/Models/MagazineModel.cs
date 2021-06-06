using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prism.Mvvm;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormsLoyalty.Models
{
    public class MagazineModel: BindableBase
    {
        public int id { get; set; }
        public string name { get; set; }

        [JsonProperty("short_description")]
        public string description { get; set; }
        [JsonProperty("post_content")]
        public string content { get; set; }
        public string image { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int author_id { get; set; }
        public int modifier_id { get; set; }
        public DateTime publish_date { get; set; }

        private string _imageSource;
        [JsonIgnore]
        [Ignore]
        public string ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        public string Url { get; set; }

    }
}
