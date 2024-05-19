namespace Lab10.ProjectSchedule;
internal class Program {
    static void Main(string[] args) {
        string[][] tasks = [
            ["1", "0", "3", "2"],
            ["2", "1", "4", "3"],
            ["3", "1", "2", "4"],
            ["4", "2", "5", "3"],
            ["5", "3", "1", "2"],
            ["6", "3", "2", "3"],
            ["7", "4,5", "4", "2"],
            ["8", "6,7", "3", "2"],
        ];

        Scheduler scheduler = new();
        scheduler.Run(tasks);
    }
}
