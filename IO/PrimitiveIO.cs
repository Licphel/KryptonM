﻿using System.IO.Compression;

namespace KryptonM.IO;

public class PrimitiveIO
{

    public static void Write(FileHandle handler, ByteChunk data)
    {
        using FileStream stream = new FileStream(handler.Path, FileMode.OpenOrCreate);
        using MemoryStream src = new MemoryStream(data.Bytes, data.Offset, data.Len);
        using GZipStream comp = new GZipStream(stream, CompressionMode.Compress);

        src.CopyTo(comp);
    }

    public static byte[] Read(FileHandle handler)
    {
        using FileStream stream = new FileStream(handler.Path, FileMode.Open);
        using MemoryStream outp = new MemoryStream();
        using GZipStream decomp = new GZipStream(stream, CompressionMode.Decompress);

        decomp.CopyTo(outp);
        return outp.ToArray();
    }

    public static void WriteN(FileHandle handler, ByteChunk data)
    {
        using FileStream stream = new FileStream(handler.Path, FileMode.OpenOrCreate);
        using MemoryStream src = new MemoryStream(data.Bytes, data.Offset, data.Len);

        src.CopyTo(stream);
    }

    public static byte[] ReadN(FileHandle handler)
    {
        using FileStream stream = new FileStream(handler.Path, FileMode.Open);
        using MemoryStream outp = new MemoryStream();

        stream.CopyTo(outp);
        return outp.ToArray();
    }

}