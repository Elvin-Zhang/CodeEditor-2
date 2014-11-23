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
    [DesignTimeVisible(false)]
    public class CodeTextBox : RichTextBox
    {
        #region External

        [DllImport("user32.dll")]
        static extern bool GetCaretPos(out Point pnt);

        #endregion

        #region Constants

        Point empty = Point.Empty;
        Color color = Color.FromArgb(230, 230, 230);
        ControlStyles style = ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the absolute position
        /// of the richtextbox-caret.
        /// </summary>
        public Point CaretPoint
        {
            get
            {
                // We call a windows API function in order to
                // get the abolute position of the caret. If it
                // failed, return an empty point.
                return GetCaret();
            }
        }

        /// <summary>
        /// The text we actually see with
        /// our eyes in the richtextbox.
        /// </summary>
        public string DisplayedText
        {
            get
            {
                // We calculate the first and the last visible
                // character in the textbox and return a substring
                // of the whole text in the richtextbox.
                if (TextLength != 0)
                {
                    int startpos = GetStartCharPos();
                    int endpos = GetLastCharPos();
                    int diff = (endpos - startpos) + 1;

                    return Text.Substring(startpos, diff);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the point and returns it to property
        /// </summary>
        private Point GetCaret()
        {
            var pt = new Point();
            if (GetCaretPos(out pt))
            {
                return pt;
            }
            else
            {
                return empty;
            }
        }

        /// <summary>
        /// Retrieve the first char by passing the top-left
        /// coordinate in the IndexFromPosition-function
        /// </summary>
        private int GetStartCharPos()
        {
            var upperleft = new Point(ClientRectangle.Left, 
                ClientRectangle.Top + (FontHeight / 2));
            return GetCharIndexFromPosition(upperleft);
        }

        /// <summary>
        /// Retrieve the last char by passing the bottom-right
        /// coordinate in the IndexFromPosition-function
        /// </summary>
        private int GetLastCharPos()
        {
            var lowerright = new Point(ClientRectangle.Right,
                ClientRectangle.Bottom);
            return GetCharIndexFromPosition(lowerright);
        }

        /// <summary>
        /// Gets the last word written.
        /// </summary>
        public string LastWord
        {
            get
            {
                return lastWord;
            }
        }

        /// <summary>
        /// Private last word
        /// </summary>
        private string lastWord;

        #endregion

        #region Constructor

        // In order to guarantee smooth drawing, we
        // enable double-buffering and ignore ERASEBKGND
        public CodeTextBox()
        {
            SetStyle(style, true);
        }

        #endregion

        #region Overriding

        protected override void OnTextChanged(EventArgs e)
        {
            char bksp = ('\b');
            char enter = ('\n');
            char space = (' ');
            int caret = this.SelectionStart;

            if (TextLength != 0)
            {
                if (caret != 0)
                {
                    var text = this.Text;
                    var data = text[caret - 1];

                    // Loops back until it hits a whitespace or a return;
                    // So we only get the currently written word.
                    if (data != bksp && data != space && data != enter)
                    {
                        int back = caret;
                        var subs = text[caret - 1];

                        while (caret > 0 && subs != space && subs != enter)
                        {
                            // Decrease position and updates
                            // the 'subs' character for loop.
                            caret -= 1; subs = text[caret];
                        } caret++;

                        // We finally receive the last wort through subbing
                        // the text via our currently calculated 'caret' and
                        // the length which is simply the old position minus the new.
                        lastWord = text.Substring(caret, (back - caret));
                    }
                    else
                    {
                        lastWord = string.Empty;
                    }
                }
            }

            // We should not forget calling the base method,
            // or we receive a loss of correct functioning.
            base.OnTextChanged(e);
        }

        #endregion
    }
}