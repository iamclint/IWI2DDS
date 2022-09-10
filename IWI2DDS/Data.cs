using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace IWI2DDS
{

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

    //layed ou explicit so it can be marshalled around
    [StructLayout(LayoutKind.Explicit)]
    public class DDS_HEADER
    {
        [FieldOffset(0)]
        public int dwSize;
        [FieldOffset(4)]
        public int dwFlags;
        [FieldOffset(8)]
        public int dwHeight;
        [FieldOffset(12)] 
        public int dwWidth;
        [FieldOffset(16)]
        public int dwPitchOrLinearSize;
        [FieldOffset(20)]
        public int dwDepth;
        [FieldOffset(24)]
        public int dwMipMapCount;
        [FieldOffset(28)]
        public int[] dwReserved1 = new int[11];
        [FieldOffset(72)]
        public int px_dwSize;
        [FieldOffset(76)]
        public int px_dwFlags;
        [FieldOffset(80)]
        public int dwFourCC;
        [FieldOffset(84)]
        public int dwRGBBitCount;
        [FieldOffset(88)]
        public int dwRBitMask;
        [FieldOffset(92)]
        public int dwGBitMask;
        [FieldOffset(96)]
        public int dwBBitMask;
        [FieldOffset(100)]
        public int dwABitMask;
        [FieldOffset(104)]
        public int dwCaps;
        [FieldOffset(108)]
        public int dwCaps2;
        [FieldOffset(112)]
        public int dwCaps3;
        [FieldOffset(116)]
        public int dwCaps4;
        [FieldOffset(120)]
        public int dwReserved2;
    }
}
