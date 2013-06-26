using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Massive;

namespace Sitecore.Pump.Entities
{
    public class Items : DynamicModel
    {
        public Items() : base("sitecoredb", "Items", "ID")
        {
        }
    }
}
