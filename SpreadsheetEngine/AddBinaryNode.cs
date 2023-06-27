// <copyright file="AddBinaryNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    /// <summary>
    /// Represents addition for binary operation.
    /// </summary>
    internal class AddBinaryNode : BinaryOperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddBinaryNode"/> class.
        /// </summary>
        public AddBinaryNode()
        {
            this.Operator = '+';
        }

        /// <summary>
        /// return the addition evaluation of Node.
        /// </summary>
        /// <returns> returns addition of value.</returns>
        public override double Eval()
        {
            return this.Left.Eval() + this.Right.Eval();
        }
    }
}
