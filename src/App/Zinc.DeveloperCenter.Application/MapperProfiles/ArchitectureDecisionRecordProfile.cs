using AutoMapper;
using Zinc.DeveloperCenter.Application.Queries.UXAppList.GetArchitectureDecisionRecords;
using Zinc.DeveloperCenter.Domain.Model;

namespace Zinc.DeveloperCenter.Application.MapperProfiles
{
    /// <summary>
    /// A <see cref="IMapper"/> for the <see cref="ArchitectureDecisionRecord"/> aggregate root.
    /// </summary>
    public class ArchitectureDecisionRecordProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ArchitectureDecisionRecordProfile()
        {
            CreateMap<ArchitectureDecisionRecord, UXAppListGetArchitectureDecisionRecordsQueryModel>();
        }
    }
}
