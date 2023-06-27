// <copyright file="BinaryOperatorNode.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    internal abstract class BinaryOperatorNode : OperatorNode
    {
        // Binary operators must have 2 values to evaluate (hence binary).
        private AbstractNode left;
        private AbstractNode right;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperatorNode"/> class.
        /// </summary>
        public BinaryOperatorNode()
        {
            this.left = null;
            this.right = null;
        }

        /// <summary>
        /// Gets or sets the left node.
        /// </summary>
        public AbstractNode Left
        {
            get { return this.left; }
            set { this.left = value; }
        }

        /// <summary>
        /// Gets or sets the right node.
        /// </summary>
        public AbstractNode Right
        {
            get { return this.right; }
            set { this.right = value; }
        }

        /// <summary>
        /// Abstract override method for Eval.
        /// </summary>
        /// <returns> returns evaluated value. </returns>
        public abstract override double Eval();
    }
}
