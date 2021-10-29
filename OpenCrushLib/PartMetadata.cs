using System;
using System.Collections.Generic;
using System.Text;

namespace LibOpenCrush
{
    public class PartMetadata
    {
        public string Name;
        public uint Status;
        public uint Version;
        public uint TimeStamp;
        public uint VertexCount;
        public Vertex[] Verticies;
        public uint FrameCount;
        public uint FaceCount;
        public Face[] Faces;
        public uint Dummy;
        public double kd;
        public double ks;
        public uint power;
        public uint Transparent;
        public uint envMap;
        public string TextureFileName;
        public string BumpFileName;

        public override string ToString()
        {
            return Name;
        }
    }
}
