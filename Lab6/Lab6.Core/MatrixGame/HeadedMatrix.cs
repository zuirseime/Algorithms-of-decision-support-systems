namespace Lab6.Core.MatrixGame;
public class HeadedMatrix(Matrix data, string[] columns, string[] rows) : Matrix(data.Data) {
    public string[] Columns { get; set; } = columns;
    public string[] Rows { get; set; } = rows;
}
