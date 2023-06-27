// <copyright file="Cell.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Cell class that inherits from AbstractCell.
    /// </summary>
    public class Cell : AbstractCell
    {
        // stores all cell objects dependent on current cell.
        private HashSet<Cell> dependOnMe;

        // stores all cell objects that current cell depend on.
        private HashSet<Cell> iDependOn;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="rowIndex">Represents the number of rows to create.</param>
        /// <param name="columnIndex">Represents the number of column to create.</param>
        public Cell(int rowIndex, int columnIndex)
            : base(rowIndex, columnIndex)
        {
            this.dependOnMe = new HashSet<Cell>();
            this.iDependOn = new HashSet<Cell>();
        }

        /// <summary>
        /// adds given cell to dependOnMe list.
        /// </summary>
        /// <param name="cell"> cell to add. </param>
        public void AddDependent(Cell cell)
        {
            this.dependOnMe.Add(cell);
        }

        /// <summary>
        /// adds given cell to iDependOn list.
        /// </summary>
        /// <param name="cell"> cell to add. </param>
        public void AddDepended(Cell cell)
        {
            this.iDependOn.Add(cell);
        }

        /// <summary>
        /// returns list of dependOnMe cells.
        /// </summary>
        /// <returns> return list. </returns>
        public HashSet<Cell> GetDependents()
        {
            return this.dependOnMe;
        }

        /// <summary>
        /// returns list of iDependOn cells.
        /// </summary>
        /// <returns> return list. </returns>
        public HashSet<Cell> GetDepended()
        {
            return this.iDependOn;
        }


        /// <summary>
        /// Removes the given cell from the dependOnMe set.
        /// </summary>
        /// <param name="cell">the cell to be removed. </param>
        public void RemoveDependent(Cell cell)
        {
            this.dependOnMe.Remove(cell);
        }

        /// <summary>
        /// Clears all iDependOn cells from set.
        /// Remove curr cell from cells it depended on.
        /// </summary>
        public void ClearDepended()
        {
            foreach (Cell cell in this.iDependOn)
            {
                cell.RemoveDependent(this);
            }

            this.iDependOn.Clear();
        }

        /// <summary>
        /// Checkes whether given cell is a dependent of this cell.
        /// </summary>
        /// <param name="cell"> cell to check. </param>
        /// <returns> true if cell is a dependent, false otherwise. </returns>
        public bool CellIsDepended(Cell cell)
        {
            return this.iDependOn.Contains(cell);
        }
    }
}
