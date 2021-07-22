using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using DTO.Documents;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services
{
    public interface IDocumentService
    {
        IEnumerable<DocumentModel> GetAll(int userId);
        int Insert(Document documentModel);
        int Update(Document documentModel);
        Document GetByNameAndUserId(string documentName, int userId);
        DocumentVersion GetDocumentVersion(string documentName, int userId, int? revision = null);
    }

    public class DocumentService : IDocumentService
    {
        private DataContext _context;

        public DocumentService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<DocumentModel> GetAll(int userId)
        {
            var documents = _context.Documents.Where(d => d.UserId == userId).Select(d => new DocumentModel() { Id = d.Id, Name = d.Name, Revisions = d.DocumentVersions.Select(dv => new DocumentRevision() { CreatedDateTime = dv.CreateDateTime, Id = dv.Id }) }).ToList();

            foreach (var document in documents)
            {
                var r = document.Revisions.Count();
                var i = 1;
                foreach (var rev in document.Revisions.OrderByDescending(d => d.CreatedDateTime))
                {
                    if (i != 1)
                        rev.Revision = r - i;
                    i++;
                }
            }

            return documents;
        }

        public int Insert(Document document)
        {
            if (string.IsNullOrWhiteSpace(document.Name))
                throw new AppException("Document Name cannot be empty");

            if (!document.DocumentVersions.Any())
                throw new AppException("Document does not have versions");

            _context.Documents.Add(document);
            _context.SaveChanges();

            return document.Id;
        }

        public int Update(Document document)
        {
            if (string.IsNullOrWhiteSpace(document.Name))
                throw new AppException("Document Name cannot be empty");

            if (!document.DocumentVersions.Any())
                throw new AppException("Document does not have versions");

            _context.Attach(document);
            _context.SaveChanges();

            return document.Id;
        }

        public Document GetByNameAndUserId(string documentName, int userId)
        {
            return _context.Documents.FirstOrDefault(d => d.UserId == userId && d.Name == documentName);
        }

        public DocumentVersion GetDocumentVersion(string documentName, int userId, int? revision = null)
        {
            var document = _context.Documents.Include(d => d.DocumentVersions).FirstOrDefault(d => d.UserId == userId && d.Name == documentName);

            if (document != null)
            {
                if (revision.HasValue)
                {
                    return document.DocumentVersions.OrderByDescending(d => d.CreateDateTime).Skip(revision.Value).Take(1).FirstOrDefault();
                }
                else
                {
                    return document.DocumentVersions.OrderByDescending(d => d.CreateDateTime).FirstOrDefault();
                }
            }

            throw new AppException("Document not found.");
        }
    }
}