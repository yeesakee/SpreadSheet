// <copyright file="ExpressionTreeTests.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace ExpressionTreeTests
{
    using System.Reflection;
    using SpreadsheetEngine;

    /// <summary>
    /// Tests functions for the ExpressionTree class.
    /// </summary>
    public class ExpressionTreeTests
    {
        [Test]

        // Tests the ExpressionTree class with addition expressions.
        public void TestExpressionTreeAdd()
        {
            ExpressionTree expTree = new ExpressionTree("1+0+9");
            Assert.That(expTree.Evaluate(), Is.EqualTo(10));

            expTree = new ExpressionTree("A1+A2");
            expTree.SetVariable("A1", 99);
            expTree.SetVariable("A2", 1);
            Assert.That(expTree.Evaluate(), Is.EqualTo(100));
        }

        [Test]

        // Tests the ExpressionTree class with subtraction expressions.
        public void TestExpressionTreeSubtract()
        {
            ExpressionTree expTree = new ExpressionTree("100-50-0");
            Assert.That(expTree.Evaluate(), Is.EqualTo(50));

            expTree = new ExpressionTree("A1-A2");
            expTree.SetVariable("A1", 100);
            expTree.SetVariable("A2", 30);
            Assert.That(expTree.Evaluate(), Is.EqualTo(70));
        }

        [Test]

        // Tests the ExpressionTree class with multiplication expressions.
        public void TestExpressionTreeMultiply()
        {
            ExpressionTree expTree = new ExpressionTree("2*5*1");
            Assert.That(expTree.Evaluate(), Is.EqualTo(10));

            expTree = new ExpressionTree("A1*A2");
            expTree.SetVariable("A1", 3);
            expTree.SetVariable("A2", 5);
            Assert.That(expTree.Evaluate(), Is.EqualTo(15));
        }

        [Test]

        // Tests the ExpressionTree class with division expressions.
        public void TestExpressionTreeDivide()
        {
            ExpressionTree expTree = new ExpressionTree("10/2/5");
            Assert.That(expTree.Evaluate(), Is.EqualTo(1));

            expTree = new ExpressionTree("A1/A2");
            expTree.SetVariable("A1", 1);
            expTree.SetVariable("A2", 1);
            Assert.That(expTree.Evaluate(), Is.EqualTo(1));

            expTree.SetVariable("A1", 33);
            expTree.SetVariable("A2", 3);
            Assert.That(expTree.Evaluate(), Is.EqualTo(11));
        }

        [Test]

        // Test for add and subtract binary operators in one expression.
        public void TestExpressionTreeAddSubtract()
        {
            ExpressionTree expTree = new ExpressionTree("1+6-2+1");
            Assert.That(expTree.Evaluate(), Is.EqualTo(6));

            expTree = new ExpressionTree("A1+A2-B1");
            expTree.SetVariable("A1", 10);
            expTree.SetVariable("A2", 7);
            expTree.SetVariable("B1", 2);
            Assert.That(expTree.Evaluate(), Is.EqualTo(15));
        }

        [Test]

        // Test for multiplication and division binary operators in one expression.
        public void TestExpressionTreeMultiplyDivide()
        {
            ExpressionTree expTree = new ExpressionTree("2*8/4*5");
            Assert.That(expTree.Evaluate(), Is.EqualTo(20));

            expTree = new ExpressionTree("A1/A2*B1/B2");
            expTree.SetVariable("A1", 10);
            expTree.SetVariable("A2", 2);
            expTree.SetVariable("B1", 3);
            expTree.SetVariable("B2", 5);
            Assert.That(expTree.Evaluate(), Is.EqualTo(3));
        }

        [Test]

        // Test for all binary operators in one expression.
        public void TestExpressionTreeAllOperators()
        {
            ExpressionTree expTree = new ExpressionTree("2+5*(7-4)/3");
            Assert.That(expTree.Evaluate(), Is.EqualTo(7));
        }

        [Test]

        // Test for variables with long names.
        public void TestExpressionTreeLongVariableNames()
        {
            ExpressionTree expTree = new ExpressionTree("A32-bee11-cbde-e11e");
            expTree.SetVariable("A32", 1);
            expTree.SetVariable("bee11", 2);
            expTree.SetVariable("cbde", 3);
            expTree.SetVariable("e11e", 4);
            Assert.That(expTree.Evaluate(), Is.EqualTo(-8));
        }

        [Test]

        // Test for multiple parenthesis within eachother.
        public void TestExpressionTreeMultipleParenthesis()
        {
            ExpressionTree expTree = new ExpressionTree("5*(3*(2+1)/(10/(8-3))-5)");
            Assert.That(expTree.Evaluate(), Is.EqualTo(-2.5));
        }

        [Test]

        // Test for ConstructTree exception for invalid expression.
        public void TestConstructTreeException()
        {
            Exception exception = Assert.Throws<ArgumentException>(() => new ExpressionTree("A+"));
            Assert.That(exception.Message, Is.EqualTo("Invalid expression, binary operation must have at least 2 values to evaluate."));
        }

        [Test]

        // Test for ShuntingYard normal case.
        public void TestShuntingYardNormal()
        {
            ExpressionTree expTree = new ExpressionTree(string.Empty);
            MethodInfo? methodInfo = expTree.GetType().GetMethod("ShutingYard", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(methodInfo);
            if (methodInfo != null)
            {
                object[] expression = new object[1];
                expression[0] = "(A+B*(C-D))/E";
                List<string> result = (List<string>)methodInfo.Invoke(expTree, expression);
                string resultString = string.Join(string.Empty, result.ToArray());
                Assert.That(resultString, Is.EqualTo("ABCD-*+E/"));
            }
        }

        [Test]

        // Test for ShuntingYard edge case.
        public void TestShuntingYardEdge()
        {
            ExpressionTree expTree = new ExpressionTree(string.Empty);
            MethodInfo? methodInfo = expTree.GetType().GetMethod("ShutingYard", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(methodInfo);
            if (methodInfo != null)
            {
                object[] expression = new object[1];
                expression[0] = string.Empty;
                List<string> result = (List<string>)methodInfo.Invoke(expTree, expression);
                string resultString = string.Join(string.Empty, result.ToArray());
                Assert.That(resultString, Is.EqualTo(string.Empty));
            }
        }
    }
}