using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MassoudBooks
{
    class TheSerialiingClass
    {
        public static void serialize<T>(string path, T objectToWrite) where T : new()
        {
            TextWriter textWriter;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var fileStream = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
            {
                
                using (textWriter = new StreamWriter(fileStream))
                {
                    
                    serializer.Serialize(textWriter, objectToWrite);
                }
            }
        }
        public static T deserialize<T>(string path) where T : new()
        {
            if (!File.Exists(path))
                return new T();
            TextReader reader;
            
            using (var fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (reader = new StreamReader(fileStream))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T) serializer.Deserialize(reader);
                }
            }
        }
    
        
    }
}
