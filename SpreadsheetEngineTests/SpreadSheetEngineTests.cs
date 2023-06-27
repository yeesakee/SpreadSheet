// <copyright file="SpreadSheetEngineTests.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

// Every test includes a comment summary but warning does not disappear.
#pragma warning disable SA1600 // Elements should be documented

namespace SpreadsheetEngineTests
{
    using System;
    using System.IO;
    using SpreadsheetEngine;

    /// <summary>
    /// Tests for the SpreadSheetEngine project.
    /// </summary>
    public class SpreadsheetEngineTests
    {

        // values for bad values in a cell.
        private string badReference = "!(bad reference)";
        private string selfReference = "!(self reference)";
        private string errorEvaluating = "!(error)";
        private string circularReference = "!(circular reference)";

        [Test]

        // Test the constructor for the Cell class.
        public void TestCellConstructor()
        {
            Cell c1 = new Cell(30, 35);
            Assert.That(c1.RowIndex, Is.EqualTo(30));
            Assert.That(c1.ColumnIndex, Is.EqualTo(35));
        }

        [Test]

        // Test setter and getter of Text property.
        public void TestCellText()
        {
            Cell c1 = new Cell(30, 35);
            c1.Text = "Text";
            Assert.That(c1.Text, Is.EqualTo("Text"));
        }

        [Test]

        // Test the constructor for the Spreadsheet class.
        public void TestSpreadsheetConstructor()
        {
            Spreadsheet sp = new Spreadsheet(20, 30);
            Assert.That(sp.RowCount, Is.EqualTo(20));
            Assert.That(sp.ColumnCount, Is.EqualTo(30));
        }

        [Test]

        // Test the CellPropertyChanged Event when value is empty.
        public void TestCellPropertyChangedEmpty()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            AbstractCell cellA = sp.GetCell(0, 0);
            AbstractCell cellB = sp.GetCell(0, 1);

            // tests that all cell values are empty when initialized.
            Assert.That(cellA.Text, Is.EqualTo(string.Empty));
            Assert.That(cellB.Text, Is.EqualTo(string.Empty));

            // tests that cellA's value changes the empty value at given formula
            cellA.Text = "This is Cell A1";
            cellA.Text = "=B1";
            Assert.That(cellA.Text, Is.EqualTo("=B1"));
            Assert.That(cellA.Value, Is.EqualTo("0"));
        }

        [Test]

        // Test the CellPropertyChanged Event when value is changed to a formula.
        public void TestCellPropertyChangedFormula()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            AbstractCell cellA = sp.GetCell(0, 0);
            AbstractCell cellB = sp.GetCell(0, 1);

