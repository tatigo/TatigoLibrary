using TatigoLibrary.Common.DesignByContract;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TatigoLibrary.Common
{
    public class Filter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Filters filenames that matches a wildcard pattern
        /// </summary>
        /// <example>
        /// InFileNames = { "hello.t", "HelLo.tx", "HeLLo.txt", "HeLLo.txtsjfhs", "HeLLo.tx.sdj", "hAlLo20984.txt" };
        /// InMasksToFilter = {"hello.txt", "hello.tx" , "H*o*.???" }
        /// </example>
        public static List<string> FilterFileNames(List<string> InFileNames, List<string> InMasksToFilter)
        {
            Check.Require(InFileNames != null && InFileNames.Count > 0);

            List<string> matches = new List<string>();

            if (InMasksToFilter == null || InMasksToFilter.Count == 0)
                return matches;

            foreach (var mask in InMasksToFilter)
            {
                var filteredFiles = FilterFileNames(mask, InFileNames);

                log.Info(string.Format("Found [{0}] files with [{1}] mask", filteredFiles.Count, mask));

                matches.AddRange(filteredFiles);
            }

            return matches;
        }

        public static List<string> FilterFileNames(string InPattern, List<string> InNames)
        {
            List<string> matches = new List<string>();
            Regex regex = FindFilesPatternToRegex.Convert(InPattern);
            foreach (string s in InNames)
            {
                if (regex.IsMatch(s))
                {
                    matches.Add(s);
                }
            }
            return matches;
        }

        public static bool IsNameMatchesWildCard(string InPattern, string InName)
        {
            Check.Require(!string.IsNullOrEmpty(InName));
            Check.Require(!string.IsNullOrEmpty(InPattern));

            Regex regex = new Regex(InPattern);
            return regex.IsMatch(InName);
        }
    }

    internal static class FindFilesPatternToRegex
    {
        private static Regex CatchExtentionRegex = new Regex(@"^\s*.+\.([^\.]+)\s*$", RegexOptions.Compiled);
        private static Regex HasQuestionMarkRegEx = new Regex(@"\?", RegexOptions.Compiled);
        private static Regex IllegalCharactersRegex = new Regex("[" + @"\/:<>|" + "\"]", RegexOptions.Compiled);
        private static string NonDotCharacters = @"[^.]*";

        public static Regex Convert(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException();
            }
            pattern = pattern.Trim();
            if (pattern.Length == 0)
            {
                throw new ArgumentException("Pattern is empty.");
            }
            if (IllegalCharactersRegex.IsMatch(pattern))
            {
                throw new ArgumentException("Pattern contains illegal characters.");
            }
            bool hasExtension = CatchExtentionRegex.IsMatch(pattern);
            bool matchExact = false;
            if (HasQuestionMarkRegEx.IsMatch(pattern))
            {
                matchExact = true;
            }
            else if (hasExtension)
            {
                matchExact = CatchExtentionRegex.Match(pattern).Groups[1].Length != 3;
            }
            string regexString = Regex.Escape(pattern);
            regexString = "^" + Regex.Replace(regexString, @"\\\*", ".*");
            regexString = Regex.Replace(regexString, @"\\\?", ".");
            if (!matchExact && hasExtension)
            {
                regexString += NonDotCharacters;
            }
            regexString += "$";
            Regex regex = new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex;
        }
    }
}