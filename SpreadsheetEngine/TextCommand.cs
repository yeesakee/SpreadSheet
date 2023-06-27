// <copyright file="TextCommand.cs" company="Yeesa Kee 11640325">
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
    /// TextCommand object that represents a text change for a cell.
    /// </summary>
    public class TextCommand : Command
    {
        private string newText;
        private string oldText;
        private Cell cell;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCommand"/> class.
        /// </summary>
        /// <param name="newText"> new color that was changed to. </param>
        /// <param name="oldText"> old color before change. </param>
        /// /// <param name="cell"> cell being changed. </param>
        public TextCommand(string newText, string oldText, Cell cell)
        {
            this.newText = newText;
            this.oldText = oldText;
            this.cell = cell;
        }

        /// <summary>
        /// Execute undo cell text change.
        /// </summary>
        public override void Execute()
        {
            this.cell.Text = this.oldText;
        }

        /// <summary>
        /// Execute redo cell text change.
        /// </summary>
        public override void UnExecute()
        {
            this.cell.Text = this.newText;
        }
    }
}
