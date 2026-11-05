using EDDCanonnPanel.Base;
using EDDCanonnPanel.Utility;
using KdTree;
using KdTree.Math;
using QuickJSON;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl 
    {
        private Patrols patrols;
        private void InitializePatrols()
        {
            try
            {
                patrols = new Patrols();

                ExtComboBoxPatrol.Items.Add("All"); //The 'all' KdTree-Dictionary has already been set in “Patrols.cs”.

                ExtComboBoxRange.Items.AddRange(DataUtil.PatrolRanges.Select(x => x.ToString() + " LY").ToArray());

                ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>(); //We have to make sure that all workers are finished before we update the patrols.

                //Start a 'head worker' to avoid blocking the UI thread.
                dataHandler.StartTaskAsync(
                (token) =>
                {
                    //Fetch information about available patrols.
                    List<Dictionary<string, string>> records = DataUtil.ParseTsv(dataHandler.FetchData(LinkUtil.PatrolUrl).response);
                    foreach (Dictionary<string, string> record in records)
                    {
                        token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                        try
                        {
                            string description = record.TryGetValue("Description", out string descriptionValue)
                            ? descriptionValue : "uncategorized";
                            bool enabled = record.TryGetValue("Enabled", out string enabledValue)
                            && enabledValue == "Y";
                            string type = record.TryGetValue("Type", out string typeValue)
                            ? typeValue : string.Empty;
                            string url = record.TryGetValue("Url", out string urlValue)
                            ? urlValue : string.Empty;

                            if (!enabled)
                                continue;
                            if (description.Equals("Landscape Signals"))
                                continue;

                            if (type.Equals("tsv")) //Worker for tsv files.
                            {
                                // comment below out to make it easier to debug sequentially: CreateFromTSV(url, description, new CancellationToken());
                                _tasks.Add(dataHandler.StartTaskAsync(
                                    (subToken) =>
                                    {
                                        CreatePatrolFromTSV(url, description, subToken);
                                    },
                                    ex =>
                                    {
                                        string error = $"EDDCanonn: Error Initialize Patrols -> {description}: {ex}";
                                        CanonnLogging.Instance.Log(error);
                                    },
                                        "InitializePatrol -> SubThread: " + description
                                    ));
                            }
                            else if (type.Equals("json")) //Worker for json files.
                            {
                                _tasks.Add(dataHandler.StartTaskAsync(
                                    (subToken) =>
                                    {
                                        CreatePatrolFromJson(url, description, subToken);
                                    },
                                    ex =>
                                    {
                                        string error = $"EDDCanonn: Error Initialize Patrols -> {description}: {ex}";
                                        CanonnLogging.Instance.Log(error);
                                    },
                                        "InitializePatrol -> SubThread: " + description
                                    ));
                            }
                        }
                        catch (Exception ex)
                        {
                            string error = $"EDDCanonn: Error processing record category: {ex}";
                            CanonnLogging.Instance.Log(error);
                        }
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error In Patrols HeadThread: {ex}";
                    CanonnLogging.Instance.Log(error);
                },
                    "InitializePatrol -> HeadThread",
                    new Action(
                    () => //We are still in the 'HeadThread' here. We forward this action as 'ContinueWith' -> ExecuteSynchronously.
                    {
                        Task.WaitAll(_tasks.ToArray()); //The 'HeadThread' must wait until its workers are all finished.
                        SafeInvoke(() =>
                        {
                            ExtComboBoxPatrol.Enabled = true;
                            ExtComboBoxPatrol.SelectedIndex = 0;
                            ExtComboBoxRange.Enabled = true;
                            ExtComboBoxRange.SelectedIndex = 2;
                        });
                        patrolToolStripLock = false;
                    })
                );
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error in InitializePatrols: {ex}";
                CanonnLogging.Instance.Log(error);
            }
        }


        private void CreatePatrolFromTSV(string url, string category, CancellationToken token)
        {
            try
            {
                //Initialize a 3D KdTree for patrol locations.
                KdTree<double, Patrol> kdT = new KdTree<double, Patrol>(3, new DoubleMath(), AddDuplicateBehavior.Update);

                //Fetch TSV data and parse it into a list of dictionaries.
                List<Dictionary<string, string>> records = DataUtil.ParseTsv(dataHandler.FetchData(url).response);

                foreach (Dictionary<string, string> record in records)
                {
                    token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.

                    try
                    {
                        string patrolType = record.TryGetValue("Patrol", out string patrolValue) ? patrolValue :
                            record.TryGetValue("Type", out string typeValue) ? typeValue : string.Empty;

                        string system = record.TryGetValue("System", out string systemValue) ? systemValue : string.Empty;

                        if (system.HasChars() && patrolType.HasChars())
                        {
                            double x = record.TryGetValue("X", out string xstr) ? Convert.ToDouble(xstr, CultureInfo.InvariantCulture) : DataUtil.PositionFallback;
                            double y = record.TryGetValue("Y", out string ystr) ? Convert.ToDouble(ystr, CultureInfo.InvariantCulture) : DataUtil.PositionFallback;
                            double z = record.TryGetValue("Z", out string zstr) ? Convert.ToDouble(zstr, CultureInfo.InvariantCulture) : DataUtil.PositionFallback;

                            string instructions = record.TryGetValue("Instructions", out string instructionsValue) ? instructionsValue : "none";
                            string urlp = record.TryGetValue("Url", out string urlValue) ? urlValue : string.Empty;

                            //Construct a new Patrol object and insert it into the KdTree.
                            Patrol patrol = new Patrol(patrolType, category, system, x, y, z, instructions, urlp);
                            kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                        }
                        else
                        {
                            string error = $"EDDCanonn: Error in record downloaded from {url} missing data";
                            CanonnLogging.Instance.Log(error);
                        }

                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error processing record: {ex}";
                        CanonnLogging.Instance.Log(error);
                    }
                }

                //Store the completed KdTree in the patrols dictionary.
                patrols.Add(category, kdT);

                //Update the UI with the new category entry.
                SafeBeginInvoke(() =>
                {
                    ExtComboBoxPatrol.Items.Add(category);
                });

            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error in CreateFromTSV for category {category}: {ex}";
                CanonnLogging.Instance.Log(error);
            }
        }

        private void CreatePatrolFromJson(string url, string category, CancellationToken token)
        {
            try
            {
                //Initialize a 3D KdTree for patrol locations.
                KdTree<double, Patrol> kdT = new KdTree<double, Patrol>(3, new DoubleMath(), AddDuplicateBehavior.Update);
                JArray jsonRecords = dataHandler.FetchData(url).response.JSONParse().Array();
                if (jsonRecords == null || jsonRecords.Count == 0)
                    return;

                foreach (JObject record in jsonRecords)
                {
                    token.ThrowIfCancellationRequested();//We abort here if a cancellation was requested.
                    try
                    {
                        if (record == null || record.Count == 0)
                            continue;

                        string system = record["system"].Str();

                        double x = record["x"].Double(DataUtil.PositionFallback);
                        double y = record["y"].Double(DataUtil.PositionFallback);
                        double z = record["z"].Double(DataUtil.PositionFallback);

                        string instructions = record["instructions"].Str("none");
                        string urlp = record["url"].Str();

                        //Construct a new Patrol object and insert it into the KdTree.
                        Patrol patrol = new Patrol(category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error processing JSON record: {ex}";
                        CanonnLogging.Instance.Log(error);
                    }
                }
                //Store the completed KdTree in the patrols dictionary.
                patrols.Add(category, kdT);

                //Update the UI with the new category entry.
                SafeBeginInvoke(() =>
                {
                    ExtComboBoxPatrol.Items.Add(category);
                });
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error in CreateFromJson for category {category}: {ex}";
                CanonnLogging.Instance.Log(error);
            }
        }

        private readonly object _patrolLock = new object();
        private bool patrolToolStripLock = true;
        private void UpdatePatrols()
        {
            dataHandler.StartTaskAsync(
                (token) =>
                {
                    while (systemData == null) // We have to wait until the system data has been loaded.
                    {
                        token.ThrowIfCancellationRequested();
                        Task.Delay(250, token).Wait();
                    }

                    lock (_patrolLock)
                    {
                        string type = "";
                        double range = 0.0;
                        SystemData system = DeepCopySystemData();

                        SafeInvoke(() =>
                        {
                            type = ExtComboBoxPatrol.SelectedItem?.ToString() ?? "";
                            range = ExtComboBoxRange.HasChildren && DataUtil.PatrolRanges.Length > 0
                                ? (ExtComboBoxRange.SelectedIndex >= 0 && ExtComboBoxRange.SelectedIndex < DataUtil.PatrolRanges.Length
                                    ? DataUtil.PatrolRanges[ExtComboBoxRange.SelectedIndex]
                                    : DataUtil.PatrolRanges[0])
                                : -1;
                        });

                        if (string.IsNullOrEmpty(type) || range == -1)
                            return;

                        List<(string category, Patrol patrol, double distance)> patrolList = patrols.FindPatrolsInRange(type, system.X, system.Y, system.Z, range);

                        List<DataGridViewRow> result = new List<DataGridViewRow>();
                        foreach (var (category, patrol, distance) in patrolList)
                            result.Add(CanonnUtil.CreateDataGridViewRow(dataGridPatrol, new object[] { patrol.category, patrol.instructions, distance.ToString(), patrol.system, patrol.url }));

                        SafeBeginInvoke(() =>
                        {
                            dataGridPatrol.Rows.Clear();
                            dataGridPatrol.Rows.AddRange(result.ToArray());
                        });
                    }
                },
            ex =>
            {
                string error = $"EDDCanonn: Error during UpdatePatrols Task: {ex}";
                CanonnLogging.Instance.Log(error);
            },
                "UpdatePatrols",
                new Action(
                    () => //We are still in the 'UpdatePatrolsThread' here. We forward this action as 'ContinueWith' -> ExecuteSynchronously.
                    {
                        SafeInvoke(() =>
                        {
                            patrolToolStripLock = false;
                        });
                    })
            );
        }


        #region UI Patrol

        private void extComboBoxPatrolOrRange_IndexChanged(object sender, EventArgs e)
        {
            if (patrolToolStripLock)
                return;
            else
                patrolToolStripLock = true;

            UpdatePatrols();
        }

        private void toolstrip_CopySystem_Click(object sender, EventArgs e)
        {
            if (dataGridPatrol.SelectedCells.Count == 0)
                return;

            int rowIndex = dataGridPatrol.SelectedCells[0].RowIndex;
            if (rowIndex < 0 || rowIndex >= dataGridPatrol.Rows.Count)
                return;

            DataGridViewRow selectedRow = dataGridPatrol.Rows[rowIndex];
            if (dataGridPatrol.Columns["SysName"] == null || selectedRow.Cells["SysName"] == null)
                return;

            object cellValue = selectedRow.Cells["SysName"].Value;
            if (cellValue == null)
                return;

            try
            {
                Clipboard.SetText(cellValue.ToString());
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Clipboard error:: {ex}";
                CanonnLogging.Instance.Log(error);
            }
        }


        private void toolstrip_OpenURL_Click(object sender, EventArgs e)
        {
            if (dataGridPatrol.SelectedCells.Count == 0)
                return;

            int rowIndex = dataGridPatrol.SelectedCells[0].RowIndex;
            if (rowIndex < 0 || rowIndex >= dataGridPatrol.Rows.Count)
                return;

            DataGridViewRow selectedRow = dataGridPatrol.Rows[rowIndex];
            if (dataGridPatrol.Columns["PatrolUrl"] == null || selectedRow.Cells["PatrolUrl"] == null)
                return;

            object cellValue = selectedRow.Cells["PatrolUrl"].Value;
            if (cellValue == null)
                return;

            LinkUtil.OpenUrl(cellValue.ToString());
        }



        private void dataGridPatrol_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo hit = dataGridPatrol.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            dataGridPatrol.ClearSelection();
            dataGridPatrol.Rows[hit.RowIndex].Cells[hit.ColumnIndex].Selected = true;
        }

        #endregion


    }
}
