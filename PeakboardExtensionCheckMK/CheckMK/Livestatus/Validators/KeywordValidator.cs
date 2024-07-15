using System.Linq;

namespace CheckMK.Livestatus.Validators
{
    internal class KeywordValidator : IValidator
    {
        private static readonly string[] ALLOWED_KEYWORDS = {
            "GET",
            "COMMAND",
            "Columns",
            "Filter",
            "Separators",
            "OutputFormat",
            "Stats", 
            "StatsAnd", 
            "ColumnHeaders", 
            "Timelimit", 
            "AuthUser",
            "WaitObject",
            "WaitCondition",
            "WaitTrigger",
            "WaitTimeout",
            "Localtime",
            "ResponseHeader",
            "KeepAlive"
        };
        private bool _foundBaseKeyword = false;

        public void Validate(QueryValidator validator, string query)
        {
            foreach (var item in query.Split(';'))
            {
                string keyword = item.Contains(" ") ? item.Split(' ')[0] : item;
                if (keyword.EndsWith(":"))
                {
                    keyword = keyword.Remove(keyword.Length - 1, 1);
                }

                if (!ALLOWED_KEYWORDS.Contains(keyword))
                    throw new LivestatusInvalidQueryException(1, 1, $"Invalid keyword '{keyword}'.");

                if (keyword == "GET")
                {
                    if (_foundBaseKeyword)
                        throw new LivestatusInvalidQueryException(1, 1, "Multiple base keywords are not allowed.");

                    _foundBaseKeyword = true;
                }
            }
        }
    }
}
