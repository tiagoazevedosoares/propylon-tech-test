using System.Collections.Generic;
using System.Threading.Tasks;
using DTO.Documents;
using DTO.Users;

namespace Frontend.Services
{
    public interface IAPIService
    {
        Task<UserModelWithToken> Login(AuthenticateModel userModel);

        Task<int> Register(RegisterModel registerModel);

        Task<IEnumerable<DocumentModel>> GetDocuments(UserModelWithToken userModel);

        Task<int> UploadDocument(UploadModel uploadModel, UserModelWithToken userModel);

        Task<DownloadModel> DownloadDocument(DownloadModel downloadModel, UserModelWithToken userModel);
    }
}
