using matrix_project.Enums;
using matrix_project.Extensions;
using matrix_project.Interfaces;
using matrix_project.Models;
using matrix_project.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



/// <summary>
/// Main program class demonstrating matrix operations.
/// </summary>
class Program
{
    /// <summary>
    /// Main entry point for the matrix operation demonstration.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    static async Task Main()
    {
        var services = new ServiceCollection();

        // Register all strategies and context through extension
        services.AddMatrixOperations<double>();
        services.AddLogging(config => config.AddConsole());

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Program>>();
        var context = provider.GetRequiredService<IMatrixOperationContext<double>>();

        logger.LogInformation("Generating matrix {Rows}x{Cols}", 200, 200);
        var data = MatrixUtils.GenerateRandomMatrix<double>(200, 200, 0, 9);
        IMatrix<double> matrix = new Matrix<double>(200, 200, (r, c) => data[r, c]);

        MatrixUtils.PrintMatrix(matrix);
        Console.WriteLine(new string('-', 30));

        foreach (var op in new[] { MatrixOperation.Rotate90, MatrixOperation.Transpose })
        {
            logger.LogInformation("Starting operation {Operation}", op);
            var sw = Stopwatch.StartNew();
            var result = await context.ExecuteOperationAsync(op, matrix);
            sw.Stop();
            MatrixUtils.PrintMatrix(result);
            logger.LogInformation("Operation {Operation} completed in {Elapsed} ms", op, sw.ElapsedMilliseconds);
            Console.WriteLine($"Execution time {op}: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine(new string('-', 30));
        }
    }
}








