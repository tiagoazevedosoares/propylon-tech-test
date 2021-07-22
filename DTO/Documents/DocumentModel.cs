using System;
using System.Collections.Generic;

namespace DTO.Documents
{
    public class DocumentModel
    {
        public DocumentModel()
        {
            Revisions = new List<DocumentRevision>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<DocumentRevision> Revisions { get; set; }
    }

    public class DocumentRevision
    {
        public int Id { get; set; }
        public int? Revision { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}