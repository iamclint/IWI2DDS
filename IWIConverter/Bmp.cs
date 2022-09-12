using Pfim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IWI2DDS
{
    internal class Bmp
    {
        public static Bitmap GetBitmap(MemoryStream x)
        {
            using (var image = Pfimage.FromStream(x, new PfimConfig()))
            {
                x.Close();
                PixelFormat format;
                // Convert from Pfim's backend agnostic image format into GDI+'s image format
                switch (image.Format)
                {
                    case Pfim.ImageFormat.Rgba32:
                        format = PixelFormat.Format32bppArgb;
                        break;
                    default:
                        // see the sample for more details
                        throw new NotImplementedException();
                }

                // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                // in this snippet but useful technique if the data was going to be used in
                byte[] data_ptr = image.Data;
                image.Data.CopyTo(data_ptr, 0);
                var handle = GCHandle.Alloc(data_ptr, GCHandleType.Pinned);
                try
                {
                    var data = Marshal.UnsafeAddrOfPinnedArrayElement(data_ptr, 0);
                    var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                    return bitmap;
                }
                finally
                {
                    handle.Free();
                }

            }
        }
    }
}
