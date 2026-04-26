using System.IO;
using System.Xml.Serialization;

namespace MiniOneNote
{
    public static class FileManager
    {
        // SAVE
        public static void Save(Page page, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Page));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(fs, page);
            }
        }

        // LOAD
        public static Page Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Page));

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return (Page)serializer.Deserialize(fs);
            }
        }
    }
}