using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Pump
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionStrings = new ConnectionStrings();
           
            for (var index = 0; index < args.Length; index++)
            {
                var arg = args[index];
                if (arg.Equals("--config", StringComparison.InvariantCultureIgnoreCase))
                {                    
                    connectionStrings = new ConnectionStrings(args[index+1]);
                    index++;
                }               
            }

            var serializationManager = new SerializationManager(connectionStrings);
            if (args[0].Equals("serialize", StringComparison.InvariantCultureIgnoreCase))
            {
                serializationManager.Serialize();
            }
            else
            {
                serializationManager.Deserialize();
            }


        }

        static void DisplayUsage()
        {
            Console.WriteLine("Usage: Sitecore.Pump serialize|deserialize [databasename] [databasename] --config configfile");
        }
    }
    public class ConnectionStrings
    {
        /// <summary>
        /// Creates conn strings from a standard connectionstring file
        /// </summary>
        /// <param name="s"></param>
        public ConnectionStrings(string s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates conn strings from the app.config
        /// </summary>
        public ConnectionStrings()
        {
            throw new NotImplementedException();
        }
    }

    public class SerializationManager
    {
        private ConnectionStrings _connectionStrings;

        public SerializationManager(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public void Serialize()
        {
            throw new NotImplementedException();
        }

        public void Deserialize()
        {
            throw new NotImplementedException();
        }
    }
}
