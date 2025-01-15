using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPS_BLAZOR.Module.Interfaces
{
    public interface IProductGroupFilter
    {
        public ProductGroup Group { get; set; }
    }
}
