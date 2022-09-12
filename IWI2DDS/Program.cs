using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Pfim;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System.Data;
using System.Linq;
using System.Text;

namespace IWI2DDS
{

    internal class Program
    {
        
        static void Main(string[] args)
        {
            string path = "";
            if (args.Length == 0)
            {
                //single test
                path = "test2.iwi";
                IWI2DDS iwi = new IWI2DDS(path);
                if (iwi.isValidIWI()) //validate that its an iwi file
                {
                    iwi.printData();
                    iwi.savePNG(path); //for some reason the pmif lib doesn't like you doing this twice so to png or to tga but not both?? causes memory error
                }
            }
            else
                path = args[0];

            foreach (string obj in args)
            {
                path = obj;
                IWI2DDS iwi = new IWI2DDS(path);
                if (iwi.isValidIWI()) //validate that its an iwi file
                {
                    iwi.printData();
                    Console.WriteLine("Converting " + path + " to PNG");
                    iwi.savePNG(path); //for some reason the pmif lib doesn't like you doing this twice so to png or to tga but not both?? causes memory error

                    //  Console.WriteLine("Converting to DDS");
                    //  iwi.saveDDS(path);
                    //  Console.WriteLine("Converting to TGA"); 
                    //   iwi.SaveTGA(path);

                }
            }
            Console.WriteLine("Completed, press any key to continue");
            Console.ReadKey(true);
        }
    }
}
