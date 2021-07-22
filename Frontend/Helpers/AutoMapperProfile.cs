using AutoMapper;
using DTO.Documents;
using DTO.Users;
using Frontend.Models;

namespace Frontend.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DocumentModel, DocumentViewModel>();
            CreateMap<DocumentRevision, DocumentRevisionViewModel>();
            CreateMap<UserLoginViewModel, AuthenticateModel>();
            CreateMap<UserRegistrationViewModel, RegisterModel>();
        }
    }
}