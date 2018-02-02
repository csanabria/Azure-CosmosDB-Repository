using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCosmosDBRepository.DTOs
{
    public class CommentDTO
    {
        [Required]
        public string Publisher { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
