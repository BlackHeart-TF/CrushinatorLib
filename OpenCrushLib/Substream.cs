using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibOpenCrush
{
    public class SubStream : Stream
    {
        private Stream baseStream;
        private readonly long offset;
        private readonly long length;
        private long position;
        public SubStream(Stream baseStream, long offset, long length)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");
            if (!baseStream.CanRead) throw new ArgumentException("can't read base stream");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");

            this.baseStream = baseStream;
            this.offset = offset;
            this.length = length;

            if (baseStream.CanSeek)
            {
                baseStream.Seek(offset, SeekOrigin.Current);
            }
            else
            { // read it manually...
                const int BUFFER_SIZE = 512;
                byte[] buffer = new byte[BUFFER_SIZE];
                while (offset > 0)
                {
                    int read = baseStream.Read(buffer, 0, offset < BUFFER_SIZE ? (int)offset : BUFFER_SIZE);
                    offset -= read;
                }
            }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            long remaining = length - position;
            if (remaining <= 0) return 0;
            if (remaining < count) count = (int)remaining;
            int read = baseStream.Read(buffer, offset, count);
            position += read;
            return read;
        }
        public short ReadShort(uint? offset = null)
        {
            if (offset != null)
                Position = (uint)offset;
            var reader = new BinaryReader(this);
            return reader.ReadInt16();
        }
        public byte[] ReadBytes(int length,uint? offset = null)
        {
            if (offset != null)
                Position = (uint)offset;
            var reader = new BinaryReader(this);
            return reader.ReadBytes(length);
        }
        public uint ReadUInt(uint? offset = null)
        {
            if (offset != null)
                Position = (uint)offset;
            var reader = new BinaryReader(this);
            return reader.ReadUInt32();
        }
        private void CheckDisposed()
        {
            if (baseStream == null) throw new ObjectDisposedException(GetType().Name);
        }
        public override long Length
        {
            get { CheckDisposed(); return length; }
        }
        public override bool CanRead
        {
            get { CheckDisposed(); return true; }
        }
        public override bool CanWrite
        {
            get { CheckDisposed(); return false; }
        }
        public override bool CanSeek
        {
            get { CheckDisposed(); return baseStream.CanSeek; }
        }
        public override long Position
        {
            get
            {
                CheckDisposed();
                return position;
            }
            set 
            {
                position = value;
                baseStream.Position = this.offset + position; }
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = this.length - offset;
                    break;
            }

           return Position;
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public override void Flush()
        {
            CheckDisposed(); baseStream.Flush();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (baseStream != null)
                {
                    try { baseStream.Dispose(); }
                    catch { }
                    baseStream = null;
                }
            }
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
