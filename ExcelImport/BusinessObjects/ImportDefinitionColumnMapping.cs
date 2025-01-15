using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;

namespace ExcelImport.BusinessObjects
{
    [ModelDefault("IsCloneable", "True")]
    [XafDisplayName("Column Mapping")]
    public class ImportDefinitionColumnMapping : ImportDefinitionMappingBase
    {
        public ImportDefinitionColumnMapping(Session session) : base(session)
        {
        }

        private ImportDefinition _importDefinition;
        [Association]
        public ImportDefinition ImportDefinition
        {
            get { return _importDefinition; }
            set
            {
                if (SetPropertyValue<ImportDefinition>(nameof(ImportDefinition), ref _importDefinition, value))
                    if (!this.IsLoading)
                        ImportDefinitionChanged();
            }
        }

        private ComplexFileDefinition complexFileDefinition;

        
        public ComplexFileDefinition ComplexFileDefinition
        {
            get { return complexFileDefinition; }
            set { complexFileDefinition = value; }
        }

        private void ImportDefinitionChanged()
        {
            if (this.ImportDefinition != null && this.OrderNumber == 0)
                this.OrderNumber = this.ImportDefinition.ColumnMappings.Select(m => m.OrderNumber).DefaultIfEmpty(0).Max() + 1;
        }
        public override ImportDefinition GetImportDefinition()
        {
            return this.ImportDefinition;
        }

        protected override void OrderNumberChanged()
        {
            if (this.ImportDefinition != null)
            {
                List<ImportDefinitionColumnMapping> mappings = this.ImportDefinition.ColumnMappings.Where(m => m.OrderNumber == this.OrderNumber && m != this).ToList();
                foreach (ImportDefinitionColumnMapping mapping in mappings)
                    mapping.OrderNumber++;
            }
        }
    }
}
