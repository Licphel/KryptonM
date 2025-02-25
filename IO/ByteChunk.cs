namespace KryptonM.IO;

public struct ByteChunk
{

    public int Offset, Len;
    public byte[] Bytes;

    public ByteChunk(byte[] bytes, int o, int l)
    {
        Offset = o;
        Len = l;
        Bytes = bytes;
    }

    public ByteChunk(byte[] bytes)
    {
        Offset = 0;
        Len = bytes.Length;
        Bytes = bytes;
    }

    public byte[] PartialArray
    {
        get
        {
            var bytes = new byte[Len];
            Array.Copy(Bytes, 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }

}