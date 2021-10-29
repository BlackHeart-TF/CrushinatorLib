using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LibOpenCrush
{
    public struct TIFMetadata
    {
        public byte[] ByteOrder;
        public short Version;
        public uint FirstIFDOffset;
        public short IFDCount;
        public List<IFD> IFDs;
        public uint NextIFD;
    }

    public struct IFD
    {
        public Tag Tag;
        public DataTypes FieldType;
        public uint ValueCount;
        public uint FileOffset;
    }

    public enum DataTypes : short
    {
        Byte = 1,
        ASCII = 2,
        Short = 3,
        Long = 4,
        Rational = 5
    }
    public enum Tag : ushort
    {
        NewSubFileType = 254,
        SubfileType = 255,
        ImageWidth = 256,
        ImageLength = 257,
        BitsPerSample = 258,
        Compression = 259,
        PhotometricInterpretation = 262,
        Threshholding = 263,
        CellWidth = 264,
        CellLength = 265,
        FillOrder = 266,
        DocumentName = 269,
        ImageDescription = 270,
        Make = 271,
        Model = 272,
        StripOffsets = 273,
        Orientation = 274,
        SamplesPerPixel = 277,
        RowsPerStrip = 278,
        StripByteCounts = 279,
        MinSampleValue = 280,
        MaxSampleValue = 281,
        XResolution = 282,
        YResolution = 283,
        PlanarConfiguration = 284,
        PageName = 285,
        XPosition = 286,
        YPosition = 287,
        FreeOffsets = 288,
        FreeByteCounts = 289,
        GrayResponseUnit = 290,
        GrayResponseCurve = 291,
        Group3Options = 292,
        Group4Options = 293,
        ResolutionUnit = 296,
        PageNumber = 297,
        ColorResponseUnit = 300,
        ColorResponseCurves = 301,
        ColorMap = 320,
        ExtraSamples = 338,
        PhotoShop = 34377
    }
    public class WrongUniverseException : Exception
    {
        public WrongUniverseException() : base("Error, wrong universe version. only C42 supported")
        {

        }
    }
}