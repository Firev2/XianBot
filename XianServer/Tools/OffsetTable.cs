using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace XianServer.Tools
{
    public class OffsetTable
    {
        public static readonly OffsetTable SeaTable = new OffsetTable("seaOffset.txt");
        public static readonly OffsetTable EuTable = new OffsetTable("euOffset.txt");

        public Dictionary<int,int> Offsets
        {
            get
            {
                return m_offsets;
            }
        }

        private string m_fileName;
        private Dictionary<int, int> m_offsets;

        public OffsetTable(string fileName)
        {
            m_fileName = fileName;
            m_offsets = new Dictionary<int, int>();
        }

        public void Cache()
        {
            m_offsets.Clear(); 

            var lines = File.ReadAllLines(m_fileName);

            foreach (var line in lines)
            {
                if (line.StartsWith("//"))
                    continue;

                var split = line.Split('|');

                int offset = Int32.Parse(split[1]);
                int value = Int32.Parse(split[2], NumberStyles.HexNumber);

                m_offsets.Add(offset, value);
            }
        }
    }
}
