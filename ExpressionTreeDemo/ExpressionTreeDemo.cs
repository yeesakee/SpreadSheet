// <copyright file="ExpressionTreeDemo.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace ExpressionTreeDemo
{
    using System;
    using SpreadsheetEngine;

    /// <summary>
    /// Demo's the ExpressionTree class.
    /// </summary>
    internal class ExpressionTreeDemo
    {
        /// <summary>
        /// Main method to run Demo.
        /// </summary>
        /// <param name="args"> ... </param>
        public static void Main(string[] args)
        {
            string expression = "A1+B2+C3";
            ExpressionTree expressionT = new ExpressionTree(expression);

            bool run = true;
            while (run)
            {
                Console.WriteLine("Menu (current expression: " + expression + ")");
                Console.WriteLine("    1: Enter a new expression");
                Console.WriteLine("    2: Set a variable value");
                Console.WriteLine("    3: Evaluate tree");
                Console.WriteLine("    4: Quit");
                Console.Write("Command: ");
                string input = Console.ReadLine();
                if (input != string.Empty && char.IsDigit(input[0]))
                {
                    int option = Convert.ToInt32(input);
                    switch (option)
                    {
                        case 1:
                            Console.Write("Enter new expression: ");
                            expression = Console.ReadLine();
                            expressionT = new ExpressionTree(expression);
                            break;
                        case 2:
                            Console.Write("Enter variable name: ");
                            string variable = Console.ReadLine();
                            Console.Write("Enter variable value: ");
                            string value = Console.ReadLine();
                            double val = Convert.ToDouble(value);
                            expressionT.SetVariable(variable, val);
                            break;
                        case 3:
                            Console.WriteLine("Evaluated Result: " + expressionT.Evaluate());
                            break;
                        case 4:
                            run = false;
                            break;
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
