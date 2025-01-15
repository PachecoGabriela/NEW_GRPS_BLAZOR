using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace ExcelImport.BusinessObjects
{
    [ModelDefault("IsCloneable", "True")]
    [XafDisplayName("Cell Mapping")]
    public class ImportDefinitionCellMapping : ImportDefinitionMappingBase
    {
        public ImportDefinitionCellMapping(Session session) : base(session)
        {
        }

        protected override string GetDisplayName()
        {
            return $"[{RowNumber}, {ColumnNumber}] {PropertyCaption}";
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
        private void ImportDefinitionChanged()
        {
            if (this.ImportDefinition != null && this.OrderNumber == 0)
                this.OrderNumber = this.ImportDefinition.CellMappings.Select(m => m.OrderNumber).DefaultIfEmpty(0).Max() + 1;
        }
        public override ImportDefinition GetImportDefinition()
        {
            return this.ImportDefinition;
        }

        protected override void OrderNumberChanged()
        {
            if (this.ImportDefinition != null)
            {
                List<ImportDefinitionCellMapping> mappings = this.ImportDefinition.CellMappings.Where(m => m.OrderNumber == this.OrderNumber && m != this).ToList();
                foreach (ImportDefinitionCellMapping mapping in mappings)
                    mapping.OrderNumber++;
            }
        }

        private int _rowNumber = 1;
        [Index(0)]
        [VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public int RowNumber
        {
            get { return _rowNumber; }
            set { SetPropertyValue<int>(nameof(RowNumber), ref _rowNumber, value); }
        }
    }
}
