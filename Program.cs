using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Web;

namespace KalturaCsvToLog
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("KalturaCsvToLog");
            Console.WriteLine("Usage: KalturaCsvToLog <column> <separator>");

            var paths = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csv");
            Console.WriteLine($"Found {paths.Length} CSV files \t{string.Join("\t", paths)}");

            var column = args.Length > 0 ? args[0] : "message";
            Console.WriteLine($"Set CSV column - {column}");

            var separator = args.Length > 1 ? args[1] : ",";
            Console.WriteLine($"Set CSV separator - {separator}");

            Console.WriteLine("Converting logs from CSV to log format");

            foreach (var path in paths)
                ConvertCsvToLog(
                    path, 
                    Path.ChangeExtension(path, "log"), 
                    column, 
                    separator);

            Console.WriteLine("All logs have been converted from CSV into log format");
            Console.WriteLine("Good luck!");
            Console.ReadKey(true);
        }

        private static void ConvertCsvToLog(string csvPath, string logPath, string column, string separator)
        {
            var lines = File.ReadAllLines(csvPath);

            using var log = new StreamWriter(logPath);

            foreach (var line in GetMessages(lines, column, separator))
                log.WriteLine(line);

            Console.WriteLine($"Created {logPath}");
        }

        private static IEnumerable<string> GetMessages(string[] lines, string column, string separator)
        {
            var header = lines.FirstOrDefault();

            if (header is null)
            {
                Console.WriteLine("ERROR: No header");
                yield break;
            }

            var columnIndex =
                Array.IndexOf(
                    header.ToUpperInvariant().Split(separator),
                    column.ToUpperInvariant());

            foreach (var line in lines.Skip(1))
            {
                // TODO: rework that! Kibana wraps messages into \" and escape any \" inside messages. We are looking for string messages on position 2 and it works. For other situation it won't.
                var values = line.Split($"\"\"\"{separator}");

                if (values.Length <= columnIndex)
                {
                    Console.WriteLine($"ERROR: Cannot find value by index:{columnIndex} for column:{column}\tERROR: line:{line}");
                    continue;
                }

                yield return 
                    values[columnIndex]
                        .Trim('"', '\'', '.', ';')
                        .Replace("\\\"", "")
                        .Replace("&gt;", ">")
                        .Replace("&lt;", "<");
            }
        }
    }
}
