    using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Frontend.Models
{
    public class UploadViewModel
    {
        public UploadViewModel()
        {
        }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; set; }
    }
}
