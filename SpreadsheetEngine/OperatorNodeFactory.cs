// <copyright file="OperatorNodeFactory.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Creates an operator node based on evaluation.
    /// </summary>
    internal class OperatorNodeFactory
    {
        private Dictionary<char, Type> operators = new Dictionary<char, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNodeFactory"/> class.
        /// </summary>
        public OperatorNodeFactory()
        {
            this.TraverseAvailableOperators((op, type) => this.operators.Add(op, type));
        }

        private delegate void OnOperator(char op, Type type);

        /// <summary>
        /// Returns a new Binary node type based on given op.
        /// </summary>
        /// <param name="op"> operator given. </param>
        /// <returns> binary node type based on op. </returns>
        public OperatorNode CreateOperatorNode(char op)
        {
            if (this.operators.ContainsKey(op))
            {
                object operatorNodeObject = System.Activator.CreateInstance(this.operators[op]);
                if (operatorNodeObject is OperatorNode)
                {
                    return (OperatorNode)operatorNodeObject;
                }
            }

            return null;
        }

        private void TraverseAvailableOperators(OnOperator onOperator)
        {
            Type operatorNodeType = typeof(OperatorNode);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // get all operator class types that are a subclass of OperatorNode.
                IEnumerable<Type> operatorTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));
                foreach (var type in operatorTypes)
                {
                    PropertyInfo operatorField = type.GetProperty("Operator", BindingFlags.Instance | BindingFlags.Public);
                    if (operatorField != null && !type.GetTypeInfo().IsAbstract)
                    {
                        object value = operatorField.GetValue(Activator.CreateInstance(type));
                        if (value is char)
                        {
                            char operatorSymbol = (char)value;
                            onOperator(operatorSymbol, type);
                        }
                    }
                }
            }
        }
    }
}
