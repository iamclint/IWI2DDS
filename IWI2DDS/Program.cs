using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IWI2DDS
{

    //function that reads iwi files
    public static class IWI2DDS
    {

        public static byte[] ReadIWI(string path)
        {
            byte[] iwi = System.IO.File.ReadAllBytes(path);
            return iwi;
        }

        public static bool isValidIWI(ref byte[] data) //just validate that the file you are looking at is a valid iwi file (using ref so it doesn't copy all the bytes)
        {
            //Console.WriteLine(System.Text.Encoding.Default.GetString(data, 0, 3));
            if (System.Text.Encoding.Default.GetString(data, 0, 3) != "IWi") //check first 4 bytes for the iwi5 string
                return false;
            return true;
        }
        
        public static int[] GetSize(ref byte[] data)
        {
            int[] rval = new int[2];
            if (data.Length > 0x6)
            {
                rval[0] = BitConverter.ToInt16(data, 0x6);
                rval[1] = BitConverter.ToInt16(data, 0x8);
            }
            return rval;
        }

        public static int GetTextureSize(ref byte[] data)
        {
            int file_size = BitConverter.ToInt32(data, 0xC); //file size
            int texture_offset = BitConverter.ToInt32(data, 0x10); //texture offset
            return file_size-texture_offset;
        }
        public static byte GetFormat(ref byte[] data)
        {
            return data[4];
        }
        public static byte GetUsage(ref byte[] data)
        {
            return data[5];
        }
        public static void SaveDDS(ref byte[] data, string output)
        {
            //Generate the dds header
            int[] texture_size = GetSize(ref data);
            DDS_HEADER header = new DDS_HEADER();
            int magic = ('D' << 0) | ('D' << 8) | ('S' << 16) | (' ' << 24);
            header.dwSize = 124;
            header.dwFlags = (int)(DDS_FLAGS.DDSD_CAPS | DDS_FLAGS.DDSD_HEIGHT | DDS_FLAGS.DDSD_WIDTH | DDS_FLAGS.DDSD_PIXELFORMAT | DDS_FLAGS.DDSD_PIXELFORMAT | DDS_FLAGS.DDSD_LINEARSIZE);
            header.dwWidth = texture_size[0];
            header.dwHeight = texture_size[1];
            header.dwPitchOrLinearSize = GetTextureSize(ref data);
            header.px_dwSize = 32;
            header.px_dwFlags = 4;
            if (GetFormat(ref data) == 0xB)
                header.dwFourCC = ('D' << 0) | ('X' << 8) | ('T' << 16) | ('1' << 24); 
            else
                header.dwFourCC = ('D' << 0) | ('X' << 8) | ('T' << 16) | ('5' << 24);
            header.dwCaps = 0x1000; //DDSCAPS_TEXTURE


            //convert the header structure into a byte array so we can write it to file easily
            byte[] header_array = new byte[124];
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(124); //creates a buffer with enough room for the entire structure
                Marshal.StructureToPtr(header, ptr, false); //copies the structure data into allocated memory buffer 
                Marshal.Copy(ptr, header_array, 0, 124); //copies that memory buffer do our managed byte array for later use
            }
            finally
            {
                Marshal.FreeHGlobal(ptr); //frees up the memory it allocated
            }
            
            int texture_offset = BitConverter.ToInt32(data, 0x10); //texture offset, there are also mipmaps i ignored for now.
            int size = data.Length - texture_offset; //Best I can tell is texture offset at 0x10 contains the index in the file where the texture actually starts (header ends)
            byte[] texture_data = new byte[size];
            Array.Copy(data, texture_offset, texture_data, 0, size); //copy the data from the texture offset forward

            //write the data to file
            BinaryWriter ddsfile = new BinaryWriter(File.OpenWrite(output));
            ddsfile.Write(magic);
            ddsfile.Write(header_array);
            ddsfile.Write(texture_data);
            ddsfile.Close();

        }

    }

    internal class Program
    {
        
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: IWI2DDS <input> <output>");
                Console.ReadKey(true);
            }
            var input = args[0];
            byte[] iwi_raw_data = IWI2DDS.ReadIWI(input);
            if (IWI2DDS.isValidIWI(ref iwi_raw_data)) //validate that its an iwi file
            {
                int[] size = IWI2DDS.GetSize(ref iwi_raw_data);
                Console.WriteLine("Texture Size: " + size[0] + " x " + size[1]); //get texture size just to prove file is correct
                IWI2DDS.SaveDDS(ref iwi_raw_data, Path.ChangeExtension(input, ".dds")); //create a .dds file with the same name only changed extension
            }
            Console.ReadKey(true);
        }
    }
}
