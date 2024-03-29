﻿using Lab2.Core.Input;
using Lab2.Core.Mathematics;
using Lab2.Core.Output;

namespace Lab2.ConsoleApp;

internal class Program {
    static void Main(string[] args) {
        string constaintsStr = """
            x1+x2-x3-2x4<=6
            x1+x2+x3-x4>=5
            2x1-x2+3x3+4x4<=10
            """;
        string funcStr = "x1+2x2-x3-x4";

        string[] constaintsArray = constaintsStr.Split('\n');
        List<Inequality> constaintsList = [];
        Array.ForEach(constaintsArray, relation => constaintsList.Add(Inequality.Parse(relation)));
        Inequality[] constraints = [.. constaintsList];

        Function func = Function.Parse(funcStr);

        int rows = constraints.Length;
        int cols = 5;

        double[,] table = new double[rows + 1, cols];

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                table[row, col] = col != cols - 1 ? constraints[row].Coefficients[col] : constraints[row].Constant;
            }
        }

        for (int col = 0; col < cols - 1; col++) {
            table[rows, col] = func.Coefficients[col];
        }
        table[rows, cols - 1] = 0;

        SimplexAlgrorithm simplex = new();
        simplex.Run(table, constaintsStr, funcStr, true);

        Log.Instance.Save();
        Log.Instance.Show();
    }
}
