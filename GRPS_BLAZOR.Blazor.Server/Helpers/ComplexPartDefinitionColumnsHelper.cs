using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using ExcelImport.BusinessObjects;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using System.Data;

namespace GRPS_BLAZOR.Blazor.Server.Helpers
{
    public class ComplexPartDefinitionColumnsHelper
    {
        public List<ImportDefinitionColumnMapping> GetComplexPartColumnMapping(Session session)
        {
            List<ImportDefinitionColumnMapping> importDefinitionColumnMappings = new List<ImportDefinitionColumnMapping>();
            //TODO: Create the column mappings 18 columns
            int order = 1;


            // Definitions for Part
            ImportDefinitionColumnMapping column5 = new ImportDefinitionColumnMapping(session)
            {
                OrderNumber = order,
                ColumnNumber = 5,
                PropertyName = nameof(Part.Name),
                PropertyClass = nameof(Part)
            };
            importDefinitionColumnMappings.Add(column5);
            order++;

            ImportDefinitionColumnMapping column10 = new ImportDefinitionColumnMapping(session)
            {
                OrderNumber = order,
                ColumnNumber = 10,
                PropertyName = nameof(Part.Code),
                PropertyClass = nameof(Part)
            };
            importDefinitionColumnMappings.Add(column10);
            order++;

            ImportDefinitionColumnMapping column2 = new ImportDefinitionColumnMapping(session)
            {
                OrderNumber = order,
                ColumnNumber = 2,
                PropertyName = nameof(Part.PartGroup),
                PropertyClass = nameof(Part),
                PropertyType = typeof(PartGroup),
                FindByField = nameof(PartGroup.Name),
                WhenFindingLookupValue = FindReferenceMappingsMode.CreateIfNotFound
            };
            importDefinitionColumnMappings.Add(column2);
            order++;

            ImportDefinitionColumnMapping column4 = new ImportDefinitionColumnMapping(session)
            {
                OrderNumber = order,
                ColumnNumber = 4,
                PropertyName = nameof(Part.PartType),
                PropertyClass = nameof(Part),
                PropertyType = typeof(EnumInstance),
                FindByField = nameof(EnumInstance.Name),
                WhenFindingLookupValue = FindReferenceMappingsMode.CreateIfNotFound
            };
            importDefinitionColumnMappings.Add(column4);
            order++;

            ImportDefinitionColumnMapping column6 = new ImportDefinitionColumnMapping(session)
            {
                OrderNumber = order,
                ColumnNumber = 6,
                PropertyName = nameof(Part.Description),
                PropertyClass = nameof(Part)
            };
            importDefinitionColumnMappings.Add(column6);
            order++;

            // Definitions for associated PackWeight
            //ImportDefinitionColumnMapping column3 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 3,
            //    PropertyName = nameof(PackWeight.Type),
            //    PropertyClass = nameof(PackWeight)
            //};
            //importDefinitionColumnMappings.Add(column3);
            //order++;

            //ImportDefinitionColumnMapping column7 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 7,
            //    PropertyName = nameof(PackWeight.MaterialCategory),
            //    PropertyClass = nameof(PackWeight)
            //};
            //importDefinitionColumnMappings.Add(column7);
            //order++;

            //ImportDefinitionColumnMapping column8 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 8,
            //    PropertyName = nameof(PackWeight.Colour),
            //    PropertyClass = nameof(PackWeight)
            //};
            //importDefinitionColumnMappings.Add(column8);
            //order++;

            //ImportDefinitionColumnMapping column9 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 9,
            //    PropertyName = nameof(PackWeight.ForestryCertified),
            //    PropertyClass = nameof(PackWeight)
            //};
            //importDefinitionColumnMappings.Add(column9);
            //order++;

            //ImportDefinitionColumnMapping column11 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 11,
            //    PropertyName = nameof(PackWeight.Quantity),
            //    PropertyClass = nameof(PackWeight)
            //};
            //importDefinitionColumnMappings.Add(column11);
            //order++;

            //ImportDefinitionColumnMapping column12 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 12,
            //    PropertyName = nameof(PackWeight.UnitOfMeas),
            //    PropertyClass = nameof(PackWeight)
            //};
            //importDefinitionColumnMappings.Add(column12);
            //order++;

            //ImportDefinitionColumnMapping column14 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 14,
            //    PropertyName = nameof(PackWeight.AvgRecCon),
            //    PropertyClass = nameof(PackWeight)
            //};
            //importDefinitionColumnMappings.Add(column14);
            //order++;


            //// Definitions for associated SRAttributes
            //ImportDefinitionColumnMapping column15 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 15,
            //    PropertyName = nameof(SRAttribute.ReportType),
            //    PropertyClass = nameof(SRAttribute)
            //};
            //importDefinitionColumnMappings.Add(column15);
            //order++;

            //ImportDefinitionColumnMapping column16 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 16,
            //    PropertyName = nameof(SRAttribute.AttributeName),
            //    PropertyClass = nameof(SRAttribute)
            //};
            //importDefinitionColumnMappings.Add(column16);
            //order++;

            //ImportDefinitionColumnMapping column17 = new ImportDefinitionColumnMapping(session)
            //{
            //    OrderNumber = order,
            //    ColumnNumber = 17,
            //    PropertyName = nameof(SRAttribute.AttributeNum),
            //    PropertyClass = nameof(SRAttribute)
            //};
            //importDefinitionColumnMappings.Add(column17);


            return importDefinitionColumnMappings;
        }

