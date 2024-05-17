namespace Lab9.HungarianMethod;

public static class ArrayExtensions {
    public static T[] GetRow<T>(this T[,] matrix, int row) =>
        Enumerable.Range(0, matrix.GetLength(1)).Select(x => matrix[row, x]).ToArray();
    public static T[] GetColumn<T>(this T[,] matrix, int col) =>
        Enumerable.Range(0, matrix.GetLength(0)).Select(y => matrix[y, col]).ToArray();

    public static T MinInRow<T>(this T[,] matrix, int row) => GetRow(matrix, row).Min()!;
    public static T MinInColumn<T>(this T[,] matrix, int col) => GetColumn(matrix, col).Min()!;

    public static int CountInRow<T>(this T[,] matrix, T value, int row) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(1)).Count(col => matrix[row, col].CompareTo(value) == 0);
    public static int CountInColumn<T>(this T[,] matrix, T value, int col) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(0)).Count(row => matrix[row, col].CompareTo(value) == 0);

    public static int CountInRow<T>(this T[,] matrix, Func<T, bool> predicate, int row) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(1)).Count(col => predicate(matrix[row, col]));
    public static int CountInColumn<T>(this T[,] matrix, Func<T, bool> predicate, int col) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(0)).Count(row => predicate(matrix[row, col]));

    public static bool Contains<T>(this T[,] matrix, T value) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(0)).Any(r => 
            Enumerable.Range(0, matrix.GetLength(1)).Any(c => 
                matrix[r, c].CompareTo(value) == 0));

    public static bool Any<T>(this T[,] matrix, Func<T, bool> predicate) =>
        matrix.Cast<T>().Any(predicate);

    public static T Min<T>(this T[,] matrix) => matrix.Cast<T>().Min()!;
    public static T Min<T>(this T[,] matrix, Func<T, bool> predicate) => 
        matrix.Cast<T>().Where(item => predicate(item)).Min()!;
}