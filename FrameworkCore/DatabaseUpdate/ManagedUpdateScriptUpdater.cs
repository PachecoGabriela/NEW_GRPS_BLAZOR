using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using FrameworkCore.Utils;

namespace FrameworkCore.DatabaseUpdate
{
    public sealed class ManagedUpdateScriptUpdater : ModuleUpdater
    {
        public ManagedUpdateScriptUpdater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            UpdateScriptManager.Instance.UpdateDatabaseAfterUpdateSchema(ObjectSpace);
        }
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();

            UpdateScriptManager.Instance.UpdateDatabaseBeforeUpdateSchema(ObjectSpace);
        }
    }
}