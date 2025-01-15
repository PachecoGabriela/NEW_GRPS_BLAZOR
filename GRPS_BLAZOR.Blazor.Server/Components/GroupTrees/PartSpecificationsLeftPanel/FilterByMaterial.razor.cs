using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using Microsoft.AspNetCore.Components;

namespace GRPS_BLAZOR.Blazor.Server.Components.GroupTrees.PartSpecificationsLeftPanel
{
    public class FilterByMaterialBase : ComponentBase
    {
        [Inject]
        IXafApplicationProvider ApplicationProvider { get; set; }

        [CascadingParameter(Name = "View")]
        public View View { get; set; }

        public IEnumerable<EnumInstance> Materials { get; set; }

        public EnumInstance CurrentMaterial { get; set; } = null;

        public bool FilterMaterial { get; set; }
        private static BlazorApplication blazorApplication;
        private static IObjectSpace objectSpace;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            blazorApplication = ApplicationProvider.GetApplication();
            objectSpace = blazorApplication.CreateObjectSpace();

            Materials = objectSpace.GetObjects<EnumInstance>(CriteriaOperator.FromLambda<EnumInstance>(ei => ei.Domain.Name == "MaterialType"));
        }

        protected void CheckedChanged(bool checkedValue)
        {
            FilterMaterial = checkedValue;
            CheckFilter();
        }

        protected void SelectedMaterialChanged(EnumInstance selectedMaterial)
        {
            CurrentMaterial = selectedMaterial;
            CheckFilter();
        }

        private void CheckFilter()
        {
            if (View is ListView listView)
            {
                if (CurrentMaterial is not null)
                {
                    if (FilterMaterial)
                    {
                        SetFilterByMaterial(listView);
                    }
                    else
                    {
                        ClearFilterByMaterial(listView);
                    }
                }
                else
                {
                    ClearFilterByMaterial(listView);
                }
            }
        }

        private void SetFilterByMaterial(ListView listView)
        {
            listView.CollectionSource.Criteria["FilterByMaterial"] =
                CriteriaOperator.FromLambda<Part>(p => p.PackWeights.Any(pw => pw.Material.Oid == CurrentMaterial.Oid));
        }

        private void ClearFilterByMaterial(ListView listView)
        {
            listView.CollectionSource.Criteria["FilterByMaterial"] = null;
        }
    }
}