        public Dictionary<string, IMemberInfo> GetColumnMappingMembers(Session session)
        {
            Dictionary<string, IMemberInfo> result = new Dictionary<string, IMemberInfo>()
            {
                {nameof(Part.Name), XafTypesInfo.Instance.FindTypeInfo(typeof(Part)).FindMember(nameof(Part.Name)) },
                {nameof(Part.Code), XafTypesInfo.Instance.FindTypeInfo(typeof(Part)).FindMember(nameof(Part.Code)) },
                {nameof(Part.PartGroup), XafTypesInfo.Instance.FindTypeInfo(typeof(Part)).FindMember(nameof(Part.PartGroup)) },
                {nameof(Part.PartType), XafTypesInfo.Instance.FindTypeInfo(typeof(Part)).FindMember(nameof(Part.PartType)) },
                {nameof(Part.Description), XafTypesInfo.Instance.FindTypeInfo(typeof(Part)).FindMember(nameof(Part.Description)) },
                //{nameof(PackWeight.Type), XafTypesInfo.Instance.FindTypeInfo(typeof(PackWeight)).FindMember(nameof(PackWeight.Type)) },
                //{nameof(PackWeight.MaterialCategory), XafTypesInfo.Instance.FindTypeInfo(typeof(PackWeight)).FindMember(nameof(PackWeight.MaterialCategory)) },
                //{nameof(PackWeight.Colour), XafTypesInfo.Instance.FindTypeInfo(typeof(PackWeight)).FindMember(nameof(PackWeight.Colour)) },
                //{nameof(PackWeight.ForestryCertified), XafTypesInfo.Instance.FindTypeInfo(typeof(PackWeight)).FindMember(nameof(PackWeight.ForestryCertified)) },
                //{nameof(PackWeight.Quantity), XafTypesInfo.Instance.FindTypeInfo(typeof(PackWeight)).FindMember(nameof(PackWeight.Quantity)) },
                //{nameof(PackWeight.UnitOfMeas), XafTypesInfo.Instance.FindTypeInfo(typeof(PackWeight)).FindMember(nameof(PackWeight.UnitOfMeas)) },
                //{nameof(PackWeight.AvgRecCon), XafTypesInfo.Instance.FindTypeInfo(typeof(PackWeight)).FindMember(nameof(PackWeight.AvgRecCon)) },
                //{nameof(SRAttribute.ReportType), XafTypesInfo.Instance.FindTypeInfo(typeof(SRAttribute)).FindMember(nameof(SRAttribute.ReportType)) },
                //{nameof(SRAttribute.AttributeName), XafTypesInfo.Instance.FindTypeInfo(typeof(SRAttribute)).FindMember(nameof(SRAttribute.AttributeName)) },
                //{nameof(SRAttribute.AttributeNum), XafTypesInfo.Instance.FindTypeInfo(typeof(SRAttribute)).FindMember(nameof(SRAttribute.AttributeNum)) }
            };

            return result;
        }
    }
}
