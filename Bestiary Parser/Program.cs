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
        private const string FILE_PATH = @"../../../workingParse.htm";

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
            // Create a new htmldocument and load the file to parse.
            HtmlDocument doc = new HtmlDocument();
            doc.Load(new StreamReader(FILE_PATH));

            // Clear both the log and the output files
            File.Delete(LOG_FILE_PATH);
            File.Delete(OUTPUT_PATH);

            entries = new List<BestiaryEntry>();

            // Get the first node that has the CSS class that matches an entry
            // Not used
            var htmlNode = doc.DocumentNode.SelectSingleNode(@"//P[@class='p2 ft9']");

            int nodeCount = 0;
            // Iterate through the nodes in the document
            foreach (HtmlNode pageNode in doc.DocumentNode.Descendants())
            {
                // Find a div inside the pageNode
                var monsterNode = pageNode.SelectSingleNode(".//div");
                // If it's not null and the inner text has something, it might be a monster
                if (monsterNode != null && monsterNode.InnerText != null)
                {
                    nodeCount++;

                    // Try to create an entry from the inner text.
                    var entry = BestiaryEntry.CreateFromText(monsterNode.InnerText);
                    if (entry != null)
                    {
                        // If it successfully parsed, add it to the list of entries.
                        entries.Add(entry);
                    }
                }
            }

            // Log all the entries to the log file
            LogEntries();

            // Write all entries to the output file
            WriteEntries(OUTPUT_PATH, entries);

            // Write about the status of the parsing
            Console.WriteLine("{0} parsed vs {1} detected", entries.Count, nodeCount);
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