using System.Diagnostics;
using System.IO;

namespace Lab3.Core.Output;

/// <summary>A class of the log</summary>
public sealed class Log {
    private string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

    private static Log instance = null!;
    public static Log Instance => instance ??= new Log();

    private List<string> _data = null!;
    private Log() => _data = [];

    /// <summary>
    /// Writes the line into the log
    /// </summary>
    /// <param name="line">The line to be written</param>
    public void WriteLine(string line) {
        _data.Add(line);
        Console.WriteLine(line);
    }

    /// <summary>
    /// Saves the log into the file
    /// </summary>
    public void Save() {
        if (!File.Exists(LogFile))
            File.Create(LogFile);

        using StreamWriter writer = new(LogFile);
        writer.WriteLine(string.Join("\n", _data));
        writer.Close();
    }

    /// <summary>
    /// Show the log file into notepad.exe
    /// </summary>
    public void Show() {
        try {
            Process process = new();
            process.StartInfo.FileName = "notepad";
            process.StartInfo.Arguments = LogFile;
            process.StartInfo.UseShellExecute = false;
            process.Start();

            //process.WaitForExit();
        } catch (Exception ex) {
            Debug.WriteLine($"Couldn't open \"{LogFile}\": {ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Clears the log
    /// </summary>
    public void Clear() => _data?.Clear();

    /// <summary>
    /// Deletes the log file
    /// </summary>
    public void Delete() => File.Delete(LogFile);

    /// <summary>
    /// Converts the log into the line
    /// </summary>
    /// <returns>The log in the string format</returns>
    public override string ToString() {
        string result = string.Empty;
        _data.ForEach(line => result += $"{line}\n");

        return result;
    }
}
