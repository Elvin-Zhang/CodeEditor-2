// Copyright (C) 2014 Niquo
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// ~~~~~~~~~~~~~~~~~~~~~~~ History ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// 23/11/2014:
// > Released the first version of the CodeEditor on CodeProject
//   and Github. It includes a fully functional syntax-highlighting,
//   an attempt of implementing intellisense and a new designed
//   listbox for the intellisense.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Scripting.Controls
{
    public class SyntaxHighlighting : IDisposable
    {
        #region Constants

        Color empty = Color.Empty;

        #endregion

        #region Variables

        private bool disposed;
        private string regex;

        private Font font;
        private Color forecolor;
        private Color backcolor;

        #endregion

        #region Properties

        /// <summary>
        /// The pattern, with which we want
        /// to search for specific chars
        /// </summary>
        [Category("Regex")]
        public string Regex
        {
            get
            {
                return regex;
            }
            set
            {
                regex = value;
            }
        }

        /// <summary>
        /// The font of the syntax-highlighting
        /// </summary>
        [Category("Font")]
        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
            }
        }

        /// <summary>
        /// The foreground-color of the syntax-highlighting
        /// </summary>
        [Category("Colors")]
        public Color ForeColor
        {
            get
            {
                return forecolor;
            }
            set
            {
                forecolor = value;
            }
        }

        /// <summary>
        /// The background-color of the syntax-highlighting
        /// </summary>
        [Category("Colors")]
        public Color BackColor
        {
            get
            {
                return backcolor;
            }
            set
            {
                backcolor = value;
            }
        }

        #endregion

        #region Constructor

        // We're having the 4 objects (regex, font, fore/backcolor)
        // in one structure to ensure readability and organization. 
        public SyntaxHighlighting()
        {
            this.font = new Font("Consolas", 9.75f);
            this.forecolor = Color.Black;
            this.backcolor = Color.White;
            this.regex = "";
            this.disposed = false;
        }

        #endregion

        #region Destructor

        ~SyntaxHighlighting()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other inheritors of IDisposable
                    // as they are not collected by the GC
                    this.font.Dispose();
                }

                // Set objects to null in order to
                // really clean up everything
                this.regex = null;
                this.forecolor = empty;
                this.backcolor = empty;
                this.disposed = true;
            }
        }

        #endregion
    }
}