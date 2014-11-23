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
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Scripting.Controls
{
    public class Intellisense : Component, IDisposable
    {
        #region Variables

        private bool disposed;
        private AutoCompleteWordCollection items;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the list of items
        /// </summary>
        [Category("Intellisense")]
        [Description("AutoCompleteItems")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AutoCompleteWordCollection Items
        {
            get
            {
                return items;
            }
        }

        #endregion

        #region Constructor

        // Creates an empty intellisense
        public Intellisense()
        {
            this.items = new AutoCompleteWordCollection();
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

                if (items != null)
                {
                    items.Clear();
                    items = null;
                    disposed = true;
                }
            }
        }

        #endregion

        #region Methods

        // Adds items from stringlist
        public void ParseItems(List<string> items, int index)
        {
            this.items.Clear();
            foreach (string itemword in items)
            {
                var word = new AutoCompleteWord();
                word.ImageIndex = index;
                word.ItemWord = itemword;
                this.items.Add(word);
            }
        }

        #endregion
    }

    #region Structures

    [Serializable]
    public class AutoCompleteWord
    {
        // We should not waste Bitmaps...
        // so we have a struct with an item_index
        private int imageIndex;
        private string itemWord;

        public int ImageIndex
        {
            get
            {
                return imageIndex;
            }
            set
            {
                imageIndex = value;
            }
        }

        public string ItemWord
        {
            get
            {
                return itemWord;
            }
            set
            {
                itemWord = value;
            }
        }
    }

    public class AutoCompleteWordCollection : CollectionBase
    {
        public AutoCompleteWord this[int Index]
        {
            get
            {
                return (AutoCompleteWord)List[Index];
            }
        }

        public void Add(AutoCompleteWord item)
        {
            List.Add(item);
        }

        public void Remove(AutoCompleteWord item)
        {
            List.Remove(item);
        }
    }

    #endregion
}