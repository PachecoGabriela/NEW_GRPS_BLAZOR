using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using FrameworkCore.DatabaseUpdate;
using FrameworkCore.Utils;

namespace FrameworkCore
{
    public sealed partial class FrameworkCoreModule : ModuleBase
    {
        public FrameworkCoreModule()
        {
            InitializeComponent();
            BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            return new ModuleUpdater[] { new ManagedUpdateScriptUpdater(objectSpace, versionFromDB) };
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);

            application.SetupComplete += (s, e) => { UpdateScriptManager.Instance.Initialize(this.Application); };
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }
    }
}
