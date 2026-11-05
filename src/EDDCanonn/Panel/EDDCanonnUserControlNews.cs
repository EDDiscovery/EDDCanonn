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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl 
    {
        private JArray News;
        private int NewsIndex = 0;

        private void InitializeNews()
        {
            dataHandler.StartTaskAsync((token) =>
            {
                try
                {
                    News = dataHandler.FetchData(LinkUtil.CanonnNews).response.JSONParseArray();
                    NewsIndexChanged(0);
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error in InitializeNews: {ex}";
                    CanonnLogging.Instance.Log(error);
                }
            },
                ex =>
                {
                    string error = $"EDDCanonn: Unexpected error in InitializeNews Task: {ex}";
                    CanonnLogging.Instance.Log(error);
                },
                "InitializeNews"
            );
        }

        private void NewsIndexChanged(int n)
        {
            SafeBeginInvoke(() =>
            {
                if (News == null)
                {
                    textBoxNews.Text = "No News Service";
                    buttonPrevNews.Enabled = buttonNextNews.Enabled = false;
                    return;
                }
                else if (News.Count < 1)
                {
                    textBoxNews.Text = "No News";
                    buttonPrevNews.Enabled = buttonNextNews.Enabled = false;
                    return;
                }

                if (n == 1 && NewsIndex + n >= News.Count)
                    NewsIndex = 0;
                else if (n == -1 && NewsIndex + n < 0)
                    NewsIndex = News.Count - 1;
                else
                    NewsIndex += n;

                try
                {
                    textBoxNews.Clear();

                    string decoded = News[NewsIndex]?["excerpt"]?["rendered"].Str("none");

                    try
                    {
                        decoded = Regex.Replace(WebUtility.HtmlDecode(System.Text.RegularExpressions.Regex.Unescape(decoded)), "<.*?>", "");
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error processing news text: {ex}";
                        CanonnLogging.Instance.Log(error);
                    }

                    textBoxNews.AppendText(decoded);
                    labelNewsIndex.Text = "Page: " + (NewsIndex + 1);
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Unexpected error in NewsIndexChanged: {ex}";
                    CanonnLogging.Instance.Log(error);
                }
            });
        }

        private void buttonNextNews_Click(object sender, EventArgs e)
        {
            NewsIndexChanged(1);
        }

        private void buttonPrevNews_Click(object sender, EventArgs e)
        {
            NewsIndexChanged(-1);
        }

        private void textBoxNews_Click(object sender, EventArgs e)
        {
            if (News != null && News.Count > 0)
                LinkUtil.OpenUrl(News[NewsIndex]?["link"].StrNull() ?? LinkUtil.CanonnWebPage);
        }

    }
}
