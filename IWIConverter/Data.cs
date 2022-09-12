using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

    
namespace IWI2DDS
{

    public enum IMG_FORMAT:byte
    {
        INVALID = 0,
        BITMAP_RGBA = 1,
        BITMAP_RGB = 2,
        BITMAP_LUMINANCE_ALPHA = 3,
        BITMAP_LUMINANCE = 4,
        BITMAP_ALPHA = 5,
        WAVELET_RGBA = 6,
        WAVELET_RGB = 7,
        WAVELET_LUMINANCE_ALPHA = 8,
        WAVELET_LUMINANCE = 9,
        WAVELET_ALPHA = 10,
        DXT1 = 11,
        DXT3 = 12,
        DXT5 = 13,
        DXN = 14,
        BITMAP_RGB565 = 15,
        BITMAP_RGB5A3 = 16,
        BITMAP_C8 = 17,
        BITMAP_RGBA8 = 18,
        A16B16G16R16F = 19,
        COUNT = 20
    }
    public enum DDS_FLAGS:int
    {
        DDSD_CAPS=0x1,
        DDSD_HEIGHT=0x2,
        DDSD_WIDTH=0x4,
        DDSD_PITCH=0x8,
        DDSD_PIXELFORMAT=0x1000,
        DDSD_MIPMAPCOUNT=0x20000,
        DDSD_LINEARSIZE=0x80000,
        DDSD_DEPTH=0x800000
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IWI_HEADER //size is 0x30 (48)
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public char[] tag; //0x0
        public byte version; //0x3
        public byte format; //0x4
        public byte flags; //0x5
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] dimensions; //0x6
        public int fileSize; //0xC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] texture_offset; //0x10
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DDS_HEADER
    {
        public int dwSize;
        public int dwFlags;
        public int dwHeight;
        public int dwWidth;
        public int dwPitchOrLinearSize;
        public int dwDepth;
        public int dwMipMapCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public int[] dwReserved1;
        public int px_dwSize;
        public int px_dwFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] dwFourCC;
        public int dwRGBBitCount;
        public int dwRBitMask;
        public int dwGBitMask;
        public int dwBBitMask;
        public int dwABitMask;
        public int dwCaps;
        public int dwCaps2;
        public int dwCaps3;
        public int dwCaps4;
        public int dwReserved2;
    }
}


public static class StreamExtensions
{
    public static byte[] ReadAllBytes(this Stream instream)
    {
        if (instream is MemoryStream)
            return ((MemoryStream)instream).ToArray();

        using (var memoryStream = new MemoryStream())
        {
            instream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}