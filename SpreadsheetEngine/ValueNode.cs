// <copyright file="ValueNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    internal class ValueNode : AbstractNode
    {
        // stores the value of the node.
        private double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueNode"/> class.
        /// </summary>
        /// <param name="value"> takes a double value for the node. </param>
        public ValueNode(double value)
        {
            this.value = value;
        }

        /// <summary>
        /// Evaluates the node (just returns the value).
        /// </summary>
        /// <returns> Returns the value of the node. </returns>
        public override double Eval()
        {
            return this.value;
        }
    }
}
