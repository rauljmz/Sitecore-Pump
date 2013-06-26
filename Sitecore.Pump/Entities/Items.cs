using Massive;

namespace Sitecore.Pump.Entities
{
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

}
