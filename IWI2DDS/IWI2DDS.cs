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

    //function that reads iwi files
    public class IWI2DDS
    {
        IWI_HEADER iwi_header_data;
        DDS_HEADER dds_header_data;
        byte[] bytes;
        bool noPicmip;
        bool isSkyMap;
        private T FromByteArray<T>(byte[] bytes, int offset, int size)
        {

            IntPtr ptr = IntPtr.Zero;
            T s;
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytes, offset, ptr, size);
                s = (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr); //frees up the memory it allocated
            }
            return s;
        }

        public IWI2DDS(string input)
        {
            bytes = System.IO.File.ReadAllBytes(input);
            iwi_header_data = FromByteArray<IWI_HEADER>(bytes, 0, Marshal.SizeOf(iwi_header_data));
            getPicmip();
            if (iwi_header_data.flags == 198)
                isSkyMap = true;
        }
        private void getPicmip()
        {
            if (isValidIWI())
            {
                if (iwi_header_data.format == (byte)IMG_FORMAT.DXT5)
                    noPicmip = true;
                else if ((iwi_header_data.flags & 0x3) == 0x0)
                {
                    int r = iwi_header_data.dimensions[1];
                    if (iwi_header_data.dimensions[0] <= iwi_header_data.dimensions[1])
                        r = iwi_header_data.dimensions[0];
                    if (r < 0x20)
                        noPicmip = true;
                }
                else
                {
                    noPicmip = true;
                }
            }
        }
        public bool isValidIWI() //just validate that the file you are looking at is a valid iwi file (using ref so it doesn't copy all the bytes)
        {
            string x = new string(iwi_header_data.tag);
            if (x != "IWi") //check the tag for IWi anything else is an invalid file
                return false;
            return true;
        }

        //converts the structure into a byte array
        public static byte[] Serialize<T>(T s)
            where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var array = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(s, ptr, true);
            Marshal.Copy(ptr, array, 0, size);
            Marshal.FreeHGlobal(ptr);
            return array;
        }
        private uint TextureSize()
        {
            uint size = 0;
            if (iwi_header_data.format == (byte)IMG_FORMAT.DXT1)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1] / 2);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.DXT3 || iwi_header_data.format == (byte)IMG_FORMAT.DXT5)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1]);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.BITMAP_RGB565)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1] * 2);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.BITMAP_RGB5A3)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1] * 2);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.BITMAP_C8)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1]);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.BITMAP_RGBA8)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1] * 4);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.A16B16G16R16F)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1] * 8);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.BITMAP_RGBA)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1] * 4);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.BITMAP_RGB)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1] * 3);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.BITMAP_LUMINANCE_ALPHA)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1] * 2);
            else if (iwi_header_data.format == (byte)IMG_FORMAT.DXT5)
                size = (uint)(iwi_header_data.dimensions[0] * iwi_header_data.dimensions[1]);
            return size;
        }

        //function to convert dds to bitmap
        private MemoryStream GetTextureData(int index = 0)
        {
            uint data_size = 0;

            //if (noPicmip) 
            //    data_size = bytes.Length - Marshal.SizeOf(iwi_header_data); //data size is file size - the header information for the texture if its a single texture file 
            //else
            //    data_size = bytes.Length - iwi_header_data.texture_offset[0];

            data_size = TextureSize();
            byte[] texture_data;
            texture_data = new byte[data_size];

            if (noPicmip) //if there isnt any picmip (copies in lower quality) get the entire file as data
                Array.Copy(bytes, Marshal.SizeOf(iwi_header_data) + (data_size * index), texture_data, 0, data_size);
            else
                Array.Copy(bytes, iwi_header_data.texture_offset[0] + (data_size * index), texture_data, 0, data_size);
            MemoryStream s = new MemoryStream(texture_data);
            return s;
        }

        private MemoryStream GetDDS(int index = 0)
        {
            //open file for read

            dds_header_data = new DDS_HEADER();
            int data_size = 0;
            if (noPicmip) //dunno seems like we could just check nopicmip flag
                data_size = bytes.Length - Marshal.SizeOf(iwi_header_data); //data size is file size - the header information for the texture if its a single texture file 
            else
                data_size = bytes.Length - iwi_header_data.texture_offset[0];

            // int magic = ('D' << 0) | ('D' << 8) | ('S' << 16) | (' ' << 24);
            dds_header_data.dwSize = Marshal.SizeOf(dds_header_data);
            dds_header_data.dwFlags = (int)(DDS_FLAGS.DDSD_CAPS | DDS_FLAGS.DDSD_HEIGHT | DDS_FLAGS.DDSD_WIDTH | DDS_FLAGS.DDSD_PIXELFORMAT | DDS_FLAGS.DDSD_PIXELFORMAT | DDS_FLAGS.DDSD_LINEARSIZE);
            dds_header_data.dwWidth = iwi_header_data.dimensions[0];
            dds_header_data.dwHeight = iwi_header_data.dimensions[1];
            dds_header_data.dwPitchOrLinearSize = data_size;
            dds_header_data.px_dwSize = 32;
            dds_header_data.px_dwFlags = 4;
            if (iwi_header_data.format == (byte)IMG_FORMAT.DXT1)
                dds_header_data.dwFourCC = "DXT1".ToCharArray();
            else
                dds_header_data.dwFourCC = "DXT5".ToCharArray();
            dds_header_data.dwCaps = 0x1000; //DDSCAPS_TEXTURE

            MemoryStream texture_data = GetTextureData(index);

            /// Buffer x = new Buffer();
            MemoryStream m = new MemoryStream();
            byte[] magic_bytes = Encoding.ASCII.GetBytes("DDS ");// BitConverter.GetBytes(magic);
            byte[] dds_data_bytes = Serialize<DDS_HEADER>(dds_header_data);
            m.Write(magic_bytes, 0, magic_bytes.Length);
            m.Write(dds_data_bytes, 0, dds_data_bytes.Length);
            m.Write(texture_data.ToArray(), 0, (int)texture_data.Length);
            texture_data.Close();
            m.Seek(0, 0);
            return m;
        }

        public void printData()
        {
            Console.WriteLine("dimensions: " + iwi_header_data.dimensions[0] + " " + iwi_header_data.dimensions[1] + " " + iwi_header_data.dimensions[2]);
            Console.WriteLine("format: " + iwi_header_data.format);
            Console.WriteLine("file size: " + iwi_header_data.fileSize);
            Console.WriteLine("texture offset: " + iwi_header_data.texture_offset[0]);
            Console.WriteLine("texture offset 1: " + iwi_header_data.texture_offset[1]);
            Console.WriteLine("texture offset 2: " + iwi_header_data.texture_offset[2]);
            Console.WriteLine("version: " + iwi_header_data.version);
            Console.WriteLine("flags: " + iwi_header_data.flags);
            Console.WriteLine("tag: " + new string(iwi_header_data.tag));
        }

        private string get_file_path_index(string base_path, int index)
        {
            return base_path + (index + 1);
        }
        
        public void saveDDS(string output) //use ref so it doesn't make copies of the data, references it directly instead
        {

            if (isSkyMap)
            {
                string base_path = Path.GetFullPath(output).Substring(0, output.LastIndexOf('.'));
                for (int i = 0; i < 6; i++)
                {
                    Dds.save(get_file_path_index(base_path, i), GetDDS(i));
                }
            }
            else
                Dds.save(output, GetDDS());
        }

        public void saveTGA(string output)
        {
            if (isSkyMap)
            {
                string base_path = Path.GetFullPath(output).Substring(0, output.LastIndexOf('.'));
                for (int i = 0; i < 6; i++)
                {
                    Tga.save(get_file_path_index(base_path, i), GetDDS(i));
                }
            }
            else
                Tga.save(output, GetDDS());
        }

        public void savePNG(string output)
        {
            if (isSkyMap)
            {
                string base_path = Path.GetFullPath(output).Substring(0, output.LastIndexOf('.'));
                for (int i = 0; i < 6; i++)
                {
                    Png.save(get_file_path_index(base_path, i), GetDDS(i));
                }
            }
            else
                Png.save(output, GetDDS());
        }

    }

}
