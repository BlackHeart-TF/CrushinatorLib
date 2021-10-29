using BitMiracle.LibTiff.Classic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibOpenCrush
{
    public class Metadata
    {
        //// Minumim imetadata for all versions and EPD
        //"name" : None,
        public string Name;
        //"size" : None,
        public uint FileSize;
        //"offset" : None,
        public uint Offset;
        //// Present in EPD and POD2+
        //"timestamp" : None,
        public uint TimeStamp;
        //"checksum"  : None,
        public uint Checksum;
        //// Present in POD2+
        //"path_offset" : None,
        public uint PathOffset;
        //"size" : None,
        public uint size;
        //// Present in POD5+
        //"uncompressed_size" : None,
        public uint UncompressedSize;
        //"compression_level" : 0,
        public uint CompressionLevel;
        //// Present in POD6
        //"flags": None,
        public uint Flags;
        //"zero": 0 
        public uint Zero;
        public SubStream FileStream;
        public SMF GetSMF()
        {
            FileStream.Position = 0;
            return new SMF(this, FileStream);
        }
        public Tiff GetTIF()
        {
            FileStream.Position = 0;
            var tiff = Tiff.ClientOpen(Name, "r", FileStream, new TiffStream());
            return tiff;
        }

        public RAW GetRAW(POD pod)
        {
            var raw = new RAW(this, pod);
            return raw;
        }
        public byte[] GetBytes()
        {
            var mem = new MemoryStream();
            FileStream.Position = 0;
            FileStream.CopyTo(mem);
            return mem.ToArray();
        }
        public static int[] GetTIF_RGBA(Tiff tiff)
        {
            var height = tiff.GetField(TiffTag.IMAGELENGTH);
            var width = tiff.GetField(TiffTag.IMAGEWIDTH);
            var hInt = height[0].ToInt();
            var wInt = width[0].ToInt();
            var raster = new int[hInt * wInt];
            if (tiff.ReadRGBAImage(wInt, hInt, raster))
                return raster;
            else
                return new int[0];
        }
    }
}
