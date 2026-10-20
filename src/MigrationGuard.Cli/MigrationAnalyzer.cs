using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MigrationGuard.Cli
{
    public static class MigrationAnalyzer
    {
        public static void Analyze(string sqlFile)
        {
            var sql = File.ReadAllText(sqlFile);

            var rules = new Dictionary<string, string>
            {
                [@"\bDROP\s+TABLE\b"] = "Dropping a table can destroy data.",
                [@"\bDROP\s+COLUMN\b"] = "Dropping a column can destroy data.",
                [@"\bTRUNCATE\s+TABLE\b"] = "Truncating a table removes all rows.",
                [@"\bDELETE\s+FROM\s+[\w.\[\]""`]+(?:\s*;|\s*$)"] =
                    "DELETE without a WHERE clause may remove every row.",
                [@"\bUPDATE\s+[\w.\[\]""`]+\s+SET\b(?:(?!\bWHERE\b)[\s\S])*?(?:;|$)"] =
                    "UPDATE without a WHERE clause may modify every row."
            };

            var foundIssues = false;

            foreach (var rule in rules)
            {
                if (!Regex.IsMatch(
                        sql,
                        rule.Key,
                        RegexOptions.IgnoreCase))
                {
                    continue;
                }

                foundIssues = true;
                Console.WriteLine($"[HIGH] {rule.Value}");
            }

            if (!foundIssues)
            {
                Console.WriteLine("No known dangerous SQL patterns detected.");
            }
        }
    }

}
