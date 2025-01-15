using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using FrameworkCore.BusinessObjects;
using FrameworkCore.Extensions;

namespace FrameworkCore.Utils
{
    public sealed class UpdateScriptManager
    {
        private UpdateScriptManager() { }       
        public static UpdateScriptManager Instance
        {
            get
            {
                IValueManager<UpdateScriptManager> manager = ValueManager.GetValueManager<UpdateScriptManager>(nameof(UpdateScriptManager));
                if (manager.Value == null)
                    manager.Value = new UpdateScriptManager();

                return manager.Value;
            }
        }

        public bool UpdateWhenDebugging { get; set; } = true;

        readonly List<IUpdateScriptProvider> providers = new List<IUpdateScriptProvider>();
        public void Initialize(XafApplication application)
        {
            providers.Clear();
            providers.AddRange(application.Modules.Where(x => x.GetType().GetInterface(typeof(IUpdateScriptProvider).FullName) != null).OfType<IUpdateScriptProvider>());
        }

        internal void UpdateDatabaseBeforeUpdateSchema(IObjectSpace space)
        {
            if (!UpdateWhenDebugging && Debugger.IsAttached)
                return;

            Session session = ((XPObjectSpace)space).Session;

            if (DatabaseExists(session))
            {
                DateTime lastupdate = GetLastUpdate(session);
                List<IUpdateScript> scripts = providers.SelectMany(x => x.GetPreUpdateScripts().Where(y=>y.CreatedDate > lastupdate)).OrderBy(x => x.CreatedDate).ToList();

                foreach (IUpdateScript script in scripts)
                {
                    UpdateScriptResult result = session.FindObject<UpdateScriptResult>(new BinaryOperator(nameof(UpdateScriptResult.UpdateID), script.UpdateID));
                    if (result == null)
                    {
                        result = new UpdateScriptResult(session);
                        result.UpdateID = script.UpdateID;
                        result.UpdateDescription = script.Description;
                        result.CreatedDate = script.CreatedDate;
                        result.RunOn = DateTime.Now;

                        try
                        {
                            result.Result = script.Run(space);
                        }
                        catch (Exception ex)
                        {
                            result.Result = ex.GetFullExceptionText();
                        }

                        space.CommitChanges();
                    }
                }
            }
        }

        internal void UpdateDatabaseAfterUpdateSchema(IObjectSpace space)
        {
            if (!UpdateWhenDebugging && Debugger.IsAttached)
                return;

            Session session = ((XPObjectSpace)space).Session;

            DateTime lastupdate = GetLastUpdate(session);
            List<IUpdateScript> scripts = providers.SelectMany(x => x.GetPostUpdateScripts().Where(y => y.CreatedDate > lastupdate)).OrderBy(x => x.CreatedDate).ToList();

            foreach (IUpdateScript script in scripts)
            {
                UpdateScriptResult result = session.FindObject<UpdateScriptResult>(new BinaryOperator(nameof(UpdateScriptResult.UpdateID), script.UpdateID));
                if (result == null)
                {
                    result = new UpdateScriptResult(session);
                    result.UpdateID = script.UpdateID;
                    result.UpdateDescription = script.Description;
                    result.CreatedDate = script.CreatedDate;
                    result.RunOn = DateTime.Now;

                    try
                    {
                        result.Result = script.Run(space.CreateNestedObjectSpace());
                    }
                    catch (Exception ex)
                    {
                        result.Result = ex.GetFullExceptionText();
                    }

                    space.CommitChanges();
                }
            }
        }

        private static bool DatabaseExists(Session session)
        {
            try
            {
                SelectedData d = session.ExecuteQuery($"SELECT * FROM sys.tables WHERE name ='{nameof(UpdateScriptResult)}'");
                return (d.ResultSet.GetValue(0) as SelectStatementResult).Rows.Length > 0;
            }
            catch (Exception ex)
            {
                Tracing.Tracer.LogError(ex);
                return false;
            }
        }

        private static DateTime GetLastUpdate(Session session)
        {
            try
            {
                return new XPQuery<UpdateScriptResult>(session).Max(x => x.CreatedDate);
            }
            catch(Exception ex)
            {
                Tracing.Tracer.LogError(ex);

                return DateTime.MinValue;
            }
        }
    }
}
