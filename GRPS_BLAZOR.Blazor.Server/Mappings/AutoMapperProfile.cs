using AutoMapper;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using TestSideTrees.Blazor.Server.POCOs;

namespace GRPS_BLAZOR.Blazor.Server.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductGroup, ProductGroupTreeItem>();
            CreateMap<Supplier, SupplierTreeItem>();
            CreateMap<PartGroup, PartGroupTreeItem>();

            CreateMap<EnumInstance, TreeItem>();

            CreateMap<EnumDomain, TreeItem>()
                .ForMember(dest => dest.ChildrenCollection, opts => opts.MapFrom(src => src.Subdomains));
        }
    }
}
