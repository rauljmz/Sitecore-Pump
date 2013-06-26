using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.Data.Serialization.ObjectModel;
using Sitecore.Pump.Entities;

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
                Deserialize(args[1]);               
            }


        }

        private static void Deserialize(string path )
        {
           
            var dir = new DirectoryInfo(path);
            if(!dir.Exists) throw new FileNotFoundException(path);


           

            foreach (var file in dir.GetFiles("*.item"))
            {
                Console.Out.WriteLine(file.FullName);
               
                using (TextReader reader = new StreamReader(file.OpenRead()))
                {
                    var syncitem = SyncItem.ReadItem(new Tokenizer(reader));
                    Console.WriteLine(syncitem.Name);
                    Sync(syncitem);                  
                }
            }

            foreach (var subdir in dir.GetDirectories())
            {
                Deserialize(subdir.FullName);
            }

        }

        private static void Sync(SyncItem syncitem)
        {
            var sitecoreDb = new SitecoreDB();
            dynamic itemCreated =
                sitecoreDb.Items.All(where: "WHERE ID=@0", args: syncitem.ID, columns: "Created").FirstOrDefault();
            var item = new
                {
                    ID = syncitem.ID,
                    Name = syncitem.Name,
                    TemplateID = syncitem.TemplateID,
                    MasterID = syncitem.MasterID,
                    ParentID = syncitem.ParentID,
                    Updated = DateTime.Now,
                    Created = itemCreated != null ? itemCreated.Created : DateTime.Now
                };

            if (itemCreated != null)
            {
                sitecoreDb.Items.Update(item, item.ID);
            }
            else
            {
                sitecoreDb.Items.Insert(item);
            }
        }

        private static void Serialize()
        {
            var items = new Items();
          
            foreach (var item in items.All())
            {
                Console.Out.WriteLine(item.Name);
            }
        }



        private static void AddConnectionStrings(string path, string databasename)
        {
            System.Configuration.Configuration config =
         ConfigurationManager.OpenExeConfiguration
                    (ConfigurationUserLevel.None);

            // Add an Application Setting.
            config.AppSettings.Settings.Add("ModificationDate",
                           DateTime.Now.ToLongTimeString() + " ");

            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);


            using (var fileStream = File.OpenRead(path))
            {
                var configuration = new ConfigXmlDocument();
                configuration.Load(fileStream);
                foreach (XmlNode configNode in configuration.SelectNodes("connectionStrings/add"))
                {
                    if (!configNode.Attributes["name"].Value.Equals(databasename))
                    {
                        continue;
                    }
                    config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(
                          "sitecoredb",
                    configNode.Attributes["connectionString"].Value
                        ));
                }
            }
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
        }

     
    }


   
}
