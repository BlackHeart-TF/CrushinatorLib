using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LibOpenCrush
{
    public class TIF
    {
        private Stream file;
        private Metadata metadata;
        public TIFMetadata Metadata;
        //public Dictionary<string, PartMetadata> Parts;
        public TIF(Metadata metadata, Stream stream)
        {
            this.metadata = metadata;
            this.file = stream;
            //Parts = new Dictionary<string, PartMetadata>();
            ParseTagImageFile();
        }

        private void ParseTagImageFile()
        {
            var meta = new TIFMetadata();
            meta.IFDs = new List<IFD>();
            meta.ByteOrder = metadata.FileStream.ReadBytes(2);
            meta.Version = metadata.FileStream.ReadShort();
            meta.FirstIFDOffset = metadata.FileStream.ReadUInt();
            meta.IFDCount = metadata.FileStream.ReadShort(meta.FirstIFDOffset);
            Metadata = meta;

            for (int i = 0; i < meta.IFDCount; i++)
            {
                var ifd = new IFD();
                ifd.Tag = (Tag)metadata.FileStream.ReadShort();
                ifd.FieldType = (DataTypes)metadata.FileStream.ReadShort();
                ifd.ValueCount = metadata.FileStream.ReadUInt();
                ifd.FileOffset = metadata.FileStream.ReadUInt();

                meta.IFDs.Add(ifd);
            }
            meta.NextIFD = metadata.FileStream.ReadUInt();
            
        }
    }
}
