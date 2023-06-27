// <copyright file="DivideBinaryNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    /// <summary>
    /// Represents division for binary operation.
    /// </summary>
    internal class DivideBinaryNode : BinaryOperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivideBinaryNode"/> class.
        /// </summary>
        public DivideBinaryNode()
        {
            this.Operator = '/';
        }

        /// <summary>
        /// return the division evaluation of Node.
        /// </summary>
        /// <returns> returns division value.</returns>
        public override double Eval()
        {
            return this.Left.Eval() / this.Right.Eval();
        }
    }
}
