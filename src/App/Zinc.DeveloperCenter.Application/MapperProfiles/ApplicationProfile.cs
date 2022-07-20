using AutoMapper;
using Zinc.DeveloperCenter.Application.Queries.UXAppList.GetApplications;

namespace Zinc.DeveloperCenter.Application.MapperProfiles
{
    /// <summary>
    /// A <see cref="IMapper"/> for the <see cref="Application"/> aggregate root.
    /// </summary>
    public class ApplicationProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ApplicationProfile()
        {
            CreateMap<Domain.Model.Application, UXAppListGetApplicationsQueryModel>();
        }
    }
}
