using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ModifieurFermette.Models
{
    [Serializable]
    [XmlRoot()]
    public class ConfigClass
    {
        [XmlElement("ConnectionString")]
        public string sChConn { get; set; }

        public ConfigClass()
        {
            this.sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=NoPathToDB;Integrated Security=True";
        }

        /// <summary>
        /// Permet de sérialiser la classe de configuration
        /// </summary>
        /// <param name="FileName"></param>
        public void SerializeToFile(string FileName)
        {
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                XmlSerializer xs = new XmlSerializer(this.GetType());
                xs.Serialize(sw, this);
                sw.Close();
            }
        }

        public static ConfigClass DeserializeFromFile(string FileName)
        {
            using (StreamReader sr = new StreamReader(FileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(ConfigClass));
                ConfigClass rep = (ConfigClass)xs.Deserialize(sr);
                sr.Close();
                return rep;
            }
        }
    }
}
