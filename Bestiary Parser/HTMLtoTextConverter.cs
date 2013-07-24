using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bestiary_Parser
{
    /// <summary>
    /// Static methods for stripping out HTML and blank lines.
    /// </summary>
    public class HTMLtoTextConverter
    {
        /// <summary>
        /// Strips the HTML and blank lines out of an enumeration of lines.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The text with no HTML or blank lines, as an array.</returns>
        public static string[] StripHTML(IEnumerable<string> input)
        {
            // Build a single line from the multiple lines
            StringBuilder sb = new StringBuilder();
            foreach (string s in input)
            {
                sb.AppendLine(s);
            }

            // Use the single string method
            var result = StripHTML(sb.ToString());

            // Split back out the individual lines
            return Regex.Split(result, "\r\n|\r|\n");
        }

        /// <summary>
        /// Strips the HTML and blank lines out of a string containing multiple lines of text.
        /// </summary>
        /// <param name="input">The input to strip.</param>
        /// <returns>The text with no HTML or blank lines, as a single multiline string.</returns>
        public static string StripHTML(string input)
        {
            stripHtmlTags(ref input);

            stripNonBreakingSpaces(ref input);

            stripCssIdentifiers(ref input);

            stripCssClasses(ref input);

            stripEmptyLines(ref input);

            return input;
        }

        private static void stripEmptyLines(ref string input)
        {
            // Match all empty lines
            Regex regex = new Regex(@"^[\r\n(\r\n)\s]*", RegexOptions.Multiline);

            // Strip them out
            input = regex.Replace(input, string.Empty);
        }

        private static void stripCssClasses(ref string input)
        {
            // Match all CSS classes
            Regex regex = new Regex(@"(.\w*)\{[^\}]*\}", RegexOptions.Multiline);

            // Strip them out
            input = regex.Replace(input, string.Empty);
        }

        private static void stripCssIdentifiers(ref string input)
        {
            // Match all CSS identifiers
            Regex regex = new Regex(@"(#\w*\s)+\{[^\}]*\}", RegexOptions.Multiline);

            // Strip them out
            input = regex.Replace(input, string.Empty);
        }

        private static void stripNonBreakingSpaces(ref string input)
        {
            // Match all &nbsp; s
            Regex regex = new Regex(@"&nbsp;", RegexOptions.Multiline);

            // Strip out all of the non-breaking spaces
            input = regex.Replace(input, string.Empty);
        }

        private static void stripHtmlTags(ref string input)
        {
            // Match all HTML tags
            Regex regex = new Regex(@"<[^>]*>", RegexOptions.Multiline);

            // Strip out all of the tags
            input = regex.Replace(input, string.Empty);
        }
    }
}