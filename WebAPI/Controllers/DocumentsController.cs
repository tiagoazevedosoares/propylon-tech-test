using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using DTO.Documents;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private IDocumentService _documentService;
        private IUserService _userService;

        public DocumentsController(
            IDocumentService _documentService,
            IUserService _userService)
        {
            this._userService = _userService;
            this._documentService = _documentService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                int.TryParse(User.Identity.Name, out int userId);

                var documents = _documentService.GetAll(userId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("upload")]
        public IActionResult Upload([FromBody] UploadModel model)
        {
            try
            {
                int.TryParse(User.Identity.Name, out int userId);
                var document = _documentService.GetByNameAndUserId(model.DocumentName, userId);

                if (document == null)
                {
                    document = new Document()
                    {
                        DocumentVersions = new List<DocumentVersion>() { new DocumentVersion()
                    {
                        CreateDateTime = DateTime.Now,
                        FileContent = Convert.FromBase64String(model.FileContent)
                    } },
                        Name = model.DocumentName,
                        UserId = userId
                    };

                    _documentService.Insert(document);
                }
                else
                {
                    document.DocumentVersions = new List<DocumentVersion>();
                    document.DocumentVersions.Add(new DocumentVersion()
                    {
                        CreateDateTime = DateTime.Now,
                        FileContent = Convert.FromBase64String(model.FileContent)
                    });

                    _documentService.Update(document);
                }

                return Ok(document.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("download")]
        public IActionResult Download(string documentName, int? revision = null)
        {
            try
            {
                int.TryParse(User.Identity.Name, out int userId);
                var document = _documentService.GetDocumentVersion(documentName, userId, revision);

                if (document != null)
                {
                    var download = new DownloadModel()
                    {
                        DocumentName = documentName,
                        FileContent = Convert.ToBase64String(document.FileContent),
                        Revision = revision
                    };

                    return Ok(download);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
