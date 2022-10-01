using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CoinBot.Helpers
{
    public static class AiRecognizer
    {
        public static (DateTime, TimeSpan) RecognizeDateTimeRange(string source, out string rawString)
        {
            List<ModelResult> aiResults = DateTimeRecognizer.RecognizeDateTime(source, Culture.English);
            if (aiResults.Count==0)
            {
                throw new Exception("No date/time found");
            }

            /* Example contents of the below dictionary
             * [0]: {[timex, 2018-11-11T06:15]}
             * [1]: {[type, datetime]}
             * [2]: {[value, 2018-11-11 06:15:00]}
             */

            rawString = aiResults[0].Text;
            Dictionary<string, string> aiResult = UnwindResult(aiResults[0]);
            foreach (KeyValuePair<string, string> kvp in aiResult)
            {
                Console.WriteLine($"[{kvp.Key}, {kvp.Value}]");
            }

            string type = aiResult["type"];
            
            if (type!= "datetimerange")
                throw new Exception("Not a date/time range");

            return (DateTime.Parse(aiResult["start"]), DateTime.Parse(aiResult["end"]) - DateTime.Parse(aiResult["start"]));
        }

        public static DateTime RecognizeDateTime(string source, out string rawString)
        {
            List<ModelResult> aiResults = DateTimeRecognizer.RecognizeDateTime(source, Culture.English);
            if (aiResults.Count == 0)
            {
                throw new Exception("No date/time found");
            }

            /* Example contents of the below dictionary
             * [0]: {[timex, 2018-11-11T06:15]}
             * [1]: {[type, datetime]}
             * [2]: {[value, 2018-11-11 06:15:00]}
             */

            rawString = aiResults[0].Text;
            Dictionary<string, string> aiResult = UnwindResult(aiResults[0]);
            foreach (KeyValuePair<string, string> kvp in aiResult)
            {
                Console.WriteLine($"[{kvp.Key}, {kvp.Value}]");
            }

            string type = aiResult["type"];

            if (type != "datetime")
                throw new Exception("Not a date/time");

            string result = Regex.IsMatch(type, @"range$") ? aiResult["start"] : aiResult["value"];
            return DateTime.Parse(result);
        }

        private static Dictionary<string, string> UnwindResult(ModelResult modelResult)
        {
            return (modelResult.Resolution["values"] as List<Dictionary<string, string>>)[0];
        }
    }
}
