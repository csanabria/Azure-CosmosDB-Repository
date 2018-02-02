using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCosmosDBRepository.Entities
{
    public class Post
    {
        [JsonProperty(PropertyName  = "id")]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Abstract { get; set; }

        public DateTime PublishDate { get; set; }

        public string Content { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
