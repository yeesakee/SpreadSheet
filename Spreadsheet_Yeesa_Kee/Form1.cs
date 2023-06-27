// <copyright file="Form1.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace Spreadsheet_Yeesa_Kee
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using SpreadsheetEngine;

    /// <summary>
    /// Form1 class for the Form1 object.
    /// </summary>
    public partial class Form1 : Form
    {
        private Spreadsheet spreadsheet;
        private int rowSize;
        private int columnSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// Form1 constructor.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
            this.InitializeDataGrid();
            this.rowSize = 50;
            this.columnSize = 26;
            this.spreadsheet = new Spreadsheet(this.rowSize, this.columnSize);
            this.spreadsheet.PropertyChanged += this.CellPropertyChanged;
            this.dataGridView1.CellBeginEdit += this.DataGridView1_CellBeginEdit;
            this.dataGridView1.CellEndEdit += this.DataGridView1_CellEndEdit;
        }

        // PropertyChangedEventHandler for form.
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes the datagrid with columns.
        /// </summary>
        public void InitializeDataGrid()
        {
            // ASCII value for 'A'
            int col = (int)'A';

            // add columns from A - Z
            for (int i = 0; i < 26; i++)
            {
                char col_name = (char)(col + i);
                DataGridViewColumn dataGridViewColumn = new DataGridViewColumn();
                dataGridViewColumn.HeaderText = col_name.ToString();
                dataGridViewColumn.CellTemplate = new DataGridViewTextBoxCell();
                this.dataGridView1.Columns.Add(dataGridViewColumn);
            }

            // add 50 rows and label them from 1 - 50
            for (int i = 1; i <= 50; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = i.ToString();
                this.dataGridView1.Rows.Add(row);
            }
        }

        /// <summary>
        /// Called when a Cell's value changes.
        /// </summary>
        /// <param name="sender"> object that was changed. </param>
        /// <param name="e"> Property that was changed. </param>
        public void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // if a cell's text is changed
            if (e.PropertyName == "Text")
            {
                Cell cell = (Cell)sender;
                this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;
                this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Text"));
            }

            // if a cell's color is changed
            else if (e.PropertyName == "BGColor")
            {
                Cell cell = (Cell)sender;
                uint color = cell.BGColor;
                if (color == 0)
                {
                    this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Style.BackColor = Color.Empty;
                }
                else
                {
                    this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Style.BackColor = Color.FromArgb(unchecked((int)color));
                }

                this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("BGColor"));
            }

            // undo/redo changes
            else
            {
                if (e.PropertyName.Contains("Activate"))
                {
                    string text = string.Empty;
                    if (sender is TextCommand)
                    {
                        text = "Cell Text Change";
                    }
                    else if (sender is ColorCommand)
                    {
                        text = "Cell Background Color Change";
                    }

                    // activating undo or redo buttons with text or color change
                    if (e.PropertyName.Contains("Undo"))
                    {
                        this.undoToolStripMenuItem.Text = "Undo " + text;
                        this.undoToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        this.redoToolStripMenuItem.Text = "Redo " + text;
                        this.redoToolStripMenuItem.Enabled = true;
                    }
                }

                // deactivating undo or redo button
                else if (e.PropertyName.Contains("Deactivate"))
                {
                    if (e.PropertyName.Contains("Undo"))
                    {
                        this.undoToolStripMenuItem.Text = "Undo";
                        this.undoToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        this.redoToolStripMenuItem.Text = "Redo";
                        this.redoToolStripMenuItem.Enabled = false;
                    }
                }

                // if a cell's text is changed
                else if (e.PropertyName == "!(circular reference)")
                {
                    Cell cell = (Cell)sender;
                    this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;
                    this.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("!(circular reference)"));
                }
            }
        }

        /// <summary>
        /// Change the displayed value in the clicked cell to the cell object's Text value.
        /// </summary>
        /// <param name="sender"> object being changed. </param>
        /// <param name="e"> information on the datagrid selected. </param>
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            AbstractCell cell = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
            this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Text;
        }

        /// <summary>
        /// Change the displayed value in the finished editing cell to the cell object's Value value.
        /// </summary>
        /// <param name="sender"> object being changed. </param>
        /// <param name="e"> information on the datagrid selected. </param>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Cell cell = (Cell)this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);

            string oldText = cell.Text;
            var newValue = this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value;
            string newText = string.Empty;
            if (newValue != null)
            {
                newText = newValue.ToString();
            }

            if (oldText != newText)
            {
                // add the new TextCommand of the cell change to stack.
                this.spreadsheet.AddUndo(new TextCommand(newText, oldText, cell));
                cell.Text = newText;
            }
            else
            {
                this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;
            }
        }

        /// <summary>
        /// Shows a color dialog and changes selected cells to user's chosen color.
        /// </summary>
        /// <param name="sender"> object being changed. </param>
        /// <param name="e"> information on the datagrid selected. </param>
        private void ChangeBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colordiag = new ColorDialog();
            colordiag.AllowFullOpen = false;

            // if user selects "OK" button, then update changes.
            if (colordiag.ShowDialog() == DialogResult.OK)
            {
                this.ChangeSelectedCellsBackgroundColor(colordiag.Color);
            }
        }

        /// <summary>
        /// Helper function to change all selected cell's background color.
        /// </summary>
        /// <param name="cellBackgroundColor"> color to change to. </param>
        private void ChangeSelectedCellsBackgroundColor(Color cellBackgroundColor)
        {
            int selectedCellCount = this.dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
            List<uint> oldColors = new List<uint>();
            List<Cell> cells = new List<Cell>();
            uint newColor = (uint)cellBackgroundColor.ToArgb();

            for (int i = 0; i < selectedCellCount; i++)
            {
                int row = this.dataGridView1.SelectedCells[i].RowIndex;
                int col = this.dataGridView1.SelectedCells[i].ColumnIndex;
                uint oldColor = (uint)this.dataGridView1.Rows[row].Cells[col].Style.BackColor.ToArgb();
                Cell cell = (Cell)this.spreadsheet.GetCell(row, col);

                // add to list of old colors and corresponding cells.
                oldColors.Add(oldColor);
                cells.Add(cell);

                cell.BGColor = newColor;
                this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Style.BackColor = cellBackgroundColor;
            }

            ColorCommand currCommand = new ColorCommand(newColor, oldColors, cells);

            // add the new ColorCommand to the undo stack.
            this.spreadsheet.AddUndo(currCommand);
        }

        /// <summary>
        /// Called when undo button is clicked from the menu bar.
        /// </summary>
        /// <param name="sender"> object selected. </param>
        /// <param name="e"> eventargs. </param>
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Undo();
        }

        /// <summary>
        /// Called when redo button is clicked from the menu bar.
        /// </summary>
        /// <param name="sender"> object selected. </param>
        /// <param name="e"> eventargs. </param>
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Redo();
        }

        /*                                 *** DEMO PORTION ***                                   */

        /// <summary>
        /// Demonstrates a Demo of the CellPropertyChanged Event.
        /// </summary>
        /// <param name="sender"> object that is being activated. </param>
        /// <param name="e"> Event that is called. </param>
        private void DemoButton_Click(object sender, System.EventArgs e)
        {
            this.Demo();
        }

        /// <summary>
        /// Creates a Demo that demonstrates the CellPropertyChanged EventHandler.
        /// </summary>
        private void Demo()
        {
            Random ran = new Random();
            int size = 50;
            int alpha_size = 25;

            // set a value to 50 random cells
            for (int i = 0; i < size; i++)
            {
                int row = ran.Next(1, size - 1);
                int col = ran.Next(0, alpha_size);
                AbstractCell cell = this.spreadsheet.GetCell(row, col);
                cell.Text = ":)";
            }

            // set all values in column B to be "This is cell B#"
            // with # being the row number.
            for (int j = 1; j <= size; j++)
            {
                // set col to 1 because that is column B
                AbstractCell cell = this.spreadsheet.GetCell(j - 1, 1);
                cell.Text = "This is cell B" + j.ToString();
            }

            // set all values in column A to be the value in the corresponding
            // row of column B.
            for (int j = 1; j <= size; j++)
            {
                // set col to 0 because that is column A
                AbstractCell cell = this.spreadsheet.GetCell(j - 1, 0);
                cell.Text = "=B" + j.ToString();
            }
        }

        /// <summary>
        /// Prompts user to save current spreadsheet as a file.
        /// </summary>
        /// <param name="sender"> object sender. </param>
        /// <param name="e"> eventargs e. </param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // filter so file can only be saved as XML
            saveFileDialog.Filter = "XML files (*.xml) | *.xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.spreadsheet.SaveFile(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Prompts user to select a file to load into spreadsheet.
        /// </summary>
        /// <param name="sender"> object sender. </param>
        /// <param name="e"> eventargs e. </param>
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.spreadsheet.LoadFile(openFileDialog.FileName);
            }
        }
    }
}
