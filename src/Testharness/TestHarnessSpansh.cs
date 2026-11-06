using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestHarness.Properties;

namespace CHash
{
    public partial class TestHarness 
    {

        private async void DLLRequestSpanshDump(object requesttag, object usertag, string systemname, long systemaddress, bool weblookup, bool cachelookup, string _)
        {
            System.Diagnostics.Trace.Assert(Application.MessageLoop);       // must be

            var dll = DLLManager.FindCSharpCallerByStackTrace();    // need to find who called - use the stack to trace the culprit

            if (dll != null)
            {
                string file = null;
                file = Resources.spanshv4__muang__4481966019282;
                file = Resources.spanshv4__pyranu_qo_z_d13_28__977516105979;
                file = Resources.spanshv4__puppis_sector_to_r_b4_4__9467852826033;

                dll.DataResult(requesttag, usertag, file);
            }
        }

    }
}
