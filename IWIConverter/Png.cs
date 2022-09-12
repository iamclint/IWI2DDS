using Pfim;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IWI2DDS
{
    internal class Png
    {
        public static void save(string output, MemoryStream x)
        {
            using (var image = Pfimage.FromStream(x, new PfimConfig()))
            {
                PixelFormat format;
                switch (image.Format)
                {
                    case Pfim.ImageFormat.Rgba32:
                        format = PixelFormat.Format32bppArgb;
                        break;
                    default:
                        // see the sample for more details
                        throw new NotImplementedException();
                }

                byte[] data_ptr = image.Data;
                image.Data.CopyTo(data_ptr, 0);
                var handle = GCHandle.Alloc(data_ptr, GCHandleType.Pinned);
                try
                {
                    var data = Marshal.UnsafeAddrOfPinnedArrayElement(data_ptr, 0);
                    var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                    bitmap.Save(Path.ChangeExtension(output, ".png"), System.Drawing.Imaging.ImageFormat.Png);
                }
                finally
                {
                    handle.Free();
                }


            }
            x.Close();
        }
    }
}
