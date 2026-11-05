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
#if true
        private CodexDatabase CodexDatabase;
        private void InitializeCodex()
        {
            CodexDatabase = new CodexDatabase();

            dataHandler.StartTaskAsync(
            (token) =>
            {
                JObject o = dataHandler.FetchData(LinkUtil.CodexRef).response?.JSONParse().Object()
                    ?? DataUtil.ReadEmbeddedTextFile("EDDCanonnPanel.Resources.codex_ref.json")?.JSONParse().Object();

                if (o == null)
                    return;

                o.PropertyNames()
                    .SelectMany(type => o[type] is JObject typeObj
                        ? typeObj.PropertyNames()
                            .SelectMany(subType => typeObj[subType] is JObject subTypeObj
                                ? subTypeObj.PropertyNames()
                                    .Select(entryKey => subTypeObj[entryKey] is JObject obj ? new CodexEntry(
                                        type,
                                        subType,
                                        entryKey ?? CanonnUtil.GenerateId().ToString(),
                                        obj["entryid"].Int(CanonnUtil.GenerateId()),
                                        obj["name"].Str(CanonnUtil.GenerateId().ToString()),
                                        obj["category"].StrNull(),
                                        obj["sub_category"].StrNull(),
                                        obj["platform"].StrNull(),
                                        obj["reward"].Int(-1)
                                    ) : null)
                                : Enumerable.Empty<CodexEntry>())
                        : Enumerable.Empty<CodexEntry>())
                    .Where(entry => entry != null)
                    .ToList()
                    .ForEach(CodexDatabase.Add);
            },
            ex =>
            {
                string error = $"EDDCanonn: Unexpected error in InitializeCodex: {ex}";
                CanonnLogging.Instance.Log(error);
            },
                "InitializeCodex"
            );
        }
#endif
    }
}
