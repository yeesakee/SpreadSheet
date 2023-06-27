// <copyright file="NodeFactory.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System;

    /// <summary>
    /// Creates a ValueNode or VariableNode based on evaluation.
    /// </summary>
    internal class NodeFactory
    {
        /// <summary>
        /// Returns a variable or value node based on given val. null if invalid.
        /// </summary>
        /// <param name="val"> value for node. </param>
        /// <returns> variable or value node, null if invalid value given. </returns>
        public AbstractNode CreateNode(string val)
        {
            if (char.IsLetter(val[0]))
            {
                return new VariableNode(val);
            }
            else if (char.IsDigit(val[0]))
            {
                return new ValueNode(Convert.ToDouble(val));
            }

            return null;
        }
    }
}
