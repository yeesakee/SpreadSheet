// <copyright file="ExpressionTree.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;

    public class ExpressionTree
    {
        private AbstractNode root;
        private Dictionary<string, VariableNode> variableDic;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression"> the inputed expression to be evaluated. </param>
        public ExpressionTree(string expression)
        {
            this.variableDic = new Dictionary<string, VariableNode>();
            List<string> postfixExpression = this.ShutingYard(expression);
            this.root = this.ConstructTree(postfixExpression);
        }

        /// <summary>
        /// Set the given variable name to the given value.
        /// </summary>
        /// <param name="variableName"> name of the variable. </param>
        /// <param name="variableValue"> value (double) of the variable .</param>
        public void SetVariable(string variableName, double variableValue)
        {
            if (this.variableDic.ContainsKey(variableName))
            {
                this.variableDic[variableName].Value = variableValue;
            }
        }

        /// <summary>
        /// returns a list of all variables in the dictionary.
        /// </summary>
        /// <returns> list of variables. </returns>
        public List<string> GetVariables()
        {
            List<string> variables = new List<string>(this.variableDic.Keys);
            return variables;
        }

        /// <summary>
        /// Evaluates the given expression and returns the value.
        /// </summary>
        /// <returns> Returns the value of the given expression. </returns>
        public double Evaluate()
        {
            return this.root.Eval();
        }

        /// <summary>
        /// Convert infix notation of given expression to postfix.
        /// </summary>
        /// <param name="expression"> expression to be converted. </param>
        /// <returns> Returns expression in postfix order. </returns>
        private List<string> ShutingYard(string expression)
        {
            List<string> result = new List<string>();
            Stack<char> opstack = new Stack<char>();

            for (int i = 0; i < expression.Length; i++)
            {
                char curr = expression[i];

                // if curr is a digit or a letter, add it to the result string.
                if (this.IsDigitLetter(curr))
                {
                    string variableName = curr.ToString();
                    int n = 1;
                    while ((i + n) < expression.Length && this.IsDigitLetter(expression[i + n]))
                    {
                        variableName += expression[i + n];
                        n++;
                    }

                    i += n - 1;
                    result.Add(variableName);
                }

                // if curr is '(' push it to the operator stack.
                else if (curr == '(')
                {
                    opstack.Push(curr);
                }

                // if curr is ')' pop all contents form stack and add to result string.
                else if (curr == ')')
                {
                    while (opstack.Count != 0 && opstack.Peek() != '(')
                    {
                        result.Add(opstack.Pop().ToString());
                    }

                    // pop the '(' from the stack
                    opstack.Pop();
                }

                // if curr is an operator, add to stack depending on precedence.
                else
                {
                    int currOp = this.OperatorPrecedence(curr);
                    while (opstack.Count != 0 &&
                        this.OperatorPrecedence(opstack.Peek()) >= currOp)
                    {
                        result.Add(opstack.Peek().ToString());
                        opstack.Pop();
                    }

                    opstack.Push(curr);
                }
            }

            // pop all operators off stack and add to result string.
            while (opstack.Count != 0)
            {
                result.Add(opstack.Pop().ToString());
            }

            return result;
        }

        /// <summary>
        /// Returns whether the given character is a letter or digit.
        /// </summary>
        /// <param name="ch"> character to check. </param>
        /// <returns> Returns true if ch is a letter or digit, false otherwise. </returns>
        private bool IsDigitLetter(char ch)
        {
            return char.IsLetter(ch) || char.IsDigit(ch);
        }

        /// <summary>
        /// Contains the operator precedence when evaluating formulas.
        /// </summary>
        /// <param name="c"> given operator. </param>
        /// <returns>
        ///     returns the precedence (higher more precedene).
        ///     -1 if not an operator accepted.
        /// </returns>
        private int OperatorPrecedence(char c)
        {
            switch (c)
            {
                case '+': case '-': return 1;
                case '*': case '/': return 2;
                default: return -1;
            }
        }

        /// <summary>
        /// ConstructTree method that takes a postfixExpression and create an expression tree.
        /// </summary>
        /// <param name="postfixExpression"> postfix expression to be evaluated into expression tree. </param>
        /// <returns> Returns root node of expression tree. </returns>
        private AbstractNode ConstructTree(List<string> postfixExpression)
        {
            if (postfixExpression.Count == 0)
            {
                return null;
            }

            Stack<AbstractNode> operatorNodes = new Stack<AbstractNode>();
            for (int i = 0; i < postfixExpression.Count; i++)
            {
                OperatorNodeFactory opnf = new OperatorNodeFactory();
                OperatorNode opNode = opnf.CreateOperatorNode(postfixExpression[i][0]);

                // If node IS an operatorNode
                if (opNode != null)
                {
                    // check if it is binary operator node, since more operator nodes
                    // can be implemented in the future.
                    if (opNode is BinaryOperatorNode)
                    {
                        BinaryOperatorNode bopNode = (BinaryOperatorNode)opNode;

                        // binary operator must have two values, if stack has less than 2 values then
                        // binary expression given is invalid.
                        if (operatorNodes.Count >= 2)
                        {
                            bopNode.Right = operatorNodes.Pop();
                            bopNode.Left = operatorNodes.Pop();
                            operatorNodes.Push(bopNode);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid expression, binary operation must have at least 2 values to evaluate.");
                        }
                    }
                }

                // If node IS NOT an operatorNode (is a variable/value node).
                else
                {
                    NodeFactory nf = new NodeFactory();
                    AbstractNode abNode = nf.CreateNode(postfixExpression[i]);
                    if (abNode is VariableNode)
                    {
                        // check if the variable is in the dictionary, if not add it.
                        if (!this.variableDic.ContainsKey(postfixExpression[i]))
                        {
                            VariableNode vbNode = (VariableNode)abNode;
                            this.variableDic.Add(postfixExpression[i], vbNode);
                        }

                        operatorNodes.Push(this.variableDic[postfixExpression[i]]);
                    }
                    else
                    {
                        operatorNodes.Push(abNode);
                    }
                }
            }

            return operatorNodes.Pop();
        }
    }
}
