using System;
namespace WebApi.Entities
{
    public class DocumentVersion
    {
        public DocumentVersion()
        {
        }

        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public byte[] FileContent { get; set; }
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
