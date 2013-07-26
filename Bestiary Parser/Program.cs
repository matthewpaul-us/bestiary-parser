using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Bestiary_Parser
{
    /// <summary>
    /// Program to parse the bestiary html file.
    /// </summary>
    internal class Program
    {
        // The file to parse
        private const string FILE_PATH = @"../../../BestiaryHtml.htm";

        // The entries that were successfully parsed
        private static List<BestiaryEntry> entries;

        // The file to write the log to.
        public const string LOG_FILE_PATH = @"Log.txt";

        // The output file to write the successful parsings to
        public const string OUTPUT_PATH = @"Output.txt";

        /// <summary>
        /// Driver method. Does nothing with args.
        /// </summary>
        /// <param name="args">The args. Nothing is done with args.</param>
        private static void Main(string[] args)
        {
            // Clear both the log and the output files
            File.Delete(LOG_FILE_PATH);
            File.Delete(OUTPUT_PATH);

            entries = new List<BestiaryEntry>();

            // Get the text from the input file
            var text = File.ReadAllText(FILE_PATH);

            // Strip the html and spaces from the text
            text = HTMLtoTextConverter.StripHTML(text);

            // Strip everything not related to the entries
            text = stripExtras(text);
            File.WriteAllText("../../../strippedText.txt", text);
            // Log all the entries to the log file
            //LogEntries();

            // Write all entries to the output file
            //WriteEntries(OUTPUT_PATH, entries);
        }

        private static string stripExtras(string text)
        {
            // Split it out into lines
            var lines = Regex.Split(text, @"\r\n|\n|\r");
            // Skip everything until the first entry
            lines = lines.SkipWhile(line => line != "Ankheg").ToArray();
            // Skip the lines containing footer material
            skipFooter(ref lines);

            // Rebuild a single string from the lines
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        private static void skipFooter(ref string[] lines)
        {
            var indexesToSkip = new List<int>();
            // Match the following lines:
            //D&D Next Playtest
            //©2013 Wizards
            //16
            //Confidential information of Wizards of the Coast LLC.
            //Do not distribute.
            Regex regex = new Regex(@"D&D.*|.20\d{2}\sWizards|^\d+$|Confidential\sinformation.*|Do\snot\sdistr.*");
            for (int i = 0; i < lines.Length; i++)
            {
                if (regex.IsMatch(lines[i]))
                {
                    indexesToSkip.Add(i);
                }
            }

            // Select only the lines that are not footer material
            lines = lines.Where((line, index) => !indexesToSkip.Contains(index)).ToArray();
        }

        private static void WriteEntries(string path, IEnumerable<BestiaryEntry> entries)
        {
            CSVWriter.WriteFile(path, entries);
        }

        /// <summary>
        /// Logs the entries successfully read from the input file.
        /// </summary>
        private static void LogEntries()
        {
            // Get the type of the bestiary entry. Used for iterating over its properties
            Type entryType = typeof(BestiaryEntry);

            // For every entry...
            foreach (BestiaryEntry entry in entries)
            {
                // Open the output file. Enclosed in a using clause to make sure the file
                // is closed when the writing is finished.
                using (StreamWriter output = File.AppendText(OUTPUT_PATH))
                {
                    // ...Write each property that's not text to the output file.
                    // Format is "[name]: [value]"
                    foreach (PropertyInfo property in entryType.GetProperties())
                    {
                        // Print the lists
                        if (property.PropertyType.IsAssignableFrom(typeof(List<string>)))
                        {
                            var list = (List<string>)property.GetValue(entry);
                            output.WriteLine(property.Name);

                            foreach (string item in list)
                            {
                                output.WriteLine("    {0}", item);
                            }

                            // Don't re-print the lists
                            continue;
                        }
                        if (property.Name != "Text")
                        {
                            output.WriteLine("{0}: {1}", property.Name, property.GetValue(entry));
                        }
                    }

                    output.WriteLine("===={0}====", entry.Name);
                }
            }
        }
    }
}