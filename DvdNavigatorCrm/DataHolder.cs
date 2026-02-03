using System.IO;

namespace DvdNavigatorCrm;

public class DataHolder
{
    Stream stream;
    long streamOffset;
    byte[] data;

    public DataHolder(Stream stream, int length)
    {
        this.Length = length;
        this.stream = stream;
        this.streamOffset = stream.Position - length;
    }

    public DataHolder(byte[] data)
    {
        this.data = data;
        this.Length = data.Length;
    }

    public int Length { get; private set; }
    public byte[] Data { get { return this.data; } }

    public void LoadInMemory()
    {
        if (this.data == null)
        {
            this.data = AllocateBuffer(this.Length);
            this.stream.Seek(this.streamOffset, SeekOrigin.Begin);
            this.stream.Read(data, 0, this.Length);
        }
    }

    public void ReleaseFromMemory()
    {
        if (this.stream != null)
        {
            this.data = null;
        }
    }

    static byte[] AllocateBuffer(int length)
    {
        return new byte[length];
    }
}

