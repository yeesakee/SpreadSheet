// <copyright file="Spreadsheet.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml;
    using Spreadsheet_Yeesa_Kee;
    using SpreadsheetEngine;

    /// <summary>
    /// Spreadsheet class. Acts as a container for 2D array of Cell objects.
    /// </summary>
    public class Spreadsheet
    {
        // 2D array, stores all the cells created
        private Cell[,] spreadsheet;
        private int rows;
        private int columns;

        // stack stores commands that will undo
        private Stack<Command> undo;

        // stack stores commands that will undo
        private Stack<Command> redo;

        // values for bad values in a cell.
        private string badReference = "!(bad reference)";
        private string selfReference = "!(self reference)";
        private string errorEvaluating = "!(error)";
        private string circularReference = "!(circular reference)";

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rows"> number of rows to create. </param>
        /// <param name="columns"> number of columns to create. </param>
        public Spreadsheet(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            this.spreadsheet = new Cell[rows, columns];
            this.undo = new Stack<Command>();
            this.redo = new Stack<Command>();

            // fill the 2d array with new Cell objects
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this.spreadsheet[i, j] = new Cell(i, j);
                    this.spreadsheet[i, j].PropertyChanged += this.CellPropertyChanged;
                }
            }
        }

        // PropertyChangedEventHandler for Spreadsheet class.
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets number of columns.
        /// </summary>
        public int ColumnCount
        {
            get { return this.columns; }
        }

        /// <summary>
        /// Gets number of columns.
        /// </summary>
        public int RowCount
        {
            get { return this.rows; }
        }

        /// <summary>
        /// returns the Cell object at the given row and column.
        /// index start at 0.
        /// </summary>
        /// <param name="row"> int row of cell. </param>
        /// <param name="column"> int col of cell. </param>
        /// <returns> returns the Cell object. </returns>
        public AbstractCell GetCell(int row, int column)
        {
            // check that given row and column index are within range of spreadsheet
            if (row < 0 || column < 0 || row >= this.rows || column >= this.columns)
            {
                return null;
            }

            return this.spreadsheet[row, column];
        }

        /// <summary>
        /// Called when the a Cell's property is changed. Notifies of the Cell's change.
        /// </summary>
        /// <param name="sender"> object that was changed. </param>
        /// <param name="e"> Property that was changed. </param>
        public void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell cell = (Cell)sender;
            if (e.PropertyName == "Text")
            {
                // remove current cell from cells that it use to depend on.
                cell.ClearDepended();
                if (cell.Text != string.Empty && cell.Text[0] == '=')
                {
                    this.EvaluateFormula(cell);
                }
                else
                {
                    cell.Value = cell.Text;
                }

                if (cell.Value == this.circularReference)
                {
                    e = new PropertyChangedEventArgs(this.circularReference);
                }

                foreach (Cell dependent in cell.GetDependents().ToList())
                {
                    this.CellPropertyChanged(dependent, e);
                    this.PropertyChanged?.Invoke(dependent, e);
                }

                this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Text"));
            }
            else if (e.PropertyName == "BGColor")
            {
                this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("BGColor"));
            }
            else if (e.PropertyName == this.circularReference)
            {
                cell.Value = this.circularReference;

                // get all cells depending on current cell and change their values
                // to circular reference error.
                foreach (Cell dependent in cell.GetDependents().ToList())
                {
                    if (dependent.Value != this.circularReference)
                    {
                        this.CellPropertyChanged(dependent, e);
                        this.PropertyChanged?.Invoke(dependent, e);
                    }
                }
            }
        }

        /// <summary>
        /// Add given command to undo stack.
        /// </summary>
        /// <param name="command"> command to add to undo. </param>
        public void AddUndo(Command command)
        {
            this.undo.Push(command);

            // since a new command is added, clear redo stack.
            this.ClearRedo();
            this.PropertyChanged?.Invoke(command, new PropertyChangedEventArgs("Activate Undo"));
        }

        /// <summary>
        /// Undo-es the last command.
        /// </summary>
        public void Undo()
        {
            // check if stack is empty
            if (this.undo.Count != 0)
            {
                Command currCommand = this.undo.Pop();
                if (this.undo.Count == 0)
                {
                    this.PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Deactivate Undo"));
                }
                else
                {
                    // send the next undo command (this.undo.Peek())
                    this.PropertyChanged?.Invoke(this.undo.Peek(), new PropertyChangedEventArgs("Activate Undo"));
                }

                this.PropertyChanged?.Invoke(currCommand, new PropertyChangedEventArgs("Activate Redo"));
                this.redo.Push(currCommand);
                currCommand.Execute();
            }
        }

        /// <summary>
        /// Redo-es the last undo.
        /// </summary>
        public void Redo()
        {
            // check if stack is empty
            if (this.redo.Count != 0)
            {
                Command currCommand = this.redo.Pop();
                if (this.redo.Count == 0)
                {
                    this.PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Deactivate Redo"));
                }
                else
                {
                    // send the next redo command (this.undo.Peek())
                    this.PropertyChanged?.Invoke(this.redo.Peek(), new PropertyChangedEventArgs("Activate Redo"));
                }

                this.undo.Push(currCommand);
                this.PropertyChanged?.Invoke(currCommand, new PropertyChangedEventArgs("Activate Undo"));
                currCommand.UnExecute();
            }
        }

        /// <summary>
        /// Given a XML file, load the spreadsheet accordingly.
        /// </summary>
        /// <param name="filePath"> path of file to load. </param>
        public void LoadFile(string filePath)
        {
            this.ClearSpreadsheet();
            XmlTextReader textReader = new XmlTextReader(filePath);
            while (textReader.Read())
            {
                if (textReader.NodeType == XmlNodeType.Element && textReader.Name == "cell")
                {
                    string cellName = textReader.GetAttribute("name");
                    int row = (int)cellName[0] - 65;
                    int col = int.Parse(cellName.Substring(1)) - 1;
                    AbstractCell currCell = this.GetCell(row, col);

                    while (textReader.Read() && currCell != null)
                    {
                        var a = textReader.NodeType;
                        if (textReader.NodeType == XmlNodeType.EndElement && textReader.Name == "cell")
                        {
                            break;
                        }
                        else
                        {
                            if (textReader.Name == "text")
                            {
                                textReader.Read();
                                currCell.Text = textReader.Value;
                                textReader.Read();
                            }

                            if (textReader.Name == "bgcolor")
                            {
                                textReader.Read();
                                currCell.BGColor = Convert.ToUInt32(textReader.Value, 16);
                                textReader.Read();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves current changes to spreadsheet to XML file.
        /// </summary>
        /// <param name="fileName"> name of the saved file. </param>
        public void SaveFile(string fileName)
        {
            XmlWriter xmlWriter = XmlWriter.Create(fileName);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("spreadsheet");

            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    Cell cell = (Cell)this.GetCell(i, j);
                    if (cell.Text != string.Empty || cell.BGColor != 0)
                    {
                        char row = (char)(i + 65);
                        string col = (j + 1).ToString();
                        string name = row + col;

                        // start writing current element cell
                        xmlWriter.WriteStartElement("cell");
                        xmlWriter.WriteAttributeString("name", name);
                        if (cell.Text != string.Empty)
                        {
                            xmlWriter.WriteStartElement("text");
                            xmlWriter.WriteString(cell.Text);
                            xmlWriter.WriteEndElement();
                        }

                        if (cell.BGColor != 0)
                        {
                            xmlWriter.WriteStartElement("bgcolor");
                            xmlWriter.WriteString(cell.BGColor.ToString("X"));
                            xmlWriter.WriteEndElement();
                        }

                        // end writing current element cell
                        xmlWriter.WriteEndElement();
                    }
                }
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }

        /// <summary>
        /// Clears the spreadsheet by undoing all changes.
        /// Also clears undo and redo button.
        /// </summary>
        private void ClearSpreadsheet()
        {
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.columns; j++)
                {
                    Cell cell = (Cell)this.GetCell(i, j);
                    cell.Text = string.Empty;
                    cell.BGColor = 0;
                }
            }

            this.ClearUndo();
            this.ClearRedo();
        }

        /// <summary>
        /// Clears the undo stack.
        /// </summary>
        private void ClearUndo()
        {
            this.undo.Clear();
            this.PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Deactivate Undo"));
        }

        /// <summary>
        /// Clears the redo stack.
        /// </summary>
        private void ClearRedo()
        {
            this.redo.Clear();
            this.PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Deactivate Redo"));
        }


        /// <summary>
        /// Recursively check if there is a circular reference in all cells
        /// dependent on the given cell.
        /// </summary>
        /// <param name="cell"> cell to check for circular reference. </param>
        /// <param name="referencedCells"> list of cells that is dependent on cell. </param>
        /// <returns> true if there is a circular reference, false otherwise. </returns>
        private bool CheckCircularReferenceHelper(Cell cell, Cell currCell, List<Cell> referencedCells)
        {
            referencedCells.Add(currCell);
            if (referencedCells.Contains(cell))
            {
                return true;
            }

            foreach (Cell dependentCell in currCell.GetDepended())
            {
                if (!referencedCells.Contains(dependentCell))
                {
                    bool result = this.CheckCircularReferenceHelper(cell, dependentCell, referencedCells);
                    if (result)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether given cell object is in a circular reference.
        /// </summary>
        /// <param name="cell"> cell to check for circular reference. </param>
        /// <returns> true if circular reference found, false otherwise. </returns>
        private bool CheckCircularReference(Cell cell)
        {
            // check each cell that given cell is dependent on for circular reference
            foreach (Cell dependentCell in cell.GetDepended())
            {
                // check each cell for circular reference.
                bool result = this.CheckCircularReferenceHelper(cell, dependentCell, new List<Cell>());
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Evaluates the given formula stored.
        /// </summary>
        /// <param name="cell"> Cell object that is being evaluated. </param>
        private void EvaluateFormula(Cell cell)
        {
            // get substring of cell's text starting at index 1
            // (excludes the '=' sign in the formula)
            string text = cell.Text.Substring(1);
            ExpressionTree exp = new ExpressionTree(text);
            List<string> variables = exp.GetVariables();
            bool checkOnlyVairable = false;
            bool badReferenceFound = false;
            bool selfReferenceFound = false;
            bool invalidFormulaFound = false;

            // check whether the formula only contains one cell reference and nothing else
            // if so then change current cell's value to the cell referenced.
            // this is for the edge case of if the referenced cell value is a string value.
            if (variables.Count == 1)
            {
                // get the column header and convert it to integer value.
                // 64 is ASCII for A so A has index value of 0.
                int columnNumber = (int)cell.Text[1] - 65;

                // get the row number from the cell
                int rowNumber;
                checkOnlyVairable = int.TryParse(cell.Text.Substring(2), out rowNumber);
                Cell parentCell = (Cell)this.GetCell(rowNumber - 1, columnNumber);
                if (checkOnlyVairable)
                {
                    if (parentCell != null && parentCell != cell)
                    {
                        parentCell.AddDependent(cell);
                        cell.AddDepended(parentCell);
                        if (this.CheckCircularReference(cell))
                        {
                            cell.Value = this.circularReference;
                        }
                        else
                        {
                            double value;
                            bool success_parse = double.TryParse(parentCell.Value, out value);

                            // if parentCell's value is valid double value or
                            // is a string, not equation (does not start with '=')
                            if (success_parse || (parentCell.Text != string.Empty && parentCell.Text[0] != '='))
                            {
                                cell.Value = parentCell.Value;
                            }

                            // if the parentCell's text is empty, set default value to 0
                            else if (parentCell.Text == string.Empty)
                            {
                                cell.Value = "0";
                            }
                            else
                            {
                                cell.Value = this.errorEvaluating;
                            }
                        }
                    }
                    else if (parentCell == null)
                    {
                        cell.Value = this.badReference;
                    }
                    else
                    {
                        cell.Value = this.selfReference;
                    }
                }
            }

            // if more than one variable/value is in the formula
            if (!checkOnlyVairable)
            {
                foreach (string variable in variables)
                {
                    int columnNumber = (int)variable[0] - 65;
                    int rowNumber;
                    int.TryParse(variable.Substring(1), out rowNumber);
                    Cell currCell = (Cell)this.GetCell(rowNumber - 1, columnNumber);

                    // check that referenced cell exists.
                    if (currCell != null)
                    {
                        // check that referenced cell is not itself.
                        if (currCell != cell)
                        {
                            // add the current cell to the dependent list of the variable cell.
                            currCell.AddDependent(cell);
                            cell.AddDepended(currCell);

                            double value;
                            bool success_parse = double.TryParse(currCell.Value, out value);

                            // if curCell's value is a double, then set the variable's value in the
                            // expression tree to the value.
                            if (success_parse)
                            {
                                exp.SetVariable(variable, value);
                            }

                            // if it is not a double, then value is invalid.
                            else
                            {
                                invalidFormulaFound = true;
                                cell.Value = this.errorEvaluating;
                                break;
                            }
                        }

                        // if currCell is the same as given cell, self reference found.
                        else
                        {
                            selfReferenceFound = true;
                            cell.Value = this.selfReference;
                            break;
                        }
                    }

                    // if cell at (row,col) does not exist, bad reference found.
                    else
                    {
                        badReferenceFound = true;
                        cell.Value = this.badReference;
                        break;
                    }
                }

                // if any errors found, evaluation is not needed.
                if (!badReferenceFound && !selfReferenceFound && !invalidFormulaFound)
                {
                    if (this.CheckCircularReference(cell))
                    {
                        cell.Value = this.circularReference;
                    }
                    else
                    {
                        // try to evaluate the formula
                        try
                        {
                            cell.Value = exp.Evaluate().ToString();
                        }

                        // if throws error, make cell value to error message.
                        catch
                        {
                            cell.Value = this.errorEvaluating;
                        }
                    }
                }
            }
        }
    }
}
