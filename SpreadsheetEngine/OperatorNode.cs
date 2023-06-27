// <copyright file="OperatorNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    internal abstract class OperatorNode : AbstractNode
    {
        /// <summary>
        /// Gets or sets operator value.
        /// </summary>
        public char Operator { get; set; }

        public override abstract double Eval();
    }
}
