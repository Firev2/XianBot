﻿namespace XianServer.Buffer
{
    public class BufferReader
    {
        private readonly byte[] m_buffer;
        private int m_index;

        public int Position
        {
            get
            {
                return m_index;
            }
            set
            {
                if (value < 0 || value > m_buffer.Length)
                    throw new BufferException("Out of range");

                m_index = value;
            }
        }
        public int Available
        {
            get
            {
                return m_buffer.Length - m_index;
            }
        }
        public int Length
        {
            get
            {
                return m_buffer.Length;
            }
        }

        public BufferReader(byte[] packet)
        {
            m_buffer = packet;
            m_index = 0;
        }

        private void CheckLength(int length)
        {
            if (m_index + length > m_buffer.Length || length < 0)
                throw new BufferException("Out of range");
        }

        public bool ReadBool()
        {
            return m_buffer[m_index++] != 0;
        }
        public byte ReadByte()
        {
            return m_buffer[m_index++];
        }
        public byte[] ReadBytes(int count)
        {
            CheckLength(count);
            var temp = new byte[count];
            System.Buffer.BlockCopy(m_buffer, m_index, temp, 0, count);
            m_index += count;
            return temp;
        }
        public unsafe short ReadShort()
        {
            CheckLength(2);

            short value;

            fixed (byte* ptr = m_buffer)
            {
                value = *(short*)(ptr + m_index);
            }

            m_index += 2;

            return value;
        }
        public unsafe int ReadInt()
        {
            CheckLength(4);

            int value;

            fixed (byte* ptr = m_buffer)
            {
                value = *(int*)(ptr + m_index);
            }

            m_index += 4;

            return value;
        }
        public unsafe long ReadLong()
        {
            CheckLength(8);

            long value;

            fixed (byte* ptr = m_buffer)
            {
                value = *(long*)(ptr + m_index);
            }

            m_index += 8;

            return value;
        }
        public string ReadString(int count)
        {
            CheckLength(count);

            char[] final = new char[count];

            for (int i = 0; i < count; i++)
            {
                final[i] = (char)ReadByte();
            }

            return new string(final);
        }
        public string ReadMapleString()
        {
            short count = ReadShort();
            return ReadString(count);
        }

        public void Skip(int count)
        {
            CheckLength(count);
            m_index += count;
        }

        public byte[] ToArray(bool diret = false)
        {
            if (diret)
            {
                return m_buffer;
            }
            else
            {
                var final = new byte[m_buffer.Length];
                System.Buffer.BlockCopy(m_buffer, 0, final, 0, m_buffer.Length);
                return final;
            }
        }
    }
}
