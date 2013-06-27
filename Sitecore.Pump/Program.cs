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
        private  static  string _templateFieldId = "{455A3E98-A627-4B40-8035-E683A0331AC7}";
        private static Guid _unversionedFieldId = new Guid("{39847666-389D-409B-95BD-F2016F11EED5}");
 
        private static List<SyncItem> _fields;
        private static HashSet<Guid> _unversionedFields; 
        private static List<SyncItem> _otherItems; 

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

        private static void Deserialize(string path)
        {
            _fields = new List<SyncItem>();
            _otherItems = new List<SyncItem>();
            _unversionedFields = new HashSet<Guid>()
                {
                    Guid.Parse("9541E67D-CE8C-4225-803D-33F7F29F09EF"),
Guid.Parse("577F1689-7DE4-4AD2-A15F-7FDC1759285F"),
Guid.Parse("B5E02AD9-D56F-4C41-A065-A133DB87BDEB"),
Guid.Parse("19A69332-A23E-4E70-8D16-B2640CB24CC8"),
Guid.Parse("9541E67D-CE8C-4225-803D-33F7F29F09EF"),
Guid.Parse("577F1689-7DE4-4AD2-A15F-7FDC1759285F"),
Guid.Parse("B5E02AD9-D56F-4C41-A065-A133DB87BDEB"),
Guid.Parse("19A69332-A23E-4E70-8D16-B2640CB24CC8"),
Guid.Parse("9541E67D-CE8C-4225-803D-33F7F29F09EF"),
Guid.Parse("577F1689-7DE4-4AD2-A15F-7FDC1759285F"),
Guid.Parse("19A69332-A23E-4E70-8D16-B2640CB24CC8"),
Guid.Parse("B12E4906-B96B-495E-B343-CD2E92DC6347"),
Guid.Parse("9541E67D-CE8C-4225-803D-33F7F29F09EF"),
Guid.Parse("577F1689-7DE4-4AD2-A15F-7FDC1759285F"),
Guid.Parse("B5E02AD9-D56F-4C41-A065-A133DB87BDEB"),
Guid.Parse("19A69332-A23E-4E70-8D16-B2640CB24CC8")
                };

            PopulateLists(path);

            var sitecoreDb = new SitecoreDB();
            Console.Out.WriteLine("Starting with templatefields");
            foreach (var syncItem in _fields)
            {
                Sync(syncItem, sitecoreDb);
            }

            PopulateUnversionedFields(sitecoreDb);

            Console.Out.WriteLine("Doing all other items");
            foreach (var syncItem in _otherItems)
            {
                Sync(syncItem, sitecoreDb);
            }

           // sitecoreDb.SaveAll();
        }

        private static void PopulateUnversionedFields(SitecoreDB sitecoreDb)
        {

            _unversionedFields.UnionWith(
                new HashSet<Guid>(
                    sitecoreDb.SharedFields.Where(sf => sf.FieldId == _unversionedFieldId && sf.Value == "1")
                              .Select(sf => (Guid) sf.ItemId)));
        }

        private static void PopulateLists(string path)
        {
           
            var dir = new DirectoryInfo(path);
            if(!dir.Exists) throw new FileNotFoundException(path);           

            foreach (var file in dir.GetFiles("*.item"))
            {
                Console.Out.WriteLine(file.FullName);
               
                using (TextReader reader = new StreamReader(file.OpenRead()))
                {
                    var syncitem = SyncItem.ReadItem(new Tokenizer(reader));
                    if (syncitem.TemplateID == _templateFieldId)
                    {
                        _fields.Add(syncitem);
                    }
                    else
                    {
                        _otherItems.Add(syncitem);
                    }
                                   
                }
            }

            foreach (var subdir in dir.GetDirectories())
            {
                PopulateLists(subdir.FullName);
            }

        }

        private static void Sync(SyncItem syncitem, SitecoreDB sitecoreDb)
        {
            Console.Out.WriteLine(syncitem.ItemPath);
            dynamic originalItem =
                sitecoreDb.Items.FirstOrDefault(i => i.ID == Guid.Parse(syncitem.ID));


            if (originalItem != null)
            {              
                originalItem.Name = syncitem.Name;
                originalItem.TemplateID = Guid.Parse(syncitem.TemplateID);
                originalItem.MasterID = Guid.Parse(syncitem.MasterID);
                originalItem.ParentID = Guid.Parse(syncitem.ParentID);
                originalItem.Updated = DateTime.Now;
                originalItem.State = RowState.Updated;
            }
            else
            {
                sitecoreDb.Items.Add(new
                    {
                        ID = syncitem.ID, 
                        TemplateID = Guid.Parse(syncitem.TemplateID),
                        ParentID = Guid.Parse(syncitem.ParentID),
                        MasterID = Guid.Parse(syncitem.MasterID),
                        Name = syncitem.Name,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        State = RowState.New
                    });
            }

            foreach (var sharedField in syncitem.SharedFields)
            {
                var originalSharedField =
                    sitecoreDb.SharedFields.FirstOrDefault(s => s.ItemId == Guid.Parse(syncitem.ID) && s.FieldId == Guid.Parse(sharedField.FieldID));
                if (originalSharedField != null)
                {
                    originalSharedField.Value = sharedField.FieldValue;
                    originalSharedField.Updated = DateTime.Now;
                    originalSharedField.State = RowState.Updated;
                }
                else
                {
                    sitecoreDb.SharedFields.Add(new
                        {
                            ItemId = Guid.Parse(syncitem.ID),
                            FieldId = Guid.Parse(sharedField.FieldID),
                            Value = Guid.Parse(sharedField.FieldValue),
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            State = RowState.New
                        });
                }
            }

            foreach (var version in syncitem.Versions)
            {
                foreach (var field in version.Fields)
                {

                    if (IsUnversioned(field.FieldID, sitecoreDb))
                    {
                        var originalUnversionedField =
                            sitecoreDb.UnversionedFields.FirstOrDefault(
                                s =>
                                s.ItemId == Guid.Parse(syncitem.ID) && s.FieldId == Guid.Parse(field.FieldID) &&
                                s.Language == version.Language);
                        if (originalUnversionedField != null)
                        {
                            originalUnversionedField.Value = field.FieldValue;
                            originalUnversionedField.Updated = DateTime.Now;
                            originalUnversionedField.State = RowState.Updated;
                        }
                        else
                        {
                            sitecoreDb.UnversionedFields.Add(new
                            {
                                ItemId = Guid.Parse(syncitem.ID),
                                FieldId = Guid.Parse(field.FieldID),
                                Version = Guid.Parse(version.Version),
                                Value = field.FieldValue,
                                Created = DateTime.Now,
                                Updated = DateTime.Now,
                                State = RowState.New
                            });
                        }
                    }
                    else
                    {
                        var originalVersionedField =
                            sitecoreDb.VersionedFields.FirstOrDefault(
                                s =>
                                s.ItemId == Guid.Parse(syncitem.ID) && s.FieldId == Guid.Parse(field.FieldID) &&
                                s.Language == version.Language && s.Version == int.Parse(version.Version));
                        if (originalVersionedField != null)
                        {
                            originalVersionedField.Value = field.FieldValue;
                            originalVersionedField.Updated = DateTime.Now;
                            originalVersionedField.State = RowState.Updated;
                        }
                        else
                        {
                            sitecoreDb.VersionedFields.Add(new
                                {
                                    ItemId = Guid.Parse(syncitem.ID),
                                    FieldId = Guid.Parse(field.FieldID),
                                    Language = version.Language,
                                    Version = version.Version,
                                    Value = field.FieldValue,
                                    Created = DateTime.Now,
                                    Updated = DateTime.Now,
                                    State = RowState.New
                                });
                        }
                    }
                }
             
            }            
        }
        

        private static bool IsUnversioned(string fieldId, SitecoreDB sitecoreDb)
        {
           //var unversioned = sitecoreDb.SharedFields.FirstOrDefault(f => f.ItemId == Guid.Parse(fieldId) && f.FieldId == Guid.Parse(_unversionedFieldId));
           //return unversioned != null && unversioned.Value == "1";
            return _unversionedFields.Contains(Guid.Parse(fieldId));
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
