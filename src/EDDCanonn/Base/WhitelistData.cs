using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDCanonn.Base
{
    public class WhitelistData
    {
        public List<WhitelistEvent> Events { get; set; }

        public WhitelistData()
        {
            Events = new List<WhitelistEvent>();
        }
    }

    public class WhitelistEvent
    {
        public string Type { get; set; }
        public List<Dictionary<string, object>> DataBlocks { get; set; }

        public WhitelistEvent()
        {
            DataBlocks = new List<Dictionary<string, object>>();
        }
    }
}
