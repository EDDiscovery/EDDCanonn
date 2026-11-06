using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CHash
{
    public partial class TestHarness 
    {
        public EDDDLLInterfaces.EDDDLLIF.EDDCallBacks callbacks = new EDDDLLInterfaces.EDDDLLIF.EDDCallBacks();

        public bool DLLRequestHistory(long index, bool isjid, out EDDDLLInterfaces.EDDDLLIF.JournalEntry f)
        {
            EDDDLLInterfaces.EDDDLLIF.JournalEntry nje = MakeJE();
            nje.indexno = (int)index;

            f = nje;

            richTextBox1.Text += "Request history " + index + " " + isjid + Environment.NewLine;
            return true;
        }

        public bool DLLRunAction(string eventname, string paras)
        {
            richTextBox1.Text += "Run action " + eventname + " " + paras + Environment.NewLine;
            return true;
        }

        public string DLLGetShipLoadout(string shipnameorcurrent)
        {
            return "Ship JSON";
        }



    }
}
