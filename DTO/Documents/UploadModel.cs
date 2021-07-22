using System;
using Microsoft.AspNetCore.Http;

namespace DTO.Documents
{
    public class UploadModel
    {
        public UploadModel()
        {
        }

        public string DocumentName   { get; set; }
        public string FileContent { get; set; }
    }
}
