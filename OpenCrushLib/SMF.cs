using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LibOpenCrush
{
    public class SMF
    {
        private Stream file;
        private Metadata metadata;
        public SMFMetadata Metadata;
        public Dictionary<string, PartMetadata> Parts;
        public SMF(Metadata metadata, Stream stream)
        {
            this.metadata = metadata;
            this.file = stream;
            Parts = new Dictionary<string, PartMetadata>();
            ParseSimpleModelFile();
        }
        void ParseSimpleModelFile()
        {
            try                                                     // Attempt to read colortable (768 bytes, Adobe color table used for 256-color raw files)
            {
                StreamReader file = new StreamReader(this.file);
                this.Metadata = new SMFMetadata();

                string[] numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Metadata.ModelType = numbers[0];
                //Console.WriteLine(numbers[0]);      // String ID model type

                numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Metadata.Version = uint.Parse(numbers[0]);
                //Console.WriteLine(numbers[0]);      // Version number (EVO1 uses Version4)

                numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Metadata.PartCount = uint.Parse(numbers[0]);       // Total number of parts in the file.
               // Console.WriteLine(numbers[0]);      // Part count

                numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Metadata.AutoDetailFlag = uint.Parse(numbers[0]);
                //Console.WriteLine(numbers[0]);      // autoDetailFlag
                Metadata.DigitalPixelHeight = uint.Parse(numbers[1]);
                //Console.WriteLine(numbers[1]);      // detailPixelHeight

                String[] partName = new String[Metadata.PartCount];
                for (int i = 0; i < Metadata.PartCount; i++)
                {
                    var data = new PartMetadata();
                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    partName[i] = numbers[0];   // part name
                    data.Name = partName[i];

                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    data.Status = uint.Parse(numbers[0]);
                    // Status? (Always 1)

                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    data.Version = uint.Parse(numbers[0]);
                    // Part Version (Always 1)

                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    data.VertexCount = uint.Parse(numbers[0]);
                    data.FrameCount = uint.Parse(numbers[1]);
                    data.FaceCount = uint.Parse(numbers[2]);
                    data.Dummy = uint.Parse(numbers[3]);
                    // &vertexCount,&frameCount,&faceCount,&dummy

                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    // v1?
                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    data.kd = double.Parse(numbers[0]);
                    data.ks = double.Parse(numbers[1]);
                    data.power = uint.Parse(numbers[2]);
                    data.Transparent = uint.Parse(numbers[3]);
                    data.envMap = uint.Parse(numbers[4]);
                    data.TextureFileName = numbers[5];
                    // &kd, &ks, &power, &transarent,  		&envMap,    TEXTURE FILE NAME
                    numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    data.BumpFileName = numbers[0];
                    data.Verticies = new Vertex[data.VertexCount];
                    for (int k = 0; k < data.VertexCount; k++)       // Change to vertex count
                    {
                        numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        var vx = new Vertex()
                        {
                            x = float.Parse(numbers[0]),
                            y = float.Parse(numbers[1]),
                            z = float.Parse(numbers[2]),
                            nx = float.Parse(numbers[3]),
                            ny = float.Parse(numbers[4]),
                            nz = float.Parse(numbers[5]),
                            u = float.Parse(numbers[6]),
                            v = float.Parse(numbers[7]),

                        };
                        data.Verticies[k]=vx;
                    }
                    data.Faces = new Face[data.FaceCount];
                    for (int j = 0; j < data.FaceCount; j++)       // Change to face count
                    {

                        numbers = file.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        var face = new Face()
                        {
                            v1 = int.Parse(numbers[0]),
                            v2 = int.Parse(numbers[1]),
                            v3 = int.Parse(numbers[2])
                        };
                        data.Faces[j] = face;
                    }       // Look back for next part E

                    Parts.Add(data.Name, data);
                }


            }
            catch (IOException e)
            {
                string debugStr = "ERROR: Failed to load SMF: " + metadata.Name + "!";
                Console.WriteLine(debugStr);
                Console.WriteLine(e);
            }
        }
    }
}
