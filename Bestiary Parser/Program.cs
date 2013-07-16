using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bestiary_Parser
{
    internal class Program
    {
        private const string FILE_PATH = @"../../../workingParse.htm";

        private static List<BestiaryEntry> entries;

        public const string LOG_FILE_PATH = @"Log.txt";
        public const string OUTPUT_PATH = @"Output.txt";

        private static void Main(string[] args)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(new StreamReader(FILE_PATH));

            File.Delete(LOG_FILE_PATH);
            File.Delete(OUTPUT_PATH);

            entries = new List<BestiaryEntry>();

            var htmlNode = doc.DocumentNode.SelectSingleNode(@"//P[@class='p2 ft9']");

            int nodeCount = 0;
            foreach (HtmlNode pageNode in doc.DocumentNode.Descendants())
            {
                foreach (HtmlNode node in pageNode.Descendants())
                {
                    //Console.WriteLine(node.InnerText);
                }

                var monsterNode = pageNode.SelectSingleNode(".//div");
                if (monsterNode != null && monsterNode.InnerText != null)
                {
                    nodeCount++;
                    var entry = BestiaryEntry.CreateFromText(monsterNode.InnerText);
                    if (entry != null)
                    {
                        entries.Add(entry);
                    }
                }
            }

            Type entryType = typeof(BestiaryEntry);

            foreach (BestiaryEntry entry in entries)
            {
                using (StreamWriter output = File.AppendText(OUTPUT_PATH))
                {
                    foreach (PropertyInfo property in entryType.GetProperties())
                    {
                        if (property.Name != "Text")
                        {
                            output.WriteLine("{0}: {1}", property.Name, property.GetValue(entry));
                        }
                    }

                    output.WriteLine("===={0}====", entry.Name);
                }
            }

            Console.WriteLine("{0} parsed vs {1} detected", entries.Count, nodeCount);
        }
    }

    public class BestiaryEntry
    {
        private static int errCount;
        public const string LOG_FILE_PATH = @"Log.txt";
        public const string OUTPUT_PATH = @"Output.txt";

        public string Name { get; set; }

        public string Text { get; set; }

        public string SizeType { get; set; }

        public string ArmorClass { get; set; }

        public string HitPoints { get; set; }

        public string Speed { get; set; }

        public string Senses { get; set; }

        public string Strength { get; set; }

        public string Dexterity { get; set; }

        public string Constitution { get; set; }

        public string Intelligence { get; set; }

        public string Wisdom { get; set; }

        public string Charisma { get; set; }

        public string Alignment { get; set; }

        public string Languages { get; set; }

        public List<string> Traits { get; set; }

        public List<string> Actions { get; set; }

        public string XPLine { get; set; }

        public string Size { get; set; }

        public string Type { get; set; }

        public int XP { get; set; }

        public int Level { get; set; }

        public static BestiaryEntry CreateFromText(string text)
        {
            text = text.Trim();
            text.Replace("\r", "");
            var lines = text.Split('\n');

            lines =
                (from line in lines
                 where !string.IsNullOrWhiteSpace(line) && !line.Contains("&nbsp")
                 select line.Trim()).ToArray();

            var entry = new BestiaryEntry();
            entry.Text = text;
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

                entry.Traits =
                (from line in lines.SkipWhile(x => x != "TRAITS").TakeWhile(x => x != "ACTIONS" && x != "ENCOUNTER BUILDING")
                 select line).Skip(1).ToList();

                entry.Actions =
                    (from line in lines.SkipWhile(x => x != "ACTIONS").TakeWhile(x => x != "ENCOUNTER BUILDING")
                     select line).Skip(1).ToList();

                entry.XPLine = lines.Last();
                int result;
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

                using (StreamWriter logFile = File.AppendText(LOG_FILE_PATH))
                {
                    logFile.WriteLine(text);

                    logFile.WriteLine(string.Format("========{0}========", errCount++));
                }
                return null;
            }

            using (StreamWriter logFile = File.AppendText(LOG_FILE_PATH))
            {
                logFile.WriteLine("{0} successfully parsed.", entry.Name);
                logFile.WriteLine(string.Format("========{0}========", errCount++));
            }
            return entry;
        }
    }
}