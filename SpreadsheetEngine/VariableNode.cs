// <copyright file="VariableNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System;

    /// <summary>
    /// Represents a variable.
    /// </summary>
    internal class VariableNode : AbstractNode
    {
        private string name;
        private double value; // null value if user did not set value.

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="name"> given variable name. </param>
        /// <param name="value"> given variable value, default null. </param>
        public VariableNode(string name, double value = 0)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets string name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Sets double value.
        /// </summary>
        public double Value
        {
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Evaluates the node, returns the value of the node.
        /// If value not set, throw an ArgumentNullException.
        /// </summary>
        /// <returns> returns the value of the node. </returns>
        public override double Eval()
        {
            return this.value;
        }
    }
}
