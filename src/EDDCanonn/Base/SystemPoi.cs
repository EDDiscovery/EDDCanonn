namespace EDDCanonnPanel.Base
{
    public class SystemPoi
    {
        public SystemPoi(string body, string name, int entryID, string hudCategory, int indexID, string lat, string lon, bool scanned)
        {
            Body = body;
            Name = name;
            EntryID = entryID;
            HudCategory = hudCategory;
            IndexID = indexID;
            Lat = lat;
            Lon = lon;
            Scanned = scanned;
        }

        public SystemPoi(SystemPoi original)
        {
            Body = original.Body;
            Name = original.Name;
            EntryID = original.EntryID;
            HudCategory = original.HudCategory;
            IndexID = original.IndexID;
            Lat = original.Lat;
            Lon = original.Lon;
            Scanned = original.Scanned;
        }

        public string Body { get; set; }
        public string Name { get; set; }
        public int EntryID { get; set; }
        public string HudCategory { get; set; }
        public int IndexID { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public bool Scanned { get; set; }
    }

    public class CmdrPoi
    {
        public CmdrPoi(string body, string category, string description, double lat, double lon)
        {
            Body = body;
            Category = category;
            Description = description;
            Lat = lat;
            Lon = lon;
        }
        public CmdrPoi(CmdrPoi original)
        {
            Body = original.Body;
            Category = original.Category;
            Description = original.Description;
            Lat = original.Lat;
            Lon = original.Lon;
        }

        public string Body { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
