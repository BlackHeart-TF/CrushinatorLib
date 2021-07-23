using System;
using System.Collections.Generic;
using System.Text;

namespace LibOpenCrush
{
    struct Metadata
    {
        //// Minumim imetadata for all versions and EPD
        //"name" : None,
        public string Name;
        //"size" : None,
        public uint Size;
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
    }
}
