using System;
using System.Collections.Generic;

namespace Frontend.Models
{
    public class DocumentViewModel
    {
        public DocumentViewModel()
        {
            Revisions = new List<DocumentRevisionViewModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<DocumentRevisionViewModel> Revisions { get; set; }
    }

    public class DocumentRevisionViewModel
    {
        public int Id { get; set; }
        public int? Revision { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
