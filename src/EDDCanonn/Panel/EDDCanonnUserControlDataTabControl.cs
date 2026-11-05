using EDDCanonnPanel.Base;
using EDDCanonnPanel.Utility;
using KdTree;
using KdTree.Math;
using QuickJSON;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl 
    {
        private void UpdateUpperGridViews(SystemData system, bool jumped)
        {
            if (system == null)
                return;

            SafeBeginInvoke(() =>
            {
                try
                {
                    //Clear all grid views before populating them with new data.
                    dataGridViewData.Rows.Clear();
                    dataGridViewRing.Rows.Clear();
                    dataGridViewGeo.Rows.Clear();
                    dataGridViewBio.Rows.Clear();

                    //Collect missing ring data and update the corresponding grid.
                    List<DataGridViewRow> ringRows = CollectRingData(system);
                    dataGridViewRing.Rows.AddRange(ringRows?.ToArray() ??
                        new[] { CanonnUtil.CreateDataGridViewRow(dataGridViewRing, new object[]
                        { "No missing ring data for this system yet.", null, null, new Bitmap(1, 1) }) });

                    if (ringRows?.Count > 0)
                    {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "There are new rings that can be scanned.", Properties.Resources.ring }));
                    }

                    //Collect missing geo data and update the corresponding grid.
                    List<DataGridViewRow> geoRows = CollectGeoData(system);
                    dataGridViewGeo.Rows.AddRange(geoRows?.ToArray() ??
                        new[] { CanonnUtil.CreateDataGridViewRow(dataGridViewGeo, new object[]
                        { "No missing geo data for this system yet.", null, null, new Bitmap(1, 1) }) });

                    if (geoRows?.Count > 0)
                    {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "There are geological data that haven't been scanned yet.", Properties.Resources.geology }));
                    }

                    // Collect missing and existing bio data
                    ValueTuple<List<DataGridViewRow>, List<DataGridViewRow>> bioResult = CollectBioData(system);
                    List<DataGridViewRow> missingRows = bioResult.Item1;
                    List<DataGridViewRow> existingRows = bioResult.Item2;

                    // Merge both lists (null-safe)
                    List<DataGridViewRow> combinedRows = (missingRows ?? Enumerable.Empty<DataGridViewRow>())
                        .Concat(existingRows ?? Enumerable.Empty<DataGridViewRow>())
                        .ToList();

                    dataGridViewBio.Rows.AddRange(combinedRows.Count > 0
                        ? combinedRows.ToArray()
                        : new[]
                        {
                            CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                            { "No bio data for this system yet.", null, new Bitmap(1, 1) })
                        });

                    if (combinedRows.Count > 0)
                    {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "There is bio data available for this system.", Properties.Resources.biology }));
                    }


                    //If no jump occurred, no need to check.
                    if (jumped)
                    {
                        if (system.BodyCount == -1)
                        {
                            DataGridNotifications[NType.dscanRequired] = CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] {
                                NTypeDescription(NType.dscanRequired),
                                Properties.Resources.tourist });
                        }

                        dataGridViewGMO.Rows.Clear();
                        //Collect GMO data and update the corresponding grid.
                        List<DataGridViewRow> gmoRows = CollectGMO(system);
                        dataGridViewGMO.Rows.AddRange(gmoRows?.ToArray() ??
                            new[] { CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[]
                            { "No GMO was found for this system." }) });

                        if (gmoRows?.Count > 0)
                        {
                            DataGridNotifications[NType.gmoAvailable] = CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] {
                                NTypeDescription(NType.gmoAvailable),
                                Properties.Resources.tourist });
                        }
                    }

                    dataGridViewData.Rows.AddRange(CanonnUtil.CloneDataGridViewRowList(DataGridNotifications.Values.ToList()).ToArray());
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error in UpdateUpperGridViews: {ex}";
                    CanonnLogging.Instance.Log(error);
                }
            });
        }


        private List<DataGridViewRow> CollectRingData(SystemData system)
        {
            if (system?.Bodys?.Values == null)
                return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewRing, new object[] { "Missing Rings:", null, null, new Bitmap(1, 1) })
            };

            Regex ringRegex = new Regex(@"([A-Z])\s+Ring$", RegexOptions.IgnoreCase); //Pattern to extract short names.

            //Filter only bodies that have ring data.
            IEnumerable<Base.Body> validBodies = system.Bodys.Values
                .Where(body => body?.ScanData?.Rings != null);

            foreach (Base.Body body in validBodies)
            {
                //Find missing rings that are not mapped or do not contain an id64.
                List<JObject> missingRings = body.ScanData.Rings
                    .Where(ring =>
                    {
                        string ringName = ring["name"].Str(ring["Name"].StrNull()) ?? "none";
                        return ringName?.Contains("Ring") == true
                            && !(system.GetBodyByName(ringName)?.IsMapped == true || ring.Contains("id64"));
                    })
                    .ToList();

                if (!missingRings.Any())
                    continue;

                rows.AddRange(missingRings.Select((ring, index) =>
                {
                    string ringName = ring["name"].Str(ring["Name"].StrNull()) ?? "none";
                    //Extract inner and outer radius values, converting from meters to light-seconds.
                    double[] values = new[] { "innerRadius", "InnerRad", "outerRadius", "OuterRad" }
                        .Select(k => ring[k].Double(0.0) / 299792458)
                        .Select(v => Math.Round(v, 2))
                        .ToArray();

                    string result = $"{(values[0] == 0.0 ? values[1] : values[0])} ls - {(values[2] == 0.0 ? values[3] : values[2])} ls";

                    Match matchRing = ringRegex.Match(ringName);

                    return CanonnUtil.CreateDataGridViewRow(dataGridViewRing, new object[]
                    {
                        index == 0 ? body.BodyName?.Replace(system.Name, "") : null, //Only show the body name for the first ring.
                        matchRing.Success ? matchRing.Groups[1].Value + " Ring" : ringName,
                        result,
                        Properties.Resources.ring
                    });
                }));
            }

            if (rows.Count <= 1)
            {
                CanonnUtil.DisposeDataGridViewRowList(rows);
                return null;
            }

            return rows;
        }

        private List<DataGridViewRow> CollectGeoData(SystemData system) //wip
        {
            if (system?.Bodys?.Values == null)
                return null;

            List<DataGridViewRow> missingRows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewGeo, new object[] { "Missing Geo Data:", null, new Bitmap(1, 1), "false" })
            };

            IEnumerable<Base.Body> validBodies = system.Bodys.Values
                .Where(body => body?.ScanData?.Signals?.Any() == true);

            foreach (Base.Body body in validBodies)
            {
                JObject geologicalSignal = CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, "Type", "$SAA_SignalType_Geological;") ??
                                           CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, null, "$SAA_SignalType_Geological;");

                if (geologicalSignal == null)
                    continue;

                int geoCount = geologicalSignal["Count"].Int(-1);
                geoCount = geoCount == -1
                    ? geologicalSignal["$SAA_SignalType_Geological;"].Int(-1)
                    : geoCount;

                if (geoCount <= 0)
                    continue;

                int foundCount = body.ScanData.Geologic?.Count ?? 0;
                int missingCount = geoCount - foundCount;

                if (missingCount <= 0)
                    continue;

                missingRows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGeo, new object[]
                {
                    body.BodyName?.Replace(system.Name, "")?.TrimStart(),
                    $"{missingCount} missing geo signals",
                    Properties.Resources.geology,
                    "false"
                }));
            }

            if (missingRows.Count <= 1)
            {
                CanonnUtil.DisposeDataGridViewRowList(missingRows);
                return null;
            }

            return missingRows;
        }

        private (List<DataGridViewRow> missing, List<DataGridViewRow> existing) CollectBioData(SystemData system)
        {
            if (system?.Bodys?.Values == null)
                return (null, null);

            List<DataGridViewRow> missingRows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[] { "Missing Bio Data:", null, new Bitmap(1, 1), "false" })
            };

            List<DataGridViewRow> existingRows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[] { "Existing Bio Data:", null, new Bitmap(1, 1), "false" })
            };

            //Filter only bodies that have biological signals.
            IEnumerable<Base.Body> validBodies = system.Bodys.Values
                .Where(body => body?.ScanData?.Signals?.Any() == true);

            foreach (Base.Body body in validBodies)
            {
                //Find the first biological signal entry.
                JObject biologicalSignal = CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, "Type", "$SAA_SignalType_Biological;") ??
                                           CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, null, "$SAA_SignalType_Biological;");

                if (biologicalSignal == null)
                    continue;

                int bioCount = biologicalSignal["Count"].Int(-1);
                bioCount = bioCount == -1
                    ? biologicalSignal["$SAA_SignalType_Biological;"].Int(-1)
                    : bioCount;

                //If no genus data is available, add an entry with unknown.
                if (body.ScanData.Genuses?.Any() != true)
                {
                    missingRows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                    {
                        body.BodyName?.Replace(system.Name, "") ?? "none",
                        $"{bioCount} unknown species",
                        Properties.Resources.biology,
                        "false"
                    }));
                    continue;
                }

                //Find samples that have not been collected yet.
                List<JObject> missingSamples = body.ScanData.Genuses
                    .Where(genus => !CanonnUtil.ContainsKeyValuePair(body.ScanData.Organics, "Genus", genus["Genus"].StrNull()))
                    .ToList();

                if (missingSamples.Count < body.ScanData.Genuses.Count && body.ScanData.SystemPois?.Count > 0)
                {
                    existingRows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                    {
                        body.BodyName?.Replace(system.Name, "")?.TrimStart(),
                        "Double-click for an overview.",
                        Properties.Resources.biology,
                        "true"
                    }));
                }

                if (missingSamples.Any())
                {
                    missingRows.AddRange(missingSamples.Select((genus, index) =>
                    CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                    {
                        index == 0 ? body.BodyName?.Replace(system.Name, "")?.TrimStart() : null, //Only show body name for the first missing sample.
                        "Data required: " + genus["Genus"] != "Brain Tree" ? DataUtil.BiologyGenuses(genus["Genus"].StrNull()) : "Brain Tree",
                        Properties.Resources.biology,
                        "false"
                    })));
                }
            }

            new[] { missingRows, existingRows }
                .Where(list => list.Count <= 1)
                .ToList()
                .ForEach(CanonnUtil.DisposeDataGridViewRowList);

            return (missingRows, existingRows);
        }



        private List<DataGridViewRow> CollectGMO(SystemData system)
        {
            JArray gmos = DLLCallBack.GetGMOs("systemname=" + system.Name)?.JSONParseArray() ?? null;
            if (gmos == null || gmos.Count == 0)
                return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>();

            string combinedDescriptions = "Tags: " + string.Join(", ", gmos[0]?["GalMapTypes"]?.Array()
                ?.Select(galMapType => galMapType?["Description"].StrNull())
                .Where(desc => !string.IsNullOrWhiteSpace(desc)) ?? Enumerable.Empty<string>());
            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { combinedDescriptions }));

            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { null }));

            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { gmos[0]?["DescriptiveNames"]?[0].StrNull() + ":" }));
            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { gmos[0]?["Description"].StrNull() }));

            return rows;
        }


    }
}
