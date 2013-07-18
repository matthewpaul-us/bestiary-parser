using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary_Parser
{
    internal class CSVWriter
    {
        public static void WriteFile(string path, IEnumerable<BestiaryEntry> entries)
        {
            StreamWriter outFile = null;
            List<string> lines = new List<string>();
            try
            {
                // Open the file for writing.
                outFile = new StreamWriter(path);

                // Write headers
                WriteHeader(outFile);
                foreach (BestiaryEntry entry in entries)
                {
                    // Get the line from the entry, and write it to the file
                    outFile.WriteLine(getLineFromEntry(entry));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Make sure the file has been closed
                if (outFile != null)
                {
                    outFile.Close();
                }
            }
        }

        /// <summary>
        /// Writes the header of the CSV File.
        /// </summary>
        /// <param name="outFile">The out file to write the header to.</param>
        private static void WriteHeader(StreamWriter outFile)
        {
            StringBuilder sb = new StringBuilder();

            object[] values = new object[]
            {
                "Name", "Size", "ArmorClass", "HitPoints", "Speed", "Senses", "Strength", "Mod",
                "Dexterity", "Mod", "Constitution", "Mod", "Intelligence", "Mod", "Wisdom", "Mod", "Charisma", "Mod",
                "Alignment", "Traits", "Actions", "Level", "XP"
            };

            // Append all the values in csv format
            foreach (object value in values)
            {
                sb.Append(string.Format("{0}~", value));
            }

            outFile.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Gets the line representing the entry as a CSV row.
        /// </summary>
        /// <param name="entry">The entry to convert.</param>
        /// <returns>A Comma-Separated Values row containing the monster entry, as a string.</returns>
        private static string getLineFromEntry(BestiaryEntry entry)
        {
            StringBuilder sb = new StringBuilder();

            // Join together the strings so they will be in one entry.
            string joinedActions = JoinStrings(entry.Actions);
            string joinedTraits = JoinStrings(entry.Traits);

            // Beast Size AC HP Speed Senses Str Mod Dex Mod Con Mod Int Mod Wis Mod Cha Mod Alignment Traits Actions Level XP
            object[] values = new object[]
            {
                entry.Name, entry.Size, entry.ArmorClass, entry.HitPoints, entry.Speed, entry.Senses, entry.Strength, null,
                entry.Dexterity, null, entry.Constitution, null, entry.Intelligence, null, entry.Wisdom, null, entry.Charisma, null,
                entry.Alignment, joinedTraits, joinedActions, entry.Level, entry.XP
            };

            // Append all the values in csv format
            foreach (object value in values)
            {
                sb.Append(string.Format("{0}~", value));
            }

            return sb.ToString();
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