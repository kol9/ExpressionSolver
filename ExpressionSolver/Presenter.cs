using System;

namespace ExpressionSolver
{
    internal static class Presenter
    {
        private static void Main()
        {
            using var solver = new ParallelSolver();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("==========================================================");
            Console.WriteLine("===         Expression Solver has been launched        ===");
            Console.WriteLine("===____________________________________________________===");
            Console.WriteLine("=== To evaluate an expression, write it to the console ===");
            Console.WriteLine("===                  and press Enter.                  ===");
            Console.WriteLine("===----------------------------------------------------===");
            Console.WriteLine("===       If you want to close Expression Solver,      ===");
            Console.WriteLine("===      write 'q' to the console, and press Enter     ===");
            Console.WriteLine("==========================================================");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("                                                          ");
            Console.ResetColor();


            while (true)
            {
                Console.Write("Enter expression>>> ");
                var request = Console.ReadLine();
                if (request == "q")
                {
                    Console.ResetColor();
                    break;
                }

                var (answer, expression, errors) = solver.SendAndEval(request);

                if (errors.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("Calculated Result: ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(answer);
                    Console.WriteLine();
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Errors occurred during the calculation,");
                    Console.Write("so the following expression was calculated: ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(expression.ToString());
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("Calculated Result: ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(answer);
                    Console.WriteLine();
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(errors.Count + " ERROR" + ((errors.Count == 1) ? ": " : "S: "));
                    Console.ResetColor();

                    foreach (var error in errors)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine(error.Message + error.ErrorDescription());
                        Console.ResetColor();
                    }
                }

                Console.WriteLine("==========================================================");
            }
        }
    }
}