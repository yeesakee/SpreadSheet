// <copyright file="AbstractNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System.Collections.Generic;

    /// <summary>
    /// Abstract Node class.
    /// </summary>
    internal abstract class AbstractNode
    {
        /// <summary>
        /// abstract evaluation method for node.
        /// </summary>
        /// <returns> returns the evaluation of the expression. </returns>
        public abstract double Eval();
    }
}
