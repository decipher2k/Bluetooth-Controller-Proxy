
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    public static class Settings
    {
        public static ButtonMapping buttonMapping = new ButtonMapping();

        public static void SerializeNow()
        {

            Stream s = File.OpenWrite("settings.dat");            
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, buttonMapping);
            s.Close();
        }
        public static void DeSerializeNow()
        {
            if (File.Exists("settings.dat"))
            {
                Stream s = File.OpenRead("settings.dat");
                BinaryFormatter b = new BinaryFormatter();
                buttonMapping = (ButtonMapping)b.Deserialize(s);

                s.Close();
            }
        }
    }
}
