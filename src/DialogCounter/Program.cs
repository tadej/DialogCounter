using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace DialogCounter
{
    class Program
    {
        private static Hashtable words = new Hashtable();
        private static Hashtable lines = new Hashtable();

        static void Main(string[] args)
        {
            Console.WriteLine("DialogCounter (c) 2019 Tadej Gregorcic");
            Console.WriteLine("Counts lines in the format");
            Console.WriteLine("Speaker Name: Spoken Text");
            string fileName = "";

            if (args.Length < 2)
            {
                Console.WriteLine("usage: DialogCounter filename.ink");

                //fileName = "/Users/tadej/elroy/wip-documents-art/elroy.ink";
                return;
            }
            else
            {
                fileName = args[1];
            }

            CountLines(fileName, out int totalLines, out int totalWords);

            DisplayResults(fileName, totalLines, totalWords);
        }

        private static void DisplayResults(string fileName, int totalLines, int totalWords)
        {
            Console.WriteLine("");
            Console.WriteLine("File: " + fileName);
            Console.WriteLine("***************************************************************");
            Console.WriteLine("Total words: " + totalWords.ToString());
            Console.WriteLine("Total lines: " + totalLines.ToString());
            Console.WriteLine("");

            int i = 0;
            foreach (var charName in GetKeysInValueOrder(words))
            {
                Console.WriteLine((++i).ToString() + ": " + charName + ": " + ((int)words[charName]).ToString() + " words / " + ((int)lines[charName]).ToString() + " lines");
            }
        }

        private static string[] GetKeysInValueOrder(Hashtable arrayToSort)
        {
            string[] keys = new string[arrayToSort.Count];
            arrayToSort.Keys.CopyTo(keys, 0);

            Array.Sort(keys, delegate (string x, string y) 
            {
                return (arrayToSort[y] as IComparable).CompareTo(arrayToSort[x]);
            });

            return keys;
        }

        private static void CountLines(string fileName, out int totalLines, out int totalWords)
        {
            totalLines = 0;
            totalWords = 0;

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.TrimStart().IndexOf("//") == 0) continue;
                    if (line.TrimStart().IndexOf("#") == 0) continue;

                    line = line.TrimStart(' ');
                    line = line.TrimStart('+');
                    line = line.TrimStart('*');
                    line = line.TrimStart('-');

                    line = Regex.Replace(line, @"\[.*\]", "");
                    line = Regex.Replace(line, @"\{.*\}", "");

                    int colon = line.IndexOf(':');
                    if (colon == -1) continue;

                    string charName = line.Substring(0, colon).Trim();
                    string charLine = line.Substring(colon + 1).Replace(" ... ", "").Replace("... ", "").Trim();

                    int cntWords = words[charName] != null ? (int)words[charName] : 0;
                    int cntLines = lines[charName] != null ? (int)lines[charName] : 0;

                    if (string.IsNullOrEmpty(charLine)) continue;

                    string[] lineWords = charLine.Split(' ');

                    if (lineWords != null && lineWords.Length != 0)
                    {
                        cntWords += lineWords.Length;
                        cntLines++;

                        totalWords += lineWords.Length;
                        totalLines++;
                    }

                    words[charName] = cntWords;
                    lines[charName] = cntLines;
                }
            }
        }
    }
}
