using System;
using System.IO;

namespace XianServer.Buffer
{
    public class BufferWriter : IDisposable
    {
        private BufferStream m_stream;
        private bool m_disposed;

        public int Position
        {
            get
            {
                return (int)m_stream.Position;
            }
            set
            {
                if (value <= 0)
                    throw new BufferException("Value out of range");

                m_stream.Position = value;
            }
        }
        public bool Disposed
        {
            get
            {
                return m_disposed;
            }
        }

        public BufferWriter()
        {
            m_stream = new BufferStream();
            m_disposed = false;
        }
        public BufferWriter(short opcode) : this()
        {
            WriteShort(opcode);
        }

        private void Append(long value, int byteCount)
        {
            for (int i = 0; i < byteCount; i++)
            {
                m_stream.WriteByte((byte)value);
                value >>= 8;
            }
        }

        public void WriteBool(bool value)
        {
            ThrowIfDisposed();
            WriteByte(value ? (byte)1 : (byte)0);
        }
        public void WriteByte(byte value = 0)
        {
            ThrowIfDisposed();
            m_stream.WriteByte(value);
        }
        public void WriteBytes(params byte[] value)
        {
            WriteBytes(value, 0, value.Length);
        }
        public void WriteBytes(byte[] value, int offset, int length)
        {
            ThrowIfDisposed();
            m_stream.WriteBytes(value, offset, length);
        }
        public void WriteShort(short value = 0)
        {
            ThrowIfDisposed();
            Append(value, 2);
        }
        public void WriteInt(int value = 0)
        {
            ThrowIfDisposed();
            Append(value, 4);
        }
        public void WriteLong(long value = 0)
        {
            ThrowIfDisposed();
            Append(value, 8);
        }
        public void WriteString(string value)
        {
            ThrowIfDisposed();

            foreach (char c in value)
                WriteByte((byte)c);
        }
        public void WriteMapleString(string value)
        {
            ThrowIfDisposed();
            WriteShort((short)value.Length);
            WriteString(value);
        }

        public void WriteZero(int count)
        {
            if (count <= 0)
                throw new BufferException("Count out of range");

            for (int i = 0; i < count; i++)
                WriteByte();
        }

        public byte[] ToArray()
        {
            ThrowIfDisposed();
            return m_stream.ToArray();
        }

        private void ThrowIfDisposed()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void Dispose()
        {
            m_disposed = true;

            if (m_stream != null)
                m_stream.Dispose();

            m_stream = null;
        }
    }
}