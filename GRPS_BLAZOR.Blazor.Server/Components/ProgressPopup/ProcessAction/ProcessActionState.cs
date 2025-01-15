using System.ComponentModel;

namespace GRPS_BLAZOR.Blazor.Server.Components.ProgressPopup.ProcessAction
{
    public class ProcessActionState
    {
        private Dictionary<ProcessOperation, double> OperationCostMap = new Dictionary<ProcessOperation, double>()
                {
                    { ProcessOperation.ApplyingRules, 0.25 },
                    { ProcessOperation.CalculatingTotalPackaging, 0.25 },
                    { ProcessOperation.Operation3, 0.25 },
                    { ProcessOperation.Operation4, 0.25 },
                };

        public ProcessOperation CurrentOperation { get; set; }
        public double CurrentOperationCost => OperationCostMap[CurrentOperation];

        private double currentOperationIncrementalCost;
        public double CurrentOperationIncrementalCost
        {
            get
            {
                currentOperationIncrementalCost = 0;
                foreach (KeyValuePair<ProcessOperation, double> keyValuePair in OperationCostMap)
                {
                    if (keyValuePair.Key == CurrentOperation)
                        break;
                    currentOperationIncrementalCost += keyValuePair.Value;
                }
                return currentOperationIncrementalCost;
            }
        }
        public int CurrentOperationPercentage { get; set; }
        public string OperationElementsDetail { get; set; }
        public string OperationCurrentElementDetail { get; set; }
        public bool Indeterminate { get; set; }
    }

    public enum ProcessOperation
    {
        [Description("Applying Rules...")]
        ApplyingRules,
        [Description("Calculating Total Packaging...")]
        CalculatingTotalPackaging,
        [Description("Executing Operation 3...")]
        Operation3,
        [Description("Executing Operation 4...")]
        Operation4,
    }
}