            // tests that cellA's value changes to the value at the given formula
            cellB.Text = "This is Cell B1";
            cellA.Text = "=B1";
            Assert.That(cellB.Text, Is.EqualTo("This is Cell B1"));
            Assert.That(cellA.Text, Is.EqualTo("=B1"));
            Assert.That(cellA.Value, Is.EqualTo("This is Cell B1"));
        }

        [Test]

        // Test the CellPropertyChanged Event for Invalid Formula
        public void TestCellPropertyChangedInvalidFormula()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            AbstractCell cellA = sp.GetCell(0, 0);

            // tests that cellA's value does not change given invalid formula
            cellA.Text = "=B3";
            Assert.That(cellA.Text, Is.EqualTo("=B3"));
            Assert.That(cellA.Value, Is.EqualTo("0"));
        }

        [Test]

        // Test that if a cell's value is changed, all cell values that dependend on it also updates.
        public void TestCellDependentUpdate()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            AbstractCell cellA = sp.GetCell(0, 0);
            AbstractCell cellB = sp.GetCell(0, 1);

            // set cellA's value first to depend on B
            cellA.Text = "=B1";

            // change cellB's value to formula "=10-7"
            cellB.Text = "=10-7";
            Assert.That(cellA.Value, Is.EqualTo("3"));
            Assert.That(cellB.Value, Is.EqualTo("3"));
        }

        [Test]

        // Test that if a cell's formula is invalid (adding strings) then cell's value
        // will update to INVALID
        public void TestCellInvalidFormula()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            AbstractCell cellA = sp.GetCell(0, 0);
            AbstractCell cellB = sp.GetCell(0, 1);

            // set cellA's value to depend on B
            cellA.Text = "=B1+3";

            // change cellB's value to string "hello"
            cellB.Text = "hello";
            Assert.That(cellA.Value, Is.EqualTo(this.errorEvaluating));
            Assert.That(cellB.Value, Is.EqualTo("hello"));
        }

        [Test]

        // Test for multiple dependent cells and all cell values are updated
        public void TestMultipleDependentCells()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            AbstractCell cellA = sp.GetCell(0, 0);
            AbstractCell cellB = sp.GetCell(0, 1);
            AbstractCell cellC = sp.GetCell(0, 2);
            AbstractCell cellD = sp.GetCell(0, 3);

            // set cellA's value first to depend on B1, C1, D1
            cellA.Text = "=(B1-C1)*D1"; // (10-5)*-2
            cellB.Text = "=3+7";
            cellC.Text = "=9-4";
            cellD.Text = "-2";
            Assert.That(cellA.Value, Is.EqualTo("-10"));
        }

        [Test]

        // Test for undo boundary case
        public void TestUndoBoundary()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            try
            {
                sp.Undo();
            }
            catch
            {
                Assert.Fail("Expected no exceptions");
            }
        }

        [Test]

        // Test for undo and redo edge case, one change
        public void TestUndoRedoEdge()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell a = (Cell)sp.GetCell(0, 0);
            TextCommand tc = new TextCommand("123", string.Empty, a);
            a.Text = "123";
            sp.AddUndo(tc);
            Assert.That(a.Value, Is.EqualTo("123"));
            sp.Undo();
            Assert.That(a.Value, Is.EqualTo(string.Empty));
            sp.Redo();
            Assert.That(a.Value, Is.EqualTo("123"));
        }

        [Test]

        // Test for multiple undos and redos
        public void TestMultipleCellUndoRedo()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell a = (Cell)sp.GetCell(0, 0);
            Cell b = (Cell)sp.GetCell(0, 1);
            Cell c = (Cell)sp.GetCell(0, 2);
            List<Cell> l = new List<Cell>();
            List<uint> uints = new List<uint>();
            l.Add(b);
            uints.Add(0);

            TextCommand tc1 = new TextCommand("123", string.Empty, a);
            ColorCommand tc2 = new ColorCommand(0xFFFFFFFF, uints, l);
            TextCommand tc3 = new TextCommand("555", string.Empty, c);
            a.Text = "123";
            b.BGColor = 0xFFFFFFFF;
            c.Text = "555";
            sp.AddUndo(tc1);
            sp.AddUndo(tc2);
            sp.AddUndo(tc3);
            Assert.That(a.Value, Is.EqualTo("123"));
            sp.Undo();
            Assert.That(c.Value, Is.EqualTo(string.Empty));
            sp.Undo();
            Assert.That(b.BGColor, Is.EqualTo(0));
            sp.Undo();
            Assert.That(a.Value, Is.EqualTo(string.Empty));
            sp.Redo();
            Assert.That(a.Value, Is.EqualTo("123"));
        }

        [Test]

        // Test for multiple undo and redo on a single cell.
        public void TestSingleCellUndoRedo()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell a = (Cell)sp.GetCell(0, 0);
            TextCommand tc1 = new TextCommand("123", string.Empty, a);
            TextCommand tc2 = new TextCommand("456", "123", a);
            TextCommand tc3 = new TextCommand("789", "456", a);
            a.Text = "789";
            sp.AddUndo(tc1);
            sp.AddUndo(tc2);
            sp.AddUndo(tc3);
            Assert.That(a.Value, Is.EqualTo("789"));
            sp.Undo();
            Assert.That(a.Value, Is.EqualTo("456"));
            sp.Redo();
            Assert.That(a.Value, Is.EqualTo("789"));
            sp.Undo();
            Assert.That(a.Value, Is.EqualTo("456"));
            sp.Undo();
            Assert.That(a.Value, Is.EqualTo("123"));
        }

        [Test]

        // Test that after loading spreadsheet is cleared
        public void TestClearAfterLoad()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(1, 1);
            c1.Text = "Hi";
            c1.Text = "There";
            sp.Undo();

            sp.LoadFile("../../../ExampleXML/b.xml");
            Assert.That(c1.Text, Is.EqualTo(string.Empty));
        }

        [Test]

        // Test Load functionality with single cell and fake attributes
        public void TestLoadSingleCell()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            sp.LoadFile("../../../ExampleXML/b.xml");
            Cell c1 = (Cell)sp.GetCell(0, 0);
            Cell c2 = (Cell)sp.GetCell(1, 1);
            Assert.That(c1.Text, Is.EqualTo("b"));
            Assert.That(c1.Value, Is.EqualTo("b"));
            Assert.That(c1.BGColor, Is.EqualTo(0));
            Assert.That(c2.Text, Is.EqualTo(string.Empty));
            Assert.That(c2.BGColor, Is.EqualTo(0));
        }

        [Test]

        // Test Load functionality with multiple cells and text/color
        public void TestLoadMultipleCells()
        {
            Spreadsheet sp = new Spreadsheet(50, 26);
            sp.LoadFile("../../../ExampleXML/abc.xml");
            Cell c1 = (Cell)sp.GetCell(0, 0);
            Cell c2 = (Cell)sp.GetCell((int)'G' - 65, 2);
            Assert.That(c1.Text, Is.EqualTo("aaa"));
            Assert.That(c1.Value, Is.EqualTo("aaa"));

            Assert.That(c2.Text, Is.EqualTo(string.Empty));
            Assert.That(c2.Value, Is.EqualTo(string.Empty));
            Assert.That(c2.BGColor, Is.EqualTo(Convert.ToUInt32("FFFFFF80", 16)));
        }

        [Test]

        // Test cell value is not corrupted with cell references
        public void TestLoadCellReference()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            Cell c2 = (Cell)sp.GetCell(0, 1);
            Cell c3 = (Cell)sp.GetCell(0, 2);

            c1.Text = "=B1+C1";
            c2.Text = "3";
            c3.Text = "7";

            Assert.That(c1.Value, Is.EqualTo("10"));

            sp.SaveFile("../../../ExampleXML/reference.xml");
            c1.Text = "0";
            c2.Text = "0";
            c3.Text = "0";

            sp.LoadFile("../../../ExampleXML/reference.xml");
            Assert.That(c1.Value, Is.EqualTo("10"));
            Assert.That(c2.Value, Is.EqualTo("3"));
            Assert.That(c3.Value, Is.EqualTo("7"));
        }

        [Test]

        // Test saving empty spreadsheet
        public void TestSavingEmpty()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            sp.SaveFile("../../../ExampleXML/empty.xml");
            string text = File.ReadAllText("../../../ExampleXML/empty.xml");
            Assert.That(text, Is.EqualTo("<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet />"));
        }

        [Test]

        // Test Save functionality with single cell
        public void TestSaveSingleCell()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            c1.Text = "Hi";
            c1.BGColor = Convert.ToUInt32("FFFFFF80", 16);

            sp.SaveFile("../../../ExampleXML/singleCell.xml");

            c1.Text = string.Empty;
            c1.BGColor = 0;

            sp.LoadFile("../../../ExampleXML/singleCell.xml");

            Assert.That(c1.Text, Is.EqualTo("Hi"));
            Assert.That(c1.Value, Is.EqualTo("Hi"));
            Assert.That(c1.BGColor, Is.EqualTo(Convert.ToUInt32("FFFFFF80", 16)));
        }

        [Test]

        // Test Save functionality with multple cell
        public void TestSaveMultipleCells()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            Cell c2 = (Cell)sp.GetCell(1, 1);
            c1.Text = "Hi";
            c1.BGColor = Convert.ToUInt32("FFFFFF80", 16);
            c2.Text = "Hi2";
            c2.BGColor = Convert.ToUInt32("FFFFFF81", 16);

            sp.SaveFile("../../../ExampleXML/multipleCells.xml");

            c1.Text = string.Empty;
            c1.BGColor = 0;
            c2.Text = string.Empty;
            c2.BGColor = 0;

            sp.LoadFile("../../../ExampleXML/multipleCells.xml");

            Assert.That(c1.Text, Is.EqualTo("Hi"));
            Assert.That(c1.Value, Is.EqualTo("Hi"));
            Assert.That(c1.BGColor, Is.EqualTo(Convert.ToUInt32("FFFFFF80", 16)));

            Assert.That(c2.Text, Is.EqualTo("Hi2"));
            Assert.That(c2.Value, Is.EqualTo("Hi2"));
            Assert.That(c2.BGColor, Is.EqualTo(Convert.ToUInt32("FFFFFF81", 16)));
        }

        [Test]

        // test for bad reference.
        public void TestBadReference()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            c1.Text = "=B1000";
            Assert.That(c1.Value, Is.EqualTo(this.badReference));
        }

        [Test]

        // test for self reference.
        public void TestSelfReference()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            c1.Text = "=A1";
            Assert.That(c1.Value, Is.EqualTo(this.selfReference));
        }

        [Test]

        // test for direct circular reference.
        public void TestDirectCircularReference()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            Cell c2 = (Cell)sp.GetCell(0, 1);
            c1.Text = "=B1";
            c2.Text = "=A1";
            Assert.That(c1.Value, Is.EqualTo(this.circularReference));
            Assert.That(c2.Value, Is.EqualTo(this.circularReference));
        }

        [Test]

        // test for indirect circular reference.
        public void TestIndirectCircularReference()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            Cell c2 = (Cell)sp.GetCell(0, 1);
            Cell c3 = (Cell)sp.GetCell(1, 1);
            Cell c4 = (Cell)sp.GetCell(1, 0);
            c1.Text = "=B1";
            c2.Text = "=B2";
            c3.Text = "=A2";
            c4.Text = "=A1";

            Assert.That(c1.Value, Is.EqualTo(this.circularReference));
            Assert.That(c2.Value, Is.EqualTo(this.circularReference));
            Assert.That(c3.Value, Is.EqualTo(this.circularReference));
            Assert.That(c4.Value, Is.EqualTo(this.circularReference));
        }

        [Test]

        // test for direct reference to a cell with an error.
        public void TestDirectReferenceError()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            Cell c2 = (Cell)sp.GetCell(0, 1);
            c1.Text = "=B100";
            c2.Text = "=A1";
            Assert.That(c1.Value, Is.EqualTo(this.badReference));
            Assert.That(c2.Value, Is.EqualTo(this.errorEvaluating));
        }

        [Test]

        // test for indirect reference to a cell with an error.
        public void TestIndirectReferenceError()
        {
            Spreadsheet sp = new Spreadsheet(5, 5);
            Cell c1 = (Cell)sp.GetCell(0, 0);
            Cell c2 = (Cell)sp.GetCell(0, 1);
            Cell c3 = (Cell)sp.GetCell(1, 0);
            c1.Text = "=B100";
            c2.Text = "=A2";
            c3.Text = "=A1";
            Assert.That(c1.Value, Is.EqualTo(this.badReference));
            Assert.That(c2.Value, Is.EqualTo(this.errorEvaluating));
            Assert.That(c3.Value, Is.EqualTo(this.errorEvaluating));
        }
    }
}