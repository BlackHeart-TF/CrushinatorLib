using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibOpenCrush
{
    public class POD
    {
        ////Header fields
        // Minumim fields for all versions and EPD
        string magic;
        uint? file_count;
        long index_offset;
        string comment = null;
        // EPD and POD6 field only
        uint version;
        // Present in EPD and POD2+
        uint checksum;
        // Present in POD2+
        uint audit_file_count;
        // Present in POD3+
        // 4 Unknown fields
        uint revision;
        uint priority;
        string author = null;
        string copyright = null;
        uint size_index;
        // Only in POD5 present
        string next_pod_file = null;
        //Dict for all file entries
        public Dictionary<string,Metadata> FileTree { get; private set; } = new Dictionary<string, Metadata>();


        short index;
        string pod_file;
        FileStream podstream;
        public POD(string pod_file = null, bool parse_only_header = false)
        {
            this.index = 0;
            this.pod_file = pod_file;
            if (this.pod_file != null)
                this.parse_header();
            if (!parse_only_header)
                this.parse_file_table();
        }
        public uint Count { get
            {
                if (this.file_count == null)
                    parse_header();
                return (uint)this.file_count;
            } }

        public Stream GetFileStream(Metadata metadata)
        {
            var stream = File.OpenRead(this.pod_file);
            var sub = new SubStream(stream, metadata.Offset, metadata.FileSize);
            return sub;
        }

        private string ReadString(int length)
        {
            var output = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                var bit = (byte)podstream.ReadByte();
                    output.Add(bit);
                
                
            }
            var ascii= Encoding.ASCII.GetString(output.ToArray());
            return ascii.Split('\0')[0];
        }
        private uint ReadUInt()
        {
            var output = new byte[4];
            for (int i = 0; i < 4; i++)
                output[i] = ((byte)podstream.ReadByte());
            //if (BitConverter.IsLittleEndian)
              //  Array.Reverse(output);
            return BitConverter.ToUInt32(output, 0);
        }
        /// <summary>
        /// Reads the data from a single file inside the POD archive and returns a file - like object
        /// </summary>
        /// <param name="FileName">The file to read</param>
        /// <param name="Decompress">If the file needs to be decompressed</param>
        /// <returns></returns>
        public byte[] ReadFile(string FileName, bool decompress = false)
        {
            podstream = File.OpenRead(this.pod_file);
            podstream.Position = this.FileTree[FileName].Offset;
            var data = new byte[this.FileTree[FileName].FileSize];
            for(int i = 0; i < data.Length;i++)
            data[i]=(byte)podstream.ReadByte();
            if (decompress)
                data = this.Decompress(data);
            return data;
        }
        public string HeaderText
        {
            get
            {
                var num_files = this.Count;
                var output = string.Format("{0}: {1}\n", "POD File", this.pod_file);
                output += string.Format("{0}: {1}\r\n", "File Type", this.magic);
                output += string.Format("{0}: {1}\r\n", "Version", this.version);

                output += string.Format("{0}: {1}\r\n", "Comment", this.comment);
                if (string.IsNullOrEmpty(this.author))
                    output += string.Format("{0}: {1}\r\n", "Author", this.author);
                if (string.IsNullOrEmpty(this.copyright))
                    output += string.Format("{0}: {1}\r\n", "Copyright", this.copyright);
                if (string.IsNullOrEmpty(this.next_pod_file))
                    output += string.Format("{0}: {1}\r\n", "Next File", this.next_pod_file);

                output += string.Format("{0}: {1}\r\n", "Index Offset", this.index_offset);
                if (this.size_index > 0)
                    output += string.Format("{0}: {1}\r\n", "Index Size", this.size_index);
                if (this.checksum > 0)
                    output += string.Format("{0}: {1}\r\n", "Checksum", this.checksum);
                output += string.Format("{0}: {1}\r\n", "Size", new System.IO.FileInfo(this.pod_file).Length);
                output += string.Format("{0}: {1}\r\n", "Number of files", num_files);
                return output;
            }
        }
        /// <summary>
        /// Decompress extracted data
        /// </summary>
        /// <param name="data">The data to decompress</param>
        /// <returns></returns>
        private byte[] Decompress(byte[] data)
        {
            // TODO compressed files, seems like some sort of zlib raw deflate format(no header)
            throw new NotImplementedException("data uncompression is not implemented");
            return data;
        }
        /// <summary>
        /// verify the data of an file
        /// </summary>
        /// <param name="data">The file data to verify</param>
        /// <param name="checksum">The expected checksum</param>
        /// <returns></returns>
        public bool verify(byte[] data, uint checksum)
        {
            //TODO the checksum is not crc32(data)
            throw new NotImplementedException("data verification is not implemented");
        }

        /// <summary>
        /// Parse the POD archive header
        /// </summary>
        private void parse_header()
        {
            var COMMENT_LENGTH_POD = 0x50;
            var COMMENT_LENGTH_EPD = 0x100;
            var AUTHOR_LENGTH = 0x50;
            var COPYRIGHT_LENGTH = 0x50;
            var NEXT_ARCHIVE_LENGTH = 0x50;

            podstream = File.OpenRead(this.pod_file);

            this.magic = ReadString(4);

            // EPD
            if (this.magic == "dtxe")
            {
                // struct EPD_header { 272 bytes
                //     char   magic[4]; // always dtxe
                //     char   comment[256]; // 0x100
                //     uint32 file_count;
                //     uint32 version;
                //     uint32 checksum;
                // }
                this.magic = "EPD";
                this.comment = ReadString(COMMENT_LENGTH_EPD);
                this.file_count = ReadUInt();
                this.version = ReadUInt();
                this.checksum = ReadUInt();
                this.index_offset = podstream.Position;
            }
            else if (this.magic == "POD2")
            {
                // struct POD2_header { // 96 bytes
                // char   magic[4]; // always POD2
                // uint32 checksum;
                // char   comment[80]; // 0x50
                // int32 file_count;
                // uint32 audit_file_count;
                // }
                this.checksum = ReadUInt();
                this.comment = ReadString(COMMENT_LENGTH_POD);
                this.file_count = ReadUInt();
                this.audit_file_count = ReadUInt();
                this.index_offset = podstream.Position;
            }
            else if (this.magic == "POD3" || this.magic == "POD4" || this.magic == "POD5")
            {
                // struct POD3+_header { // 288 bytes POD3/4 / 368 bytes POD5
                // char   magic[4]; // always POD3/POD4/POD5
                // uint32 checksum;
                // char   comment[80]; // 0x50
                // uint32 file_count;
                // uint32 audit_file_count;
                // uint32 revision;
                // uint32 priority;
                // char   author[80]; // 0x50
                // char   copyright[80]; // 0x50
                // uint32 index_offset;
                // uint32 unknown10C;
                // uint32 size_index;
                // uint32 unknown114;
                // uint32 unknown118;
                // uint32 unknown11C;
                // char   next_pod_file[80]; // 0x50 // POD5 only
                // }
                this.checksum = ReadUInt();
                this.comment = ReadString(COMMENT_LENGTH_POD);
                this.file_count = ReadUInt();
                this.audit_file_count = ReadUInt();
                this.revision = ReadUInt();
                this.priority = ReadUInt();
                this.author = ReadString(AUTHOR_LENGTH);
                this.copyright = ReadString(COPYRIGHT_LENGTH);
                this.index_offset = ReadUInt();
                ReadUInt(); // skip unknown field9 at offset 0x10C
                this.size_index = ReadUInt();
                ReadUInt(); // skip unknown fieldB at offset 0x114
                ReadUInt(); // skip unknown fieldC at offset 0x118
                ReadUInt(); // skip unknown fieldD at offset 0x11C

                if (this.magic == "POD5")
                    this.next_pod_file = ReadString(NEXT_ARCHIVE_LENGTH);
            }
            else if (this.magic == "POD6")
            {
                // struct POD6_header { // 20 bytes POD6
                //     char magic[4];   // always POD6
                //     uint32 file_count;
                //     uint32 version;
                //     uint32 index_offset;
                //     uint32 size_index;
                // }
                this.file_count = ReadUInt();
                this.version = ReadUInt();
                this.index_offset = ReadUInt();
                this.size_index = ReadUInt();
                podstream.Position = this.index_offset;
            }
            else
            {
                // struct POD1_header { // 84 bytes
                // uint32 file_count;
                // char   comment[80]; // 0x50
                // }
                this.magic = "POD1";
                podstream.Position = 0;
                this.file_count = ReadUInt();
                // self.comment      = self._get_c_string(pod_file.read(COMMENT_LENGTH_POD))
                this.comment = ReadString(COMMENT_LENGTH_POD);
                this.index_offset = podstream.Position;
            }
            podstream.Dispose();
        }

        /// <summary>
        /// Parse the file table of the POD archive and populates the file directory tree
        /// </summary>
        private void parse_file_table()
        {

            if (this.FileTree == null)
                this.FileTree = new Dictionary<string, Metadata>();

            this.FileTree.Clear();

                uint DIR_ENTRY_SIZE;
            switch (this.magic)
            {
                case "POD1":
                    DIR_ENTRY_SIZE = 40;
                    break;
                case "EPD":
                    DIR_ENTRY_SIZE = 80;
                    break;
                case "POD2":
                    DIR_ENTRY_SIZE = 20;
                    break;
                case "POD3":
                    DIR_ENTRY_SIZE = 20;
                    break;
                case "POD6":
                    DIR_ENTRY_SIZE = 24;
                    break;
                default:
                    DIR_ENTRY_SIZE = 28;
                    break;
            }


            var FILE_NAME_LENGTH = 0x100;
            var FILE_NAME_LENGTH_EPD = 0x40;
            var FILE_NAME_LENGTH_POD1 = 0x20;

            podstream = File.OpenRead(this.pod_file);
            podstream.Position = this.index_offset;

            for (index = 0; index < this.file_count; index++)
            {
                var metadata = new Metadata();
                string file_name;
                if (this.magic == "POD1")
                {
                    // struct POD1_file { // 40 bytes
                    // char   file_name[32]; // Zero terminated string // 0x20
                    // uint32 file_size;
                    // uint32 file_offset;
                    // }
                    file_name = ReadString(FILE_NAME_LENGTH_POD1);
                    metadata.FileSize = ReadUInt();
                    metadata.Offset = ReadUInt();
                    metadata.UncompressedSize = metadata.FileSize;
                }
                else if (this.magic == "EPD")
                {

                    // struct EPD_file { // 80 bytes
                    //     char   file_name[64]; // Zero terminated string // 0x40
                    //     uint32 file_size;
                    //     uint32 file_offset;
                    //     uint32 file_timestamp;
                    //     uint32 file_checksum;
                    // }
                    file_name = ReadString(FILE_NAME_LENGTH_EPD);
                    metadata.FileSize = ReadUInt();
                    metadata.Offset = ReadUInt();
                    metadata.TimeStamp = ReadUInt();
                    metadata.Checksum = ReadUInt();
                    metadata.UncompressedSize = metadata.FileSize;
                }
                else if (this.magic == "POD6")
                {
                    // struct POD6_file { // 24 bytes POD6
                    //     uint32 file_path_offset;
                    //     uint32 file_size;
                    //     uint32 file_offset;
                    //     uint32 file_uncompressed_size;
                    //     uint32 file_flags;
                    //     uint32 file_zero;
                    // }
                    // char file_name[256]; // Zero terminated string // 0x100
                    // Seek to the start of the index entry
                    podstream.Position = this.index_offset + (index * DIR_ENTRY_SIZE);
                    metadata.PathOffset = ReadUInt();
                    metadata.FileSize = ReadUInt();
                    metadata.Offset = ReadUInt();
                    metadata.UncompressedSize = ReadUInt();
                    metadata.Flags = ReadUInt();
                    // metadata["compression_level"] = metadata["flags"]
                    metadata.Zero = ReadUInt();
                    // metadata["timestamp"] = metadata["zero"]
                    // metadata["checksum"] = metadata["zero"]
                    // get filename from name table
                    // Seek to the file_name entry
                    podstream.Position = this.index_offset + ((uint)this.file_count * DIR_ENTRY_SIZE) + metadata.PathOffset;
                    file_name = ReadString(FILE_NAME_LENGTH);

                    if (metadata.FileSize != metadata.UncompressedSize && (metadata.Flags & 8) == 0)

                        Console.WriteLine("Found compressed and uncompressed size mismatch for file %s", file_name);
                }
                else
                {
                    // struct POD2+_file { // 20 bytes POD2/3 / 28 POD4+
                    //     uint32 file_path_offset;
                    //     uint32 file_size; // POD4+ this is the compressed size
                    //     uint32 file_offset;
                    //     uint32 file_uncompressed_size; // POD4+ only
                    //     uint32 file_compression_level; // POD4+ only
                    //     uint32 file_timestamp;
                    //     uint32 file_checksum;
                    // }
                    // char   file_name[256]; // Zero terminated string // 0x100
                    // Seek to the start if the index entry
                    podstream.Position = this.index_offset + (index * DIR_ENTRY_SIZE);
                    metadata.PathOffset = ReadUInt();
                    metadata.FileSize = ReadUInt();
                    metadata.Offset = ReadUInt();
                    if (this.magic == "POD4" || this.magic == "POD5")
                    {
                        metadata.UncompressedSize = ReadUInt();
                        metadata.CompressionLevel = ReadUInt();
                    }
                    else
                        metadata.UncompressedSize = metadata.FileSize;
                    metadata.TimeStamp = ReadUInt();
                    metadata.Checksum = ReadUInt();

                    // get filename from name table
                    // Seek to the file_name entry
                    podstream.Position = this.index_offset + ((uint)this.file_count * DIR_ENTRY_SIZE) + metadata.PathOffset;
                    file_name = ReadString(FILE_NAME_LENGTH);

                    if (metadata.FileSize != metadata.UncompressedSize && metadata.CompressionLevel == 0)
                        Console.WriteLine("Found compressed and uncompressed size mismatch for file %s", file_name);
                }
                if (Path.DirectorySeparatorChar != '\\')
                    file_name = file_name.Replace('\\', Path.DirectorySeparatorChar);

                this.FileTree.Add(file_name, metadata);
            }
            podstream.Dispose();
        }
    }
}
