using System;

namespace XianServer.Buffer
{
    public class BufferStream : IDisposable
    {
        public const int IncrementSize = 1024;

        private byte[] m_buffer;

        public int Position { get; set; }
        public int Length { get; private set; }

        public BufferStream()
        {
            m_buffer = new byte[IncrementSize];
        }

        private void EnsureCapacity(int length)
        {
            if (m_buffer.Length - Position < length)
            {
                int newSize = m_buffer.Length;

                do
                {
                    newSize += IncrementSize;
                }
                while (newSize < Position + length);

                Array.Resize<byte>(ref m_buffer, newSize);
            }
        }

        public void WriteBytes(byte[] bytes, int offset, int length)
        {
            if (bytes == null || length == 0)
                throw new ArgumentNullException("bytes", "Trying to write zero or null bytes");

            EnsureCapacity(length);

            System.Buffer.BlockCopy(bytes, offset, m_buffer, Position, length);

            Length += length;
            Position += length;
        }

        public void WriteByte(byte value)
        {
            EnsureCapacity(1);
            
            m_buffer[Position] = value;

            Position++;
            Length++;
        }

        public void Dispose()
        {
            Position = 0;
            Length = 0;
            m_buffer = null;
        }

        public byte[] ToArray()
        {
            var final = new byte[Length];
            System.Buffer.BlockCopy(m_buffer, 0, final, 0, Length);
            return final;

        }
    }
}
