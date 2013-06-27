using System;
using System.Collections.Generic;
using Massive;
using System.Linq;
namespace Sitecore.Pump.Entities
{
    public enum RowState
    {
        New,
        Updated,
        Deleted
    }

    public class SitecoreDB
    {
        public  List<dynamic> Items { get; set; }
        public List<dynamic> SharedFields { get; set; }
        public List<dynamic> UnversionedFields { get; set; }
        public List<dynamic> VersionedFields { get; set; }
      //  public List<dynamic> Descendants { get; set; }

        public SitecoreDB()
        {
            Items = new Items().All().ToList();
            SharedFields = new SharedFields().All().ToList();
            UnversionedFields = new UnversionedFields().All().ToList();
            VersionedFields = new VersionedFields().All().ToList();
           // Descendants = new Descendants().All().ToList();
        }

        public void SaveAll()
        {
            SaveTable(new Items(), Items);
            SaveTable(new SharedFields(), SharedFields);
            SaveTable(new VersionedFields(), VersionedFields);
            SaveTable(new UnversionedFields(), UnversionedFields);
        }

        public void SaveTable(DynamicModel table, List<dynamic> items )
        {           
            foreach (dynamic item in items)
            {
                if (item.State == RowState.New)
                {
                    table.Insert(item);
                }
                else if(item.State == RowState.Updated)
                {
                    table.Update(item, item.ID);
                }
            }
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
