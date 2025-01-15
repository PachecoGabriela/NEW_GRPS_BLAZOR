using System;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ExcelImport.BusinessObjects
{
    [Appearance("ImportCrossReferenceLine_OrderNumber_Appearance", null, TargetItems = nameof(OrderNumber), FontColor = "Black", BackColor = "WhiteSmoke")]
    [Appearance("ImportCrossReferenceLine_ImportValue_Appearance", null, TargetItems = "ImportValue;OutputValue", FontColor = "Black", BackColor = "LightYellow")]
    [ImageName("Replace")]
    [OptimisticLocking(false)]
    public class ImportCrossReferenceLine : BaseObject
    {
        public ImportCrossReferenceLine(Session session) : base(session) { }

        private ImportCrossReference _importCrossReference;
        [Association]
        public ImportCrossReference ImportCrossReference
        {
            get { return _importCrossReference; }
            set
            {
                if (SetPropertyValue<ImportCrossReference>(nameof(ImportCrossReference), ref _importCrossReference, value))
                    if (!this.IsLoading)
                        ImportCrossReferenceChanged();
            }
        }
        private void ImportCrossReferenceChanged()
        {
            if (this.ImportCrossReference != null)
                if (this.OrderNumber == 0)
                    this.OrderNumber = this.Session.QueryInTransaction<ImportCrossReferenceLine>().Where(l => l.ImportCrossReference == this.ImportCrossReference && l != this).Select(l => l.OrderNumber).Max() + 1;
        }

        private int _orderNumber;
        [ModelDefault("AllowEdit", "False")]
        [ToolTip("Order Number")]
        public int OrderNumber
        {
            get { return _orderNumber; }
            set { SetPropertyValue<int>(nameof(OrderNumber), ref _orderNumber, value); }
        }

        private string _importValue;
        [Size(30)]
        [ToolTip(
@"Import Value. 
Search pattern that should match with an Excel cell.
For 'RegEx' matching mode:
    . (dot): matches any alphanumerical word (e.g. 123WORD, WORD, 123456). Empty values are not matched.", "", ToolTipIconType.Information)]
        public string ImportValue
        {
            get { return _importValue; }
            set { SetPropertyValue<string>(nameof(ImportValue), ref _importValue, value); }
        }

        private string _outputValue;
        [Size(60)]
        [ToolTip(
@"Output Value. 
Output Value - replacement value that is applied to the imported record if there is a match.
Output Value can have the following tags:

{TimeStamp} - represents a current date and time format of yyyyMMdd_HHmmss, e.g. 20210622_150218
{CurrentDate} - represents a current date and time format of default format, e.g. 22/06/2021 15:16:27
{CurrentDate:yyyy-MM-dd}  - represents current date format of yyyy-MM-dd, e.g. 2021-06-22
{CurrentDate:yyyyMMdd_HHmmss} - represents current date and time format of yyyyMMdd_HHmmss, e.g. 20210622_151627
{Value} - represents entire Excel cell content.
{Value:X,Y} - represents the portion of Excel cell content (e.g. for {Value:2,4} and cell value '1234567' it returns '2345'), where:
    X - indicates the initial character 1-based index (ie. first character has index 1).
    Y - indicates the number of characters after initial index that would be copied from Excel cell content"
, "", ToolTipIconType.Information)]
        public string OutputValue
        {
            get { return _outputValue; }
            set { SetPropertyValue<string>(nameof(OutputValue), ref _outputValue, value); }
        }

        private ImportCrossReferenceMatchingMode _matchingMode;
        [ToolTip(
@"Matching Mode. 
Exact Match - do the replace if whole cell value Exactly Matches the searched portion.

Contains / Starts With / Ends With - do the replace if cell value Contains / Starts With / Ends With the searched portion.

RegEx - do the replace if cell value matches the regular expression specified in the Import Value.


Sample regex patterns:  
  ^C.*D6$               - (starts with 'C', then has 0 or many characters, then ends with 'D6')
  ...456..              - (has three (any) characters, then has '456', then has two (any) characters)
  [0-9]{3}[a-zA-Z]{2,5} - (has three digits, then has from two to five letters)

For testing regex see: www.regex101.com
", "", ToolTipIconType.Information)]
        public ImportCrossReferenceMatchingMode MatchingMode
        {
            get { return _matchingMode; }
            set { SetPropertyValue<ImportCrossReferenceMatchingMode>(nameof(MatchingMode), ref _matchingMode, value); }
        }

        private bool _caseSensitive;
        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set { SetPropertyValue<bool>(nameof(CaseSensitive), ref _caseSensitive, value); }
        }

        public bool IsMatch(object valueToAssign)
        {
            if (string.IsNullOrEmpty(this.ImportValue) || string.IsNullOrEmpty(this.OutputValue))
                return false;
            if (valueToAssign == null)
                return false;

            string stringValue = Convert.ToString(valueToAssign);
            if (string.IsNullOrEmpty(stringValue))
                return false;

            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            if (this.CaseSensitive)
                stringComparison = StringComparison.CurrentCulture;

            switch (this.MatchingMode)
            {
                case ImportCrossReferenceMatchingMode.ExactMatch:
                    return stringValue.Equals(this.ImportValue, stringComparison);
                case ImportCrossReferenceMatchingMode.Contains:
                    if (this.CaseSensitive)
                        return stringValue.Contains(this.ImportValue);
                    else
                        return stringValue.ToLower().Contains(this.ImportValue.ToLower());
                case ImportCrossReferenceMatchingMode.StartsWith:
                    return stringValue.StartsWith(this.ImportValue, stringComparison);
                case ImportCrossReferenceMatchingMode.EndsWith:
                    return stringValue.EndsWith(this.ImportValue, stringComparison);
                case ImportCrossReferenceMatchingMode.RegEx:
                    RegexOptions regexOptions = RegexOptions.Compiled;
                    if (!this.CaseSensitive)
                        regexOptions |= RegexOptions.IgnoreCase;
                    Regex regex = new Regex(this.ImportValue, regexOptions);
                    return regex.IsMatch(stringValue);
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public enum ImportCrossReferenceMatchingMode
    {
        ExactMatch = 0,
        Contains = 1,
        StartsWith = 2,
        EndsWith = 3,
        [XafDisplayName("RegEx")]
        RegEx = 4,
    }
}
