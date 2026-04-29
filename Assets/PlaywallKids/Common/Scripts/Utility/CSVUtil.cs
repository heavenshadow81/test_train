using System.Linq;

namespace ML.PlaywallKids.Common
{
    public static class CSVUtil
    {
        public static string[] SplitCsvLine(string szLine)
        {
            return (from System.Text.RegularExpressions.Match kMatch in System.Text.RegularExpressions.Regex.Matches(szLine,
                                                                                                                     @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
                                                                                                                     System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select kMatch.Groups[1].Value).ToArray();
        }
    }
}