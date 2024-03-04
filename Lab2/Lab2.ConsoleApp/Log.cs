using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.ConsoleApp;
public static class Log {
    private static string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

    public static void Write(string[] logArray) {
        if (!File.Exists(LogFile))
            File.Create(LogFile);

        string log = string.Empty;
        Array.ForEach(logArray, line => log += $"{line.Trim()}\n");

        using StreamWriter writer = new(LogFile);
        writer.WriteLine(log);
        writer.Close();
    }

    public static void Show() {
        try {
            Process.Start("notepad", LogFile);
        } catch (Exception ex) {
            Debug.WriteLine($"Couldn't open \"{LogFile}\": {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void Delete() => File.Delete(LogFile);
}
