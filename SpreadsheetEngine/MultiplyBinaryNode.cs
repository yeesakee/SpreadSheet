// <copyright file="MultiplyBinaryNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    /// <summary>
    /// Represents multiplication for binary operation.
    /// </summary>
    internal class MultiplyBinaryNode : BinaryOperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplyBinaryNode"/> class.
        /// </summary>
        public MultiplyBinaryNode()
        {
            this.Operator = '*';
        }

        /// <summary>
        /// return the multiplication evaluation of Node.
        /// </summary>
        /// <returns> returns multiplication value.</returns>
        public override double Eval()
        {
            return this.Left.Eval() * this.Right.Eval();
        }
    }
}
