// <copyright file="Command.cs" company="Yeesa Kee 11640325">
// Copyright (c) Yeesa Kee 11640325. All rights reserved.
// </copyright>

namespace Spreadsheet_Yeesa_Kee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class Command
    {
        /// <summary>
        /// Execute command.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Redo executed command.
        /// </summary>
        public abstract void UnExecute();
    }
}
