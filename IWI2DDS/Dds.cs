using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWI2DDS
{
    internal class Dds
    {
        public static void save(string output, MemoryStream x)
        {
            using (BinaryWriter sw = new BinaryWriter(File.OpenWrite(Path.ChangeExtension(output, ".dds"))))
            {
                sw.Write(x.ReadAllBytes());
                sw.Close();
            }
            x.Close();
        }
    }
}
