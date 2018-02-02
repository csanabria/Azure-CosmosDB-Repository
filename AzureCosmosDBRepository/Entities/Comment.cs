using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCosmosDBRepository.Entities
{
    public class Comment
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Publisher { get; set; }

        public DateTime PublishDate { get; set; }

        public string Content { get; set; }
    }
}
