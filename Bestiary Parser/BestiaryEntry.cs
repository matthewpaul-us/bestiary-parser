using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary_Parser
{
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

        #region Properties

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

        #endregion Properties

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
                entry.HitPoints = lines[3].Split().Skip(2).First();
                entry.Speed = lines[4].Split()[1];
                entry.Senses = JoinStrings(lines[5].Split().Skip(1));
                entry.Strength = lines[6].Split().Skip(1).First();
                entry.Dexterity = lines[7].Split().Skip(1).First();
                entry.Constitution = lines[8].Split().Skip(1).First();
                entry.Intelligence = lines[9].Split().Skip(1).First();
                entry.Wisdom = lines[10].Split().Skip(1).First();
                entry.Charisma = lines[11].Split().Skip(1).First();
                entry.Alignment = lines[12].Split().Skip(1).First();
                entry.Languages = JoinStrings(lines[13].Split().Skip(1));

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

        public static string JoinStrings(IEnumerable<string> strings)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string item in strings)
            {
                sb.Append(item + " ");
            }

            return sb.ToString();
        }
    }
}