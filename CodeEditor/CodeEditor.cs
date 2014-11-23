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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Scripting.Controls
{
    public class CodeEditor : UserControl
    {
        #region Override

        [Browsable(false)]
        public new string AccessibleDescription
        {
            get { return base.AccessibleDescription; }
        }

        [Browsable(false)]
        public new string AccessibleName
        {
            get { return base.AccessibleName; }
        }

        [Browsable(false)]
        public new AccessibleRole AccessibleRole
        {
            get { return base.AccessibleRole; }
        }

        [Browsable(false)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }

        [Browsable(false)]
        public new BorderStyle BorderStyle
        {
            get { return base.BorderStyle; }
            set { base.BorderStyle = value; }
        }

        [Browsable(false)]
        public override Cursor Cursor
        {
            get { return base.Cursor; }
            set { base.Cursor = value; }
        }

        [Category("Layout")]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        [Browsable(false)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        [Browsable(false)]
        public override RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; }
        }

        [Browsable(false)]
        public new bool UseWaitCursor
        {
            get { return base.UseWaitCursor; }
        }

        [Browsable(false)]
        public new ControlBindingsCollection DataBindings
        {
            get { return base.DataBindings; }
        }

        [Browsable(false)]
        public new Object Tag
        {
            get { return base.Tag; }
        }

        [Browsable(false)]
        public new bool CausesValidation
        {
            get { return base.CausesValidation; }
        }

        [Browsable(false)]
        public new bool AutoScroll
        {
            get { return base.AutoScroll; }
        }

        [Browsable(false)]
        public new Size AutoScrollMargin
        {
            get { return base.AutoScrollMargin; }
            set { base.AutoScrollMargin = value; }
        }

        [Browsable(false)]
        public new Size AutoScrollMinSize
        {
            get { return base.AutoScrollMinSize; }
            set { base.AutoScrollMinSize = value; }
        }

        [Browsable(false)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        [Browsable(false)]
        public new AutoSizeMode AutoSizeMode
        {
            get { return base.AutoSizeMode; }
        }

        [Browsable(false)]
        public new Padding Margin
        {
            get { return base.Margin; }
        }

        [Browsable(false)]
        public new Size MaximumSize
        {
            get { return base.MaximumSize; }
        }

        [Browsable(false)]
        public new Size MinimumSize
        {
            get { return base.MinimumSize; }
        }

        [Browsable(false)]
        public new Padding Padding
        {
            get { return base.Padding; }
        }

        [Browsable(false)]
        public override bool AllowDrop
        {
            get { return base.AllowDrop; }
            set { base.AllowDrop = value; }
        }

        [Browsable(false)]
        public new AutoValidate AutoValidate
        {
            get { return base.AutoValidate; }
        }

        [Browsable(false)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { base.ContextMenuStrip = value; }
        }

        [Browsable(false)]
        public new ImeMode ImeMode
        {
            get { return base.ImeMode; }
        }

        [Browsable(false)]
        public new int TabIndex
        {
            get { return base.TabIndex; }
            set { base.TabIndex = value; }
        }

        [Browsable(false)]
        public new bool TabStop
        {
            get { return base.TabStop; }
        }

        #endregion

        #region External

        [DllImport("user32.dll")]
        static extern bool LockWindowUpdate(IntPtr hWndLock);

        #endregion

        #region Constants

        const RegexOptions option = RegexOptions.Multiline;
        const AnchorStyles anchor = AnchorStyles.Bottom | AnchorStyles.Top
                | AnchorStyles.Left | AnchorStyles.Right;
        const ControlStyles style = ControlStyles.ResizeRedraw;
        const string whitespace = " ";

        #endregion

        #region Variables

        private bool intellisenseActive;
        private bool displayedOnTop;

        private CodeListBox intellisenseBox;
        private CodeTextBox codeTextBox;
        private List<SyntaxHighlighting> ruleSets;

        private Color lineColor;
        private Color lineBorderColor;
        private Color lineBackColor;

        private Color codeBackColor;
        private Color codeBorderColor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the regex rules
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The Syntax-Highlighting rules")]
        [Category("CodeEditor-Data")]
        public List<SyntaxHighlighting> RuleSets
        {
            get
            {
                return ruleSets;
            }
            set
            {
                ruleSets = value;
            }
        }

        /// <summary>
        /// Gets or sets the intellisense
        /// </summary>
        [Description("A list of autocompletewords and imageindexes")]
        [Category("CodeEditor-Data")]
        public Intellisense AutoComplete
        {
            get
            {
                return intellisenseBox.AutoComplete;
            }
            set
            {
                intellisenseBox.AutoComplete = value;
            }
        }

        /// <summary>
        /// Gets or sets the imagelist of the listbox
        /// </summary>
        [Description("The imagelist thats used by the intellisense")]
        [Category("CodeEditor-Data")]
        public ImageList ImageList
        {
            get
            {
                return intellisenseBox.ImageList;
            }
            set
            {
                intellisenseBox.ImageList = value;
            }
        }

        [Category("CodeListBox-Properties")]
        public Color SelectionBorderColor
        {
            get
            {
                return intellisenseBox.SelectionBorderColor;
            }
            set
            {
                intellisenseBox.SelectionBorderColor = value;
            }
        }

        [Category("CodeListBox-Properties")]
        public Color SelectionBackColor
        {
            get
            {
                return intellisenseBox.SelectionColor;
            }
            set
            {
                intellisenseBox.SelectionColor = value;
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Line between lines and textbox.")]
        public Color LineColor
        {
            get
            {
                return lineColor;
            }
            set
            {
                lineColor = value;
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Bordercolor of selected line.")]
        public Color LineBorderColor
        {
            get
            {
                return lineBorderColor;
            }
            set
            {
                lineBorderColor = value;
                this.Invalidate();
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Backcolor of selected line.")]
        public Color LineBackColor
        {
            get
            {
                return lineBackColor;
            }
            set
            {
                lineBackColor = value;
                this.Invalidate();
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Backcolor of the editor.")]
        public Color CodeBackColor
        {
            get
            {
                return codeBackColor;
            }
            set
            {
                codeBackColor = value;
                this.Invalidate();
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Bordercolor of the editor.")]
        public Color CodeBorderColor
        {
            get
            {
                return codeBorderColor;
            }
            set
            {
                codeBorderColor = value;
                this.Invalidate();
            }
        }

        #endregion

        #region Constructor

        // Constructs a new CodeEditor and sets various properties
        public CodeEditor()
        {
            // CodeEditor properties
            this.intellisenseActive = false;
            this.displayedOnTop = false;
            this.SetStyle(style, true);
            this.DoubleBuffered = true;
            this.Anchor = anchor;

            this.codeBorderColor = Color.FromArgb(160, 160, 160);
            this.codeBackColor = Color.FromArgb(244, 244, 244);
            this.lineBorderColor = Color.FromArgb(51, 153, 255);
            this.lineBackColor = Color.FromArgb(158, 206, 255);
            this.lineColor = Color.FromArgb(230, 230, 230);

            this.codeTextBox = new CodeTextBox();
            this.ruleSets = new List<SyntaxHighlighting>();

            // Predefines some variables
            var sze = new Size(Width - 53, Height - 2);
            var fnt = new Font("Consolas", 9.75f);
            var pnt = new Point(52, 1);
            var bds = BorderStyle.None;

            // ScriptBox properties
            this.codeTextBox.WordWrap = false;
            this.codeTextBox.AcceptsTab = true;
            this.codeTextBox.HideSelection = false;

            this.codeTextBox.Anchor = anchor;
            this.codeTextBox.Location = pnt;
            this.codeTextBox.Size = sze;

            this.codeTextBox.BorderStyle = bds;
            this.codeTextBox.Font = fnt;

            // CodeTextBox events
            this.codeTextBox.PreviewKeyDown += PreviewOnKey;
            this.codeTextBox.TextChanged += OnTextChange;
            this.codeTextBox.VScroll += OnTextScroll;
            this.codeTextBox.GotFocus += OnTextFocus;
            this.codeTextBox.Click += OnMouseClick;
            this.codeTextBox.KeyDown += OnKeyDown;

            // CodeListBox definitions
            this.intellisenseBox = new CodeListBox();
            this.intellisenseBox.KeyDown += OnMenuKey;
            this.intellisenseBox.Visible = false;
            this.codeTextBox.Controls.Add(intellisenseBox);

            // Other properties
            this.Controls.Add(codeTextBox);
            this.Font = fnt;
        }

        #endregion

        #region Codeevents

        // Hides the autocompletelist if you press escape or enter a new line.
        private void PreviewOnKey(object obj, PreviewKeyDownEventArgs arg)
        {
            // Variables (readability) =D
            var data = arg.KeyData;
            var enter = Keys.Enter;
            var esc = Keys.Escape;

            if (data == enter || data == esc)
            {
                // Hide the intellibox
                if (this.intellisenseActive == true)
                {
                    this.HideIntellisense();
                }
            }
        }

        // Updates and highlights the text, shows intellisense if word matches.
        private void OnTextChange(object obj, EventArgs arg)
        {
            try
            {
                int textlength = codeTextBox.TextLength;
                if (textlength != 0)
                {
                    // We have to lock various updates from
                    // our textbox, else we might receive
                    // a flickering or bugging control
                    LockWindowUpdate(this.codeTextBox.Handle);

                    var caretpos = this.codeTextBox.SelectionStart;
                    var length = this.codeTextBox.TextLength;
                    var foreclr = this.codeTextBox.ForeColor;
                    var scfont = this.codeTextBox.Font;

                    // Only edits the visible text, for performance improvements
                    string text = this.codeTextBox.DisplayedText;

                    // Resets the whole selection colors to prevent bugs
                    // when, for example, the return key has been pressed
                    this.codeTextBox.Select(0, length);
                    this.codeTextBox.SelectionColor = foreclr;
                    this.codeTextBox.SelectionFont = scfont;


                    foreach (var sh in ruleSets)
                    {
                        // Saves all the properties so we 
                        // don't have to access them all the time
                        var fore = sh.ForeColor;
                        var back = sh.BackColor;
                        var font = sh.Font;
                        var str = sh.Regex;

                        // Looks for any matches
                        var match = Regex.Match(text, str, option);
                        if (match.Success == true)
                        {
                            do
                            {
                                // Loops as long as there are matches
                                // and selects them via match.Index.
                                this.codeTextBox.Select(match.Index, match.Length);
                                this.codeTextBox.SelectionBackColor = back;
                                this.codeTextBox.SelectionColor = fore;
                                this.codeTextBox.SelectionFont = font;
                                match = match.NextMatch();
                            } while (match.Success == true);
                        }
                    }

                    // Now we reset the caret position,
                    // in preparation for intellisense.
                    this.codeTextBox.Select(caretpos, 0);

                    // Now we see if the current line is a PART
                    // or is the WHOLE of an autocomplete-word. 
                    if (this.codeTextBox.Lines.Length > 0)
                    {
                        if (AutoComplete != null)
                        {
                            // Receive the line number and gets the current
                            // line text by simply accessing the lines property
                            int linenumber = this.codeTextBox.GetLineFromCharIndex(caretpos);
                            var sublinetext = this.codeTextBox.Lines[linenumber];

                            if (sublinetext.Length > 0)
                            {
                                var founditems = new List<AutoCompleteWord>();
                                var escape = this.codeTextBox.LastWord;

                                if (!string.IsNullOrEmpty(escape))
                                {
                                    // Loop through all the intelliwords and
                                    // add to founditems if there is a match
                                    foreach (AutoCompleteWord word in AutoComplete.Items)
                                    {
                                        var itemword = word.ItemWord;
                                        if (itemword.Contains(escape))
                                        {
                                            founditems.Add(word);
                                        }
                                    }

                                    // If no items found, overjump this part
                                    // as its just unnecessary waste of performance
                                    var foundcount = founditems.Count;
                                    if (foundcount != 0)
                                    {
                                        // Clears out all the previous items and
                                        // adds the new, found items to the list
                                        this.intellisenseBox.Visible = true;
                                        this.intellisenseBox.Items.Clear();
                                        foreach (var auto in founditems)
                                        {
                                            this.intellisenseBox.Items.Add(auto);
                                        }

                                        var itemheight = this.intellisenseBox.MyItemHeight;
                                        var textheight = MeasureScriptBoxHeight();
                                        var caretpoint = this.codeTextBox.CaretPoint;
                                        var point = new Point(caretpoint.X,
                                            caretpoint.Y + textheight);
                                        var size = new Size(200, (8 + (itemheight * foundcount)));

                                        // If the intellibox is not fitting the space
                                        // just display it on the top!
                                        if (point.Y + size.Height > this.Height)
                                        {
                                            displayedOnTop = true;
                                            point.Y = (codeTextBox.Height - ((codeTextBox.Height - caretpoint.Y) + size.Height));
                                        }
                                        else
                                        {
                                            displayedOnTop = false;
                                        }

                                        this.intellisenseBox.Size = size;
                                        this.intellisenseBox.Location = point;

                                        // Finally adds it to our textbox
                                        this.intellisenseActive = true;
                                        founditems.Clear();
                                    }
                                    else
                                    {
                                        if (this.intellisenseActive == true)
                                        {
                                            this.HideIntellisense();
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.intellisenseActive == true)
                                    {
                                        this.HideIntellisense();
                                    }
                                }
                            }
                            else
                            {
                                if (this.intellisenseActive == true)
                                {
                                    this.HideIntellisense();
                                }
                            }
                        }
                        else
                        {
                            if (this.intellisenseActive == true)
                            {
                                this.HideIntellisense();
                            }
                        }
                    }
                    else
                    {
                        if (this.intellisenseActive == true)
                        {
                            this.HideIntellisense();
                        }
                    }
                }
                else
                {
                    if (this.intellisenseActive == true)
                    {
                        this.HideIntellisense();
                    }
                }
            }
            finally
            {
                // Unlock update for proper eventlogic
                // and redraw the whole CodeEditor
                LockWindowUpdate(IntPtr.Zero);
                Invalidate(true); Regex.CacheSize = 0x0;
            }
        }

        private void HideIntellisense()
        {
            this.intellisenseBox.Visible = false;
            this.intellisenseActive = false;
        }

        private int MeasureScriptBoxHeight()
        {
            return TextRenderer.MeasureText(whitespace, Font).Height;
        }

        // If scrolled, must update all the line-numbers.
        private void OnTextScroll(object obj, EventArgs arg)
        {
            this.Invalidate();
        }

        // We focus the intellibox, as soon as we press the down arrow.
        // Or if its displayed on top, the up arrow.
        private void OnKeyDown(object obj, KeyEventArgs arg)
        {
            var down = (displayedOnTop ? Keys.Up : Keys.Down);
            var data = arg.KeyData;
            var tab = Keys.Tab;
            var txt = ("    ");

            // Checks also for tab key - Why?
            // Because the indent of the normal tab
            // is too big imo, thus reducing it
            if (this.intellisenseActive && data == down)
            {
                this.intellisenseBox.SelectedIndex = 0;
                this.intellisenseBox.Focus();
            }
            else if (data == tab)
            {
                arg.SuppressKeyPress = true;
                this.codeTextBox.SelectedText = txt;
            }
        }

        // If the scriptbox receives focus, hide the intellibox.
        private void OnTextFocus(object obj, EventArgs arg)
        {
            if (this.intellisenseActive == true)
            {
                this.HideIntellisense();
            }
        }

        // If you click on the scriptbox, hide the intellibox.
        private void OnMouseClick(object obj, EventArgs arg)
        {
            if (this.intellisenseActive == true)
            {
                this.HideIntellisense();
            }
        }

        private void OnMenuKey(object obj, KeyEventArgs arg)
        {
            var ups = (displayedOnTop ? Keys.Down : Keys.Up);
            var data = arg.KeyData;
            var back = Keys.Back;
            var esc = Keys.Escape;
            var ent = Keys.Enter;

            if (data == esc || data == back)
            {
                this.codeTextBox.Focus();
            }
            else if (data == ups)
            {
                // Only hide the intellibox, if the FIRST
                // auto_item is selected -> else act normal
                int maxitems = (intellisenseBox.Items.Count - 1);
                int outindex = (displayedOnTop ? maxitems : 0);
                if (this.intellisenseBox.SelectedIndex == outindex)
                {
                    this.codeTextBox.Focus();
                }
            }
            else if (data == ent)
            {
                var item = (((AutoCompleteWord)intellisenseBox.SelectedItem).ItemWord);
                var array = this.codeTextBox.Lines;

                if (array.Length == 0)
                {
                    codeTextBox.SelectionStart = 0;
                    array = new string[1];
                }

                // Get the current line number
                int selection = codeTextBox.SelectionStart;
                int linenumber = codeTextBox.GetLineFromCharIndex(selection);
                selection += (item.Length - array[linenumber].Length);
                array[linenumber] = item;

                // Now copy all the lines back to the textbox
                this.codeTextBox.Lines = array;
                this.codeTextBox.SelectionStart = selection;
                this.codeTextBox.Focus();
            }
        }

        #endregion

        #region Codedrawing

        // Paints the background plus the border of the CodeEditor
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var border_rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
            var back_rect = new Rectangle(1, 1, base.Width - 2, base.Height - 2);
            var line_rect = new Rectangle(49, 1, base.Width - 50, base.Height - 2);
            var line_point = new Point(48, 1); var pnt2 = new Point(48, base.Height - 2);

            var line_brush = new SolidBrush(Color.White);
            var back_brush = new SolidBrush(codeBackColor);
            var border_pen = new Pen(codeBorderColor);
            var line_pen = new Pen(lineColor);

            e.Graphics.FillRectangle(back_brush, back_rect);
            e.Graphics.FillRectangle(line_brush, line_rect);
            e.Graphics.DrawRectangle(border_pen, border_rect);
            e.Graphics.DrawLine(line_pen, line_point, pnt2);

            back_brush.Dispose();
            border_pen.Dispose();
            line_pen.Dispose();
            line_brush.Dispose();
        }

        // Paints the linenumbers, their selection & more
        protected override void OnPaint(PaintEventArgs e)
        {
            int scriptheight = MeasureScriptBoxHeight();
            int visibleindex = 0; var pos = new Point(0, 0);
            int selection = codeTextBox.SelectionStart;
            int visibleLength = codeTextBox.DisplayedText.Length;

            int firstIndex = codeTextBox.GetCharIndexFromPosition(pos);
            int firstLine = codeTextBox.GetLineFromCharIndex(firstIndex);
            int currLine = codeTextBox.GetLineFromCharIndex(selection);
            int lastLine = codeTextBox.GetLineFromCharIndex(firstIndex + visibleLength);

            // Draws the line-selection
            int selectLine = (currLine - firstLine);
            var text_brush = new SolidBrush(this.codeTextBox.ForeColor);
            var border_rect = new Rectangle(1, (selectLine * scriptheight + 1), 46, scriptheight - 1);
            var inner_rect = new Rectangle(2, (selectLine * scriptheight + 2), 46, scriptheight - 2);

            var border_pen = new Pen(this.lineBorderColor);
            var back_brush = new SolidBrush(this.lineBackColor);

            e.Graphics.FillRectangle(back_brush, inner_rect);
            e.Graphics.DrawRectangle(border_pen, border_rect);

            for (int line = firstLine; line <= lastLine; line++)
            {
                var text_point = new Point(8, visibleindex * scriptheight);
                var line_text = line.ToString();

                while (line_text.Length < 0x4)
                {
                    line_text = line_text.Insert(0, "0");
                }

                // Draw the string
                e.Graphics.DrawString(line_text, Font, text_brush, text_point);

                // Increase the visible lineindex
                visibleindex += 1;
            }

            // Clean-Up
            back_brush.Dispose();
            border_pen.Dispose();
            text_brush.Dispose();
        }

        #endregion
    }
}