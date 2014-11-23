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
    public class CodeListBox : ListBox
    {
        #region Constants

        const ControlStyles style = ControlStyles.AllPaintingInWmPaint 
            | ControlStyles.OptimizedDoubleBuffer;

        #endregion

        #region Variables

        private bool disposed;
        private int itemHeight;

        private ImageList imageList;
        private Intellisense autoComplete;

        private Color selectionBorderColor;
        private Color selectionColor;

        #endregion

        #region Properties

        /// <summary>
        /// Retrieves or sets the imagelist
        /// </summary>
        public ImageList ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                imageList = value;
            }
        }

        /// <summary>
        /// Retrieves or sets the intellisense
        /// </summary>
        public Intellisense AutoComplete
        {
            get
            {
                return autoComplete;
            }
            set
            {
                this.autoComplete = value;
                if (value != null)
                {
                    this.Items.Clear();
                    foreach (AutoCompleteWord word in value.Items)
                    {
                        this.Items.Add(word);
                    }
                }
            }
        }

        public int MyItemHeight
        {
            get
            {
                return itemHeight;
            }
            set
            {
                itemHeight = value;
                ItemHeight = value;
            }
        }

        /// <summary>
        /// Retrieves or sets the border-color
        /// of the currently selected item
        /// </summary>
        public Color SelectionBorderColor
        {
            get
            {
                return selectionBorderColor;
            }
            set
            {
                selectionBorderColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Retrieves or sets the back-color
        /// of the currently selected item
        /// </summary>
        public Color SelectionColor
        {
            get
            {
                return selectionColor;
            }
            set
            {
                selectionColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Retrieves the count of items, which can be displayed
        /// before a scrollbar appears (default value: 10)
        /// </summary>
        public int MaxItemsWithoutScroll
        {
            get
            {
                return ((MaximumSize.Height / itemHeight) - 8);
            }
            set
            {
                int width = MaximumSize.Width;
                var size = new Size(width, (8 + (itemHeight * value)));
                MaximumSize = size; this.Invalidate();
            }
        }

        #endregion

        #region Constructor

        // Constructs a listbox and enables various
        // styles for customizable drawing
        public CodeListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DoubleBuffered = true;
            this.SetStyle(style, true);
            this.itemHeight = 16;
            this.ItemHeight = 16;

            this.selectionBorderColor = Color.FromArgb(216, 216, 216);
            this.selectionColor = Color.FromArgb(230, 230, 230);
            this.MaximumSize = new Size(200, (itemHeight * 10) + 8);
            this.disposed = false;
        }

        #endregion

        #region Destructor

        // We override the destructor with our custom code
        // because we have a list, which we need to clear.
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Clean up other resources,
                // done by the base-destructor
                base.Dispose(disposing);

                if (autoComplete != null)
                {
                    autoComplete.Dispose();
                    autoComplete = null;
                }
                if (imageList != null)
                {
                    imageList.Dispose();
                    imageList = null;
                }

                disposed = true;
            }
        }

        #endregion

        #region Painting

        // We can override the drawing logic for each auto_item
        // when we set the DrawMode to OwnerDrawFixed. This
        // provides the possibility of e.g drawing our own selection
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // It can happen (because of windows-errors) that
            // the item_index of the auto_item returns -1, which makes it
            // impossible to draw - with this procedure we prevent a crash.
            var item_index = e.Index;
            if (item_index != -1)
            {
                var auto_item = ((AutoCompleteWord)Items[item_index]);
                var text_pnt = new Point(18, item_index * itemHeight);
                var empty_pnt = new Point(0, 0);
                var item_text = auto_item.ItemWord;

                // Determine whether the auto_item is selected, by ANDing a bitfield.
                // ANDing ensures us an error-free determination, because some items
                // might have more DrawItemStates, than only the selection.
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    var back_brush = new SolidBrush(selectionColor);
                    var selection_pen = new Pen(selectionBorderColor);

                    // Reduces the height and width of our auto_item-rectangle
                    // because fill/drawrectangle has a zero-based mechanism.
                    var item_size = new Size(e.Bounds.Width, itemHeight);
                    var item_rect = new Rectangle(text_pnt, item_size); item_rect.X = 0;
                    item_rect.Height--; item_rect.Width--;

                    e.Graphics.FillRectangle(back_brush, item_rect);
                    e.Graphics.DrawRectangle(selection_pen, item_rect);

                    back_brush.Dispose();
                    selection_pen.Dispose();
                }
                else
                {
                    // If the auto_item is not selected we must clear the whole
                    // auto_item-area to avoid a string-overlay, which looks buggy.
                    var item_size = new Size(e.Bounds.Width, itemHeight);
                    var item_rect = new Rectangle(text_pnt, item_size); item_rect.X = 0;
                    using (var fill_brush = new SolidBrush(BackColor))
                    {
                        e.Graphics.FillRectangle(fill_brush, item_rect);
                    }
                }

                // As soon as we finished drawing the background and
                // the selection, we draw the text with the control's forecolor.
                using (var text_brush = new SolidBrush(ForeColor))
                {
                    e.Graphics.DrawString(item_text, Font, text_brush, text_pnt);
                }

                // If there are no images, we have to stop drawing here
                // in order to avoid null-argument-exceptions.
                if (imageList != null)
                {
                    int image_index = auto_item.ImageIndex;
                    if (image_index < imageList.Images.Count)
                    {
                        var bmp = imageList.Images[auto_item.ImageIndex];
                        e.Graphics.DrawImage(bmp, empty_pnt);
                    }
                }
            }
        }

        // As we want to draw an image next to the intellibox-auto_item
        // we need to trick out the MeasureItemEventArgs 
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);
            e.ItemHeight = itemHeight;
            e.ItemWidth += 18;
        }

        #endregion
    }
}