using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared_Models.Models
{
    public struct Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Color(byte r, byte g, byte b) {
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }
}
