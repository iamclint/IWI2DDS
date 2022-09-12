using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWI2DDS
{
    internal class Tga
    {
        public static void save(string output, MemoryStream x)
        {
            Bitmap p = Bmp.GetBitmap(x);
            TGASharpLib.TGA tga = new TGASharpLib.TGA(p);
            tga.Save(Path.ChangeExtension(output, "tga"));
        }
    }
}
