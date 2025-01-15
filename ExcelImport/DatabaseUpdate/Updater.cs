using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using ExcelImport.BusinessObjects;

namespace ExcelImport.DatabaseUpdate
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
        {
        }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            #region Version 2.0

            //if (this.CurrentDBVersion <= new Version(2, 0, 7863, 20740))
            //{
            //    List<IGrouping<Type, ImportDefinitionColumnMapping>> importDefinitionColumnMappings = this.ObjectSpace.GetObjects<ImportDefinitionColumnMapping>()
            //        .Where(a => !string.IsNullOrEmpty(a.PropertyCaption) && string.IsNullOrEmpty(a.PropertyName) && a.ImportDefinition != null && a.ImportDefinition.TargetObjectType != null).GroupBy(g => g.ImportDefinition.TargetObjectType).ToList();

            //    List<IGrouping<Type, ImportDefinitionCellMapping>> importDefinitionCellMappings = this.ObjectSpace.GetObjects<ImportDefinitionCellMapping>()
            //        .Where(b => !string.IsNullOrEmpty(b.PropertyCaption) && string.IsNullOrEmpty(b.PropertyName) && b.ImportDefinition != null && b.ImportDefinition.TargetObjectType != null).GroupBy(g => g.ImportDefinition.TargetObjectType).ToList();

            //    foreach (IGrouping<Type, ImportDefinitionColumnMapping> group in importDefinitionColumnMappings)
            //    {
            //        foreach (ImportDefinitionColumnMapping columnMapping in group)
            //        {
            //            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(columnMapping.ImportDefinition.TargetObjectType);
            //            if (typeInfo != null)
            //            {
            //                IMemberInfo member = typeInfo.Members.Where(a => DevExpress.ExpressApp.Utils.CaptionHelper.GetMemberCaption(a) == columnMapping.PropertyCaption).FirstOrDefault();
            //                if (member != null)
            //                {
            //                    columnMapping.PropertyName = member.Name;
            //                }
            //            }
            //        }
            //    }

            //    foreach (IGrouping<Type, ImportDefinitionCellMapping> group in importDefinitionCellMappings)
            //    {
            //        foreach (ImportDefinitionCellMapping cellMapping in group)
            //        {
            //            ITypeInfo typeinfo = XafTypesInfo.Instance.FindTypeInfo(cellMapping.ImportDefinition.TargetObjectType);
            //            if (typeinfo != null)
            //            {
            //                IMemberInfo member = typeinfo.Members.Where(a => DevExpress.ExpressApp.Utils.CaptionHelper.GetMemberCaption(a) == cellMapping.PropertyCaption).FirstOrDefault();
            //                if (member != null)
            //                {
            //                    cellMapping.PropertyName = member.Name;
            //                }
            //            }
            //        }
            //    }

            //    IList<ImportDefinition> importDefinitions = this.ObjectSpace.GetObjects<ImportDefinition>().Where(a => a.EmailToMapping != null).ToList();
            //    foreach (ImportDefinition importDefinition in importDefinitions)
            //    {
            //        if (importDefinition.EmailToMapping != null && importDefinition.TargetObjectType != null)
            //        {
            //            ITypeInfo typeinfo = XafTypesInfo.Instance.FindTypeInfo(importDefinition.TargetObjectType);
            //            if (typeinfo != null)
            //            {
            //                IMemberInfo member = typeinfo.Members.Where(a => DevExpress.ExpressApp.Utils.CaptionHelper.GetMemberCaption(a) == importDefinition.EmailToMapping.PropertyCaption).FirstOrDefault();
            //                if (member != null)
            //                    importDefinition.EmailToMapping.PropertyName = member.Name;
            //            }
            //        }
            //    }
            //}



            this.ObjectSpace.CommitChanges();

            #endregion
            //string name = "MyName";
            //DomainObject1 theObject = ObjectSpace.FindObject<DomainObject1>(CriteriaOperator.Parse("Name=?", name));
            //if(theObject == null) {
            //    theObject = ObjectSpace.CreateObject<DomainObject1>();
            //    theObject.Name = name;
            //}

            //ObjectSpace.CommitChanges(); //Uncomment this line to persist created object(s).
        }

        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
    }
}
