﻿using Pk3DSRNGTool.Core;

namespace Pk3DSRNGTool
{
    public class ID6 : IDResult
    {
        public uint RandNum { get; }
        public string Status { get; }

        public ushort TID { get; }
        public ushort SID { get; }
        public ushort TSV { get; }

        public ID6(uint rand, string str = null)
        {
            RandNum = rand;
            TID = (ushort)(RandNum & 0xFFFF);
            SID = (ushort)(RandNum >> 16);
            TSV = (ushort)((TID ^ SID) >> 4);
            Status = str;
        }
    }
}
