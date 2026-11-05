/*
 * Copyright 2016-2025 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ExtendedControls
{
    public class ExtPanelDataGridViewScrollOutlining : Control
    {
        public class Outline
        {
            public int start;
            public int end;
            public bool expanded;

            public Outline() { }
            public Outline(int start, int end, bool expanded = true) { this.start = start; this.end = end; this.expanded = expanded; }
        }

        public System.Collections.IEnumerable OutlineSet() { return null; }
        public void UpdateOutlines() { }

        public void SetDGV(DataGridView dgv)    // called to set dgv
        {
        }

        public void Clear()
        { }

        public void RowAdded(int row, int count)
        { }
        public void RowRemoved(int row, int count)      // called if we removed rows from DGV
        { }

        public void RowChangedState(int row)
        { }


    }

}
