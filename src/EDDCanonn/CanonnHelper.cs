
using System.Collections.Generic;
using System.Linq;
using QuickJSON;
namespace EDDCanonn
{
    public static class CanonnHelper
    {
        public static readonly string WhitelistUrl = "https://us-central1-canonn-api-236217.cloudfunctions.net/postEventWhitelist";
        public static readonly string EventPushUrl = "https://us-central1-canonn-api-236217.cloudfunctions.net/postEvent";
        public static readonly string PatrolUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQsi1Vbfx4Sk2msNYiqo0PVnW3VHSrvvtIRkjT-JvH_oG9fP67TARWX2jIjehFHKLwh4VXdSh0atk3J/pub?gid=0&single=true&output=tsv";
   
        public static List<JObject> GetJObjectList(JObject source, string key)
        {
            if (source[key] is JArray array)
                return array.OfType<JObject>().ToList();
            return null;
        }

    }
}
