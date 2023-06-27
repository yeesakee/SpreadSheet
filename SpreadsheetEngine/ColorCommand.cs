// <copyright file="ColorCommand.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Spreadsheet_Yeesa_Kee;

    /// <summary>
    /// ColorCommand object that represents a color change for a.multiple cell.
    /// </summary>
    public class ColorCommand : Command
    {
        // new color to change to
        private uint newColor;

        // old colors corresponding to cell
        private List<uint> oldColor;
        private List<Cell> cell;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorCommand"/> class.
        /// </summary>
        /// <param name="newColor"> new color that was changed to. </param>
        /// <param name="oldColor"> old color before change. </param>
        /// /// <param name="cell"> cell being changed. </param>
        public ColorCommand(uint newColor, List<uint> oldColor, List<Cell> cell)
        {
            this.newColor = newColor;
            this.oldColor = oldColor;
            this.cell = cell;
        }

        /// <summary>
        /// Execute undo cell color change.
        /// </summary>
        public override void Execute()
        {
            for (int i = 0; i < this.cell.Count; i++)
            {
                this.cell[i].BGColor = this.oldColor[i];
            }
        }

        /// <summary>
        /// Execute redo cell color change.
        /// </summary>
        public override void UnExecute()
        {
            foreach (Cell cell in this.cell)
            {
                cell.BGColor = this.newColor;
            }
        }
    }
}
