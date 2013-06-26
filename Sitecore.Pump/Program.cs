using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sitecore.Pump
{
    class Program
    {

        static void DisplayUsage()
        {
            Console.WriteLine("Usage: Sitecore.Pump serialize|deserialize path  [--config configfile databasename]");
        }

        static void Main(string[] args)
        {


            for (var index = 0; index < args.Length; index++)
            {
                var arg = args[index];
                if (arg.Equals("--config", StringComparison.InvariantCultureIgnoreCase))
                {
                    AddConnectionStrings(args[index + 1], args[index+2]);
                    index+=2;
                }
            }

            if (args[0].Equals("serialize", StringComparison.InvariantCultureIgnoreCase))
            {
                Serialize();
            }
            else
            {
                Deserialize();
            }


        }

        private static void Deserialize()
        {
            throw new NotImplementedException();
        }

        private static void Serialize()
        {
            throw new NotImplementedException();
        }

        private static void AddConnectionStrings(string path, string databasename)
        {
            using (var fileStream = File.OpenRead(path))
            {
                var config = new ConfigXmlDocument();
                config.Load(fileStream);
                foreach (XmlNode configNode in config.SelectNodes("connectionStrings/add"))
                {
                    if (!configNode.Attributes["name"].Value.Equals(databasename))
                    {
                        continue;
                    }
                    ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings(
                          "sitecoredb",
                    configNode.Attributes["connectionString"].Value
                        ));
                }
            }
        }

     
    }


   
}
