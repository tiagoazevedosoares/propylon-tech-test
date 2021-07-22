using System;
using System.Collections.Generic;

namespace WebApi.Entities
{
    public class Document
    {
        public Document()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<DocumentVersion> DocumentVersions { get; set; }
    }
}
