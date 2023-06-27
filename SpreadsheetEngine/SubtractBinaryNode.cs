// <copyright file="SubtractBinaryNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    /// <summary>
    /// Represents subtraction for binary operation.
    /// </summary>
    internal class SubtractBinaryNode : BinaryOperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractBinaryNode"/> class.
        /// </summary>
        public SubtractBinaryNode()
        {
            this.Operator = '-';
        }

        /// <summary>
        /// return the subtraction evaluation of Node.
        /// </summary>
        /// <returns> returns subtraction of value.</returns>
        public override double Eval()
        {
            return this.Left.Eval() - this.Right.Eval();
        }
    }
}
