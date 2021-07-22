using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using DTO.Documents;
using DTO.Users;
using Frontend.Extensions;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IAPIService apiService;
        private readonly IMapper mapper;

        public DocumentsController(IAPIService apiService, IMapper mapper)
        {
            this.apiService = apiService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.Session.Get<UserModelWithToken>("UserWithToken");
            if (user == null)
            {
                return RedirectToAction("login", "account");
            }

            var documents = await apiService.GetDocuments(user);
            var documentsvm = mapper.Map<IEnumerable<DocumentViewModel>>(documents);

            return View(documentsvm);
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View(new UploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadViewModel uploadViewModel)
        {
            var user = HttpContext.Session.Get<UserModelWithToken>("UserWithToken");
            if (user == null)
            {
                return RedirectToAction("login", "account");
            }

            using (var ms = new MemoryStream())
            {
                uploadViewModel.File.CopyTo(ms);
                var fileBytes = ms.ToArray();

                await apiService.UploadDocument(new UploadModel()
                {
                    DocumentName = uploadViewModel.Name,
                    FileContent = Convert.ToBase64String(fileBytes)
                }, user);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string id, int? revision = null)
        {
            var user = HttpContext.Session.Get<UserModelWithToken>("UserWithToken");
            if (user == null)
            {
                return RedirectToAction("login", "account");
            }

            var document = await apiService.DownloadDocument(new DownloadModel()
            {
                DocumentName = id,
                Revision = revision
            },
            user);

            return File(Convert.FromBase64String(document.FileContent), "application/octet-stream", id); 
        }
    }
}
