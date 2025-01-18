using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CHash
{
    // for the .bat runner, see
    // https://stackoverflow.com/questions/5605885/how-to-run-a-bat-from-inside-the-ide
    // make sure you select the right part in section 2 part 1!

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestHarness());
        }
    }
}
