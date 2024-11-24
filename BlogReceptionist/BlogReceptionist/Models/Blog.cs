using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace BlogReceptionist.Models
{
    public class Blog
    {
        public long ID { get; set; }

        [Required]
        [MaxLength(205)]
        public string? Content { get; set; }
    }
}
