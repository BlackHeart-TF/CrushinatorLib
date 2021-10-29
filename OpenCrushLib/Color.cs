using System;
using System.Collections.Generic;
using System.Text;

namespace LibOpenCrush
{
    public class Color
    {
        byte r;
        byte g;
        byte b;
        byte a;
        public Color(byte r, byte g, byte b, byte a = 0xff)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public int ToRGBA_int()
        {
            int i = (int)r;
            i = (i << 1)+g;
            i = (i << 1) + b;
            i = (i << 1) + a;
            return i;
        }
        public int ToARGB_int()
        {
            int i = (int)a;
            i = (i << 1) + r;
            i = (i << 1) + g;
            i = (i << 1) + b;
            return i;
        }
        public int ToBGRA_int()
        {
            int i = (int)b;
            i = (i << 1) + g;
            i = (i << 1) + r;
            i = (i << 1) + a;
            return i;
        }
        public byte[] ToBGRA_Byte()
        {
            var i = new byte[4];
            i[0] = b;
            i[1] = g;
            i[2] = r;
            i[3] = a;
            return i;
        }
        public byte[] ToRGB_byte()
        {
            var i = new byte[3];
            i[0] = r;
            i[1] = g;
            i[2] = b;
            return i;
        }
        public byte[] ToRGBA_byte()
        {
            var i = new byte[4];
            i[0] = r;
            i[1] = g;
            i[2] = b;
            i[3] = b;
            return i;
        }
    }
}
