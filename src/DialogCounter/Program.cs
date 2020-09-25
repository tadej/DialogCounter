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
        private static ArrayList includes = new ArrayList();

        static void Main(string[] args)
        {
            Console.WriteLine("DialogCounter \ud83d\udde3️ 2019 Tadej Gregorcic");
            Console.WriteLine("Counts dialog words and lines in the format:");
            Console.WriteLine("Speaker Name: Spoken Text until endline");
            Console.WriteLine("");

            string fileName = "";

            if (args.Length < 2)
            {
                Console.WriteLine("usage: DialogCounter filename.ink");
                fileName = @"c:\users\tadej\desktop\work\motiviti\elroy-script\elroy.ink";
                //return;
            }
            else
            {
                fileName = args[1];
            }

            var fileNames = "";

            GetIncludes(fileName);

            CountLines(fileName, out int totalLines, out int totalWords);
            fileNames = fileName;

            foreach(string include in includes)
            {
                fileNames += " " + include;
                int tlines = 0;
                int twords = 0;
                CountLines(include, out tlines, out twords);
                totalLines += tlines;
                totalWords += twords;
            }

            DisplayResults(fileNames, totalLines, totalWords);
        }

        private static void GetIncludes(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    int i = line.IndexOf("INCLUDE");
                    if (i!=-1)
                    {
                        int j = fileName.LastIndexOf(Path.DirectorySeparatorChar);

                        string path = fileName.Substring(0, j+1);

                        includes.Add(path + line.Replace("INCLUDE ", "").Trim());
                    }
                }
            }
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
                    line = line.TrimStart('+');
                    line = line.Replace("--", "").Replace("+++", "").Replace("---", "").Replace("+ -", "").Replace("+  ", "");
                    line = line.Replace("- ", "");
                    line = line.Replace("+  ", "");
                    line = line.Replace("+ ", "");
                    line = line.Trim();
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
