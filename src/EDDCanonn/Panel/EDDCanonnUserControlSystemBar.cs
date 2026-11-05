using EDDCanonnPanel.Base;
using EDDCanonnPanel.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl 
    {
        private void UpdateSystemBarFields(SystemData system, bool jumped)
        {
            if (system == null) //Safety first.
                return;

            SafeBeginInvoke(() =>
            {
                try
                {
                    textBoxSystem.Clear();
                    textBoxBodyCount.Clear();

                    textBoxSystem.AppendText(system.Name ?? "none");
                    textBoxBodyCount.AppendText(system.CountBodysFilteredByNodeType(new string[] { "Planet", "Star" }) + " / " + (system.BodyCount == -1 ? "?" : system.BodyCount.ToString()));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error in UpdateMainFields: {ex}";
                    CanonnLogging.Instance.Log(error);
                }
            });
        }

        private void textBoxSystem_Click(object sender, EventArgs e)
        {
            LinkUtil.OpenUrl(LinkUtil.SignalsCanonnTech + systemData?.Name?.Replace(" ", "%20"));
        }
    }
}
