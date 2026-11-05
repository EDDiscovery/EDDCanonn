using EDDCanonnPanel.Base;
using EDDCanonnPanel.Utility;
using QuickJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl 
    {
        private readonly object _lockSystemData = new object();
        private SystemData _systemData; //Do not use this. Otherwise it could get bad.
        private void ResetSystemData()
        {
            lock (_lockSystemData)
            {
                SafeInvoke(() =>
                {
                    _systemData = null;
                    extTabControlData.SelectedIndex = 0;
                    CanonnUtil.DisposeDataGridViewRowList(notifications.Values.ToList());
                    notifications.Clear();
                });
            }
        }

        private SystemData DeepCopySystemData()
        {
            lock (_lockSystemData)
            {
                if (_systemData != null)
                    return new SystemData(_systemData);
                else
                    return null;
            }
        }

        private SystemData systemData //Encapsulation Enforcement | Is that what it's called?
        {
            get
            {
                lock (_lockSystemData)
                {
                    if (_systemData == null)
                        return null;

                    return _systemData;
                }
            }
            set
            {
                lock (_lockSystemData)
                {
                    if (_systemData != null)
                        return;

                    _systemData = new SystemData(); //Enforces encapsulation.
                }
            }
        }

        private void ProcessNewSystem(JObject eventData)
        {
            try
            {
                if (systemData == null)
                    systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

                systemData.Name = eventData["StarSystem"].StrNull() ?? eventData["System"].Str("none");
                systemData.SystemAddress = eventData["SystemAddress"].Long(-1);
                if (eventData["StarPos"] != null)
                {
                    systemData.X = eventData["StarPos"][0].Double(DataUtil.PositionFallback);
                    systemData.Y = eventData["StarPos"][1].Double(DataUtil.PositionFallback);
                    systemData.Z = eventData["StarPos"][2].Double(DataUtil.PositionFallback);
                }
            }
            catch (Exception ex) //If we end up here. We have a problem.
            {
                ResetSystemData(); //In any case, we set the system to null.
                string error = $"EDDCanonn: Error processing NewSystem: {ex}";
                CanonnLogging.Instance.Log(error);
            }
        }


    }
}
