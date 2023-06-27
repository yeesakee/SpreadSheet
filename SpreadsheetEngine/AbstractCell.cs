// <copyright file="AbstractCell.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

#pragma warning disable SA1306 // Field names should begin with lower-case letter

namespace SpreadsheetEngine
{
    using System.ComponentModel;

    /// <summary>
    /// Abstract cell class that represents a single cell.
    /// </summary>
    public abstract class AbstractCell : INotifyPropertyChanged
    {
        // stores the actual text typed into the cell
        protected string text = string.Empty;

        // stores evaluation of text typed into the cell
        protected string value = string.Empty;

        private readonly int rowIndex;
        private readonly int columnIndex;

        // stores the color of the cell.
        private uint bgColor = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCell"/> class.
        /// </summary>
        /// <param name="rowIndex">Represents the number of rows to create.</param>
        /// <param name="columnIndex">Represents the number of column to create.</param>
        public AbstractCell(int rowIndex, int columnIndex)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets rowindex integer value.
        /// </summary>
        public int RowIndex
        {
            get { return this.rowIndex; }
        }

        /// <summary>
        /// Gets columnindex integer value.
        /// </summary>
        public int ColumnIndex
        {
            get { return this.columnIndex; }
        }

        /// <summary>
        /// Gets or sets 'text' string value.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                // set text to new value only if new value is different from current value.
                if (this.text != value)
                {
                    this.text = value;

                    // Notify that the 'Text' property has changed.
                    this.NotifyPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Gets 'value' string value.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }

            // only allows current class to access setter
            internal set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.NotifyPropertyChanged("Value");
                }
            }
        }

        /// <summary>
        /// Gets or sets 'bgColor' string value.
        /// </summary>
        public uint BGColor
        {
            get
            {
                return this.bgColor;
            }

            set
            {
                if (this.bgColor != value)
                {
                    this.bgColor = value;

                    // Notify that the 'BGColor' property has changed.
                    this.NotifyPropertyChanged("BGColor");
                }
            }
        }

        /// <summary>
        /// Called by Set accessor of each property to notify property change.
        /// </summary>
        /// <param name="propertyName">optional properyName parameter of the caller.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
