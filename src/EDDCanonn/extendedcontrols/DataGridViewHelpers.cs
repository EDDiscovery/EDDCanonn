using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static partial class DataGridViewControlHelpersStaticFunc
{
    public static int SafeFirstDisplayedScrollingRowIndex(this DataGridView dgv)
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            return dgv.CurrentCell != null ? dgv.CurrentCell.RowIndex : 0;
        }
        else
        {
            return dgv.FirstDisplayedScrollingRowIndex;
        }
    }

    public static int GetNumberOfVisibleRowsAbove(this DataGridViewRowCollection table, int rowindex)
    {
        int visible = 0;
        for (int i = 0; i < rowindex; i++)
        {
            if (table[i].Visible)
                visible++;
        }
        return visible;
    }

    public static void SafeFirstDisplayedScrollingRowIndex(this DataGridView dgv, int rowno)
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            // MONO does not implement SafeFirstDisplayedScrollingRowIndex
            if (rowno >= 0 && rowno < dgv.Rows.Count)
            {
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    if (dgv.Columns[i].Visible)
                    {
                        while (!dgv.Rows[rowno].Visible && rowno < dgv.Rows.Count)
                            rowno++;
                        int rowsvisible = dgv.DisplayedRowCount(false);
                        int rownobot = Math.Min(rowsvisible + rowno - 1, dgv.Rows.Count - 1);
                        while (!dgv.Rows[rownobot].Visible && rownobot > 1)
                            rownobot--;
                        dgv.CurrentCell = dgv.Rows[rownobot].Cells[i];      // blam top and bottom to try and get the best view
                        dgv.CurrentCell = dgv.Rows[rowno].Cells[i];
                        dgv.Rows[rowno].Selected = true;
                        break;
                    }
                }
            }
        }
        else
        {
            try
            {
                dgv.FirstDisplayedScrollingRowIndex = rowno;    // SAFE VERSION
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DGV exception FDR " + e);       // v.rare.
            }
        }
    }


}
