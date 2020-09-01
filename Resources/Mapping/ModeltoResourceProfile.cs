using AutoMapper;
using CoreAPi.Models;
using Mywebsite.Models;
using Mywebsite.Resources.Request;
using Mywebsite.Resources.Requests;
using Mywebsite.Resources.Response;
using Mywebsite.Resources.Responses;

namespace Mywebsite.Mapping
{
    public class ModeltoResourceProfile : Profile
    {
        public ModeltoResourceProfile()
        {
            #region member
            CreateMap<MemberModel,MemberLoginResource>();
            CreateMap<MemberModel,MemberUpdateResource>();
            CreateMap<MemberRegisterResource,MemberModel>();
            CreateMap<MemberChangePasswordResource,MemberModel>();
            CreateMap<MemberUpdateResource,MemberModel>();
            #endregion

            #region news
            CreateMap<NewsModel,NewsViewResource>();
            CreateMap<NewsInsertResource,NewsModel>();
            CreateMap<NewsUpdataResource,NewsModel>();
            #endregion
            
            #region FileUpload
            CreateMap<MemberModel, GetMemberResource>();
            CreateMap<FileUploadModel, GetFileUploadResource>();
            #endregion

            #region LearnOnline
            CreateMap<LearnOnlineModel, GetLearnOnlineResource>();
            CreateMap<CreateLearnOnlineResource, LearnOnlineModel>();
            #endregion

            #region Course
            CreateMap<CourseCreateResources, CourseModel>();
            CreateMap<CourseResources, CourseModel>();
            CreateMap<CourseResources, CourseTypeModel>();
            #endregion
        }
    }
}