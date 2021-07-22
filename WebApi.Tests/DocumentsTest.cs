using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Tests
{
    [TestClass]
    public class DocumentsTests
    {
        private IUserService userService;
        private IDocumentService documentService;

        [TestInitialize]
        public void Setup()
        {
            byte[] passwordSalt;
            byte[] passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("123"));
            }

            var users = new List<User>
            {
                new User { FirstName = "Tiago", LastName = "Soares", Id = 1, Username = "tsoares", PasswordHash = passwordHash, PasswordSalt = passwordSalt },
                new User { FirstName = "Tiago", LastName = "Soares", Id = 2, Username = "tsoares2", PasswordHash = passwordHash, PasswordSalt = passwordSalt }
            }.AsQueryable();

            var mockUsersSet = new Mock<DbSet<User>>();
            mockUsersSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUsersSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUsersSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUsersSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var docs = new List<Document>
            {
                new Document { Id = 1, Name = "test.pdf", UserId = 1, DocumentVersions = new List<DocumentVersion>() {
                    new DocumentVersion(){ CreateDateTime = System.DateTime.Now, DocumentId = 1, Id = 1 },
                    new DocumentVersion(){ CreateDateTime = System.DateTime.Now, DocumentId = 1, Id = 2 }
                } },
                new Document { Id = 2, Name = "test2.pdf", UserId = 2, DocumentVersions = new List<DocumentVersion>() {
                    new DocumentVersion(){ CreateDateTime = System.DateTime.Now, DocumentId = 2, Id = 3 },
                    new DocumentVersion(){ CreateDateTime = System.DateTime.Now, DocumentId = 2, Id = 4 }
                } }
            }.AsQueryable();

            var mockDocsSet = new Mock<DbSet<Document>>();
            mockDocsSet.As<IQueryable<Document>>().Setup(m => m.Provider).Returns(docs.Provider);
            mockDocsSet.As<IQueryable<Document>>().Setup(m => m.Expression).Returns(docs.Expression);
            mockDocsSet.As<IQueryable<Document>>().Setup(m => m.ElementType).Returns(docs.ElementType);
            mockDocsSet.As<IQueryable<Document>>().Setup(m => m.GetEnumerator()).Returns(docs.GetEnumerator());

            var mockContext = new Mock<DataContext>();
            mockContext.Setup(c => c.Users).Returns(mockUsersSet.Object);
            mockContext.Setup(c => c.Documents).Returns(mockDocsSet.Object);

            userService = new UserService(mockContext.Object);
            documentService = new DocumentService(mockContext.Object);
        }

        [TestMethod]
        public void GetAll_Success()
        {
            var docs = documentService.GetAll(1);

            Assert.IsTrue(docs.Count() == 1);
        }

        [TestMethod]
        public void GetByDocumentNameAndUserId_Success()
        {
            var doc = documentService.GetByNameAndUserId("test.pdf", 1);

            Assert.IsNotNull(doc);
        }

        [TestMethod]
        public void GetByDocumentNameAndUserId_Error()
        {
            var doc = documentService.GetByNameAndUserId("test.pdf", 2);

            Assert.IsNull(doc);
        }

        [TestMethod]
        public void Insert_Success()
        {
            var doc = new Document()
            {
                Id = 3,
                Name = "test2.pdf",
                UserId = 1,
                DocumentVersions = new List<DocumentVersion>()
                {
                    new DocumentVersion(){ CreateDateTime = System.DateTime.Now, FileContent = null }
                }
            };

            var id = documentService.Insert(doc);

            Assert.IsTrue(id == 3);
        }

        [TestMethod]
        public void Insert_DocName_Blank()
        {
            Assert.ThrowsException<AppException>(() => documentService.Insert(new Document()
            {
                Id = 3,
                Name = string.Empty,
                UserId = 1,
                DocumentVersions = new List<DocumentVersion>()
                {
                    new DocumentVersion(){ CreateDateTime = System.DateTime.Now, FileContent = null }
                }
            }), "Document Name cannot be empty");
        }

        [TestMethod]
        public void Insert_NoVersions()
        {
            Assert.ThrowsException<AppException>(() => documentService.Insert(new Document()
            {
                Id = 3,
                Name = "name.pdf",
                UserId = 1,
                DocumentVersions = new List<DocumentVersion>()
            }), "Document does not have versions");
        }

        [TestMethod]
        public void Update_Success()
        {
            var doc = new Document()
            {
                Id = 1,
                Name = "test.pdf",
                UserId = 1,
                DocumentVersions = new List<DocumentVersion>()
                {
                    new DocumentVersion(){ CreateDateTime = System.DateTime.Now, FileContent = null }
                }
            };

            var id = documentService.Update(doc);

            Assert.IsTrue(id == 1);
        }

        [TestMethod]
        public void Update_DocName_Blank()
        {
            Assert.ThrowsException<AppException>(() => documentService.Update(new Document()
            {
                Id = 1,
                Name = string.Empty,
                UserId = 1,
                DocumentVersions = new List<DocumentVersion>()
                {
                    new DocumentVersion(){ CreateDateTime = System.DateTime.Now, FileContent = null }
                }
            }), "Document Name cannot be empty");
        }

        [TestMethod]
        public void Update_NoVersions()
        {
            Assert.ThrowsException<AppException>(() => documentService.Update(new Document()
            {
                Id = 1,
                Name = "name.pdf",
                UserId = 1,
                DocumentVersions = new List<DocumentVersion>()
            }), "Document does not have versions");
        }
    }
}
