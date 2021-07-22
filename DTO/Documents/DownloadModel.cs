using System;
namespace DTO.Documents
{
    public class DownloadModel
    {
        public DownloadModel()
        {
        }

        public string DocumentName { get; set; }
        public int? Revision { get; set; }
        public string FileContent { get; set; }
    }
}
