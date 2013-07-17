using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

            // Write about the status of the parsing
            Console.WriteLine("{0} parsed vs {1} detected", entries.Count, nodeCount);
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

    /// <summary>
    /// Stores information parsed from the input file.
    /// </summary>
    public class BestiaryEntry
    {
        // Count the number of errors that have occurred since running the program.
        private static int errCount;

        // Double write the log and output file so that BestiaryEntry has it.
        public const string LOG_FILE_PATH = @"Log.txt";

        public const string OUTPUT_PATH = @"Output.txt";

        /// <summary>
        /// Gets or sets the name of the creature.
        /// </summary>
        /// <value>
        /// The name of the creature.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text the entry is parsed from.
        /// </summary>
        /// <value>
        /// The text the entry is parsed from.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the SizeType line of the creature.
        /// </summary>
        /// <value>
        /// The sizeType line.
        /// </value>
        public string SizeType { get; set; }

        /// <summary>
        /// Gets or sets the armor class.
        /// </summary>
        /// <value>
        /// The armor class.
        /// </value>
        public string ArmorClass { get; set; }

        /// <summary>
        /// Gets or sets the hit points.
        /// </summary>
        /// <value>
        /// The hit points.
        /// </value>
        public string HitPoints { get; set; }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public string Speed { get; set; }

        /// <summary>
        /// Gets or sets the senses the creature possesses.
        /// </summary>
        /// <value>
        /// The senses.
        /// </value>
        public string Senses { get; set; }

        /// <summary>
        /// Gets or sets the strength.
        /// </summary>
        /// <value>
        /// The strength.
        /// </value>
        public string Strength { get; set; }

        /// <summary>
        /// Gets or sets the dexterity.
        /// </summary>
        /// <value>
        /// The dexterity.
        /// </value>
        public string Dexterity { get; set; }

        /// <summary>
        /// Gets or sets the constitution.
        /// </summary>
        /// <value>
        /// The constitution.
        /// </value>
        public string Constitution { get; set; }

        /// <summary>
        /// Gets or sets the intelligence.
        /// </summary>
        /// <value>
        /// The intelligence.
        /// </value>
        public string Intelligence { get; set; }

        /// <summary>
        /// Gets or sets the wisdom.
        /// </summary>
        /// <value>
        /// The wisdom.
        /// </value>
        public string Wisdom { get; set; }

        /// <summary>
        /// Gets or sets the charisma.
        /// </summary>
        /// <value>
        /// The charisma.
        /// </value>
        public string Charisma { get; set; }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>
        /// The alignment.
        /// </value>
        public string Alignment { get; set; }

        /// <summary>
        /// Gets or sets the languages the creature knows.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public string Languages { get; set; }

        /// <summary>
        /// Gets or sets the traits. A list of strings.
        /// </summary>
        /// <value>
        /// The traits.
        /// </value>
        public List<string> Traits { get; set; }

        /// <summary>
        /// Gets or sets the actions. A list of strings.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        public List<string> Actions { get; set; }

        /// <summary>
        /// Gets or sets the XP line parsed from the text.
        /// </summary>
        /// <value>
        /// The XP line.
        /// </value>
        public string XPLine { get; set; }

        /// <summary>
        /// Gets or sets the size of the creature.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the type of the creature.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the XP the creature is worth.
        /// </summary>
        /// <value>
        /// The XP.
        /// </value>
        public int XP { get; set; }

        /// <summary>
        /// Gets or sets the level of the creature.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level { get; set; }

        /// <summary>
        /// Parses a creature from the entry text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The parsed creature, or null if an error occurred.</returns>
        public static BestiaryEntry CreateFromText(string text)
        {
            // Clean the whitespace from the entries, remove the carriage returns, and then split on newlines.
            // Ideally, this is make every string in lines a line of the text.
            text = text.Trim();
            text.Replace("\r", "");
            var lines = text.Split('\n');

            // LINQ syntax. Select only the lines that have something and that don't contain "&nbsp"
            lines =
                (from line in lines
                 where !string.IsNullOrWhiteSpace(line) && !line.Contains("&nbsp")
                 select line.Trim()).ToArray();

            var entry = new BestiaryEntry();
            entry.Text = text;
            // Try parsing the entry. If fails, return null and log the error.
            try
            {
                entry.Name = lines[0];
                entry.SizeType = lines[1];
                entry.Size = entry.SizeType.Split().First();
                entry.Type = entry.SizeType.Split().Last();
                entry.ArmorClass = lines[2].Split().Last();
                entry.HitPoints = lines[3].Split().Last();
                entry.Speed = lines[4].Split()[1];
                entry.Senses = lines[5];
                entry.Strength = lines[6].Split().Skip(1).First();
                entry.Dexterity = lines[7].Split().Skip(1).First();
                entry.Constitution = lines[8].Split().Skip(1).First();
                entry.Intelligence = lines[9].Split().Skip(1).First();
                entry.Wisdom = lines[10].Split().Skip(1).First();
                entry.Charisma = lines[11].Split().Skip(1).First();
                entry.Alignment = lines[12].Split().Skip(1).First();
                entry.Languages = lines[13];

                // LINQ Shenanigans. Get all the strings in between TRAITS and ACTION, making sure to stop before ENCOUNTER BUILDING.
                entry.Traits =
                (from line in lines.SkipWhile(x => x != "TRAITS").TakeWhile(x => x != "ACTIONS" && x != "ENCOUNTER BUILDING")
                 select line).Skip(1).ToList();

                // Get all strings between ACTIONS and ENCOUNTER BUILDING
                entry.Actions =
                    (from line in lines.SkipWhile(x => x != "ACTIONS").TakeWhile(x => x != "ENCOUNTER BUILDING")
                     select line).Skip(1).ToList();

                // The last line should be the XP lines
                entry.XPLine = lines.Last();
                int result;

                // Pull out the Level and XP as ints from the XPLine
                var xpResults =
                (from word in entry.XPLine.Split()
                 where Int32.TryParse(word, out result)
                 select Convert.ToInt32(word));

                entry.Level = xpResults.First();
                entry.XP = xpResults.Last();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Caught in {0}: {1}", entry.Name, ex.Message);

                // Log the error
                using (StreamWriter logFile = File.AppendText(LOG_FILE_PATH))
                {
                    logFile.WriteLine(text);

                    logFile.WriteLine(string.Format("========{0}========", errCount++));
                }

                // Return null
                return null;
            }

            // Log the success and return the parsed creature.
            using (StreamWriter logFile = File.AppendText(LOG_FILE_PATH))
            {
                logFile.WriteLine("{0} successfully parsed.", entry.Name);
                logFile.WriteLine(string.Format("========{0}========", errCount++));
            }
            return entry;
        }
    }
}