using System;
using Massive;

namespace Sitecore.Pump.Entities
{
    public class SitecoreDB
    {
        public Items Items { get; set; }
        public SharedFields SharedFields { get; set; }
        public UnversionedFields UnversionedFields { get; set; }
        public VersionedFields VersionedFields { get; set; }
        public Descendants Descendants { get; set; }

        public SitecoreDB()
        {
            Items = new Items();
            SharedFields = new SharedFields();
            UnversionedFields = new UnversionedFields();
            VersionedFields = new VersionedFields();
            Descendants = new Descendants();
        }
    }

    public class Items : DynamicModel
    {
        public Items() : base("sitecoredb", "Items", "ID")
        {
        }
    }

    public class SharedFields : DynamicModel
    {
        public SharedFields() :base("sitecoredb","SharedFields","Id")
        {
        }
    }

    public class UnversionedFields : DynamicModel
    {
        public UnversionedFields()
            : base("sitecoredb", "UnversionedFields", "Id")
        {
        }
    }

    public class VersionedFields : DynamicModel
    {
        public VersionedFields()
            : base("sitecoredb", "VersionedFields", "Id")
        {
        }
    }

    public class Descendants : DynamicModel
    {
        public Descendants()
            : base("sitecoredb", "Descendants", "Id")
        {
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string TemplateID { get; set; }
        public string MasterID { get; set; }
        public string ParentID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

}
