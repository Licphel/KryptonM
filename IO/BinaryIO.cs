namespace KryptonM.IO;

public class BinaryIO
{

    private const byte
        COMPOUND = 16,
        LIST = 64,
        BYTE = 128,
        INT = 129,
        LONG = 130,
        FLOAT = 131,
        DOUBLE = 132,
        BOOL = 133,
        STR = 134,
        BYTE_ARR = 200,
        INT_ARR = 201,
        FLOAT_ARR = 202;

    public static byte GetId(object o)
    {
        switch(o)
        {
            case IBinaryCompound:
                return COMPOUND;
            case IBinaryList:
                return LIST;
            case byte:
                return BYTE;
            case int:
                return INT;
            case long:
                return LONG;
            case float:
                return FLOAT;
            case double:
                return DOUBLE;
            case bool:
                return BOOL;
            case string:
                return STR;
            case byte[]:
                return BYTE_ARR;
            case int[]:
                return INT_ARR;
            case float[]:
                return FLOAT_ARR;
        }

        return 0;
    }

    public static void Encode(IBinaryCompound compound, ByteBuffer output)
    {
        foreach(KeyValuePair<string, object> pair in compound)
        {
            var key = pair.Key;
            var val = pair.Value;

            output.WriteByte(GetId(val));
            output.WriteString(key);
            EncodePrimitive(val, output);
        }

        output.WriteByte(0); //Exit
    }

    public static IBinaryCompound Decode(ByteBuffer input)
    {
        IBinaryCompound compound = IBinaryCompound.New();

        while(true)
        {
            var id = input.ReadByte();

            if(id == 0) break;

            var key = input.ReadString();
            var data = DecodePrimitive(input, id);
            compound.Set(key, data);
        }

        return compound;
    }

    private static object DecodePrimitive(ByteBuffer input, byte id)
    {
        switch(id)
        {
            case COMPOUND:
                return Decode(input);
            case LIST:
                IBinaryList lst = IBinaryList.New();
                var type = input.ReadByte();
                var size = input.ReadInt();

                if(type == 0 && size > 0) throw new Exception("Find no type mark in BinaryList! Is the saving broken?");

                for(var i = 0; i < size; i++)
                {
                    var data = DecodePrimitive(input, type);
                    lst.Insert(data);
                }

                return lst;
            case BYTE:
                return input.ReadByte();
            case INT:
                return input.ReadInt();
            case LONG:
                return input.ReadLong();
            case FLOAT:
                return input.ReadFloat();
            case DOUBLE:
                return input.ReadDouble();
            case BOOL:
                return input.ReadBoolean();
            case STR:
                return input.ReadString();
            case BYTE_ARR:
                var len = input.ReadInt();
                var bytes = new byte[len];
                input.ReadBytes(bytes, len);
                return bytes;
            case INT_ARR:
                len = input.ReadInt();
                var ints = new int[len];
                for(var i = 0; i < len; i++) ints[i] = input.ReadInt();
                return ints;
            case FLOAT_ARR:
                len = input.ReadInt();
                var fs = new float[len];
                for(var i = 0; i < len; i++) fs[i] = input.ReadFloat();
                return fs;
        }

        return null;
    }

    private static void EncodePrimitive(object o, ByteBuffer output)
    {
        switch(o)
        {
            case IBinaryCompound:
                Encode((IBinaryCompound)o, output);
                break;
            case IBinaryList:
                IBinaryList lst = (IBinaryList)o;
                output.WriteByte(lst.Type);
                output.WriteInt(lst.Count);
                foreach(var v in lst) EncodePrimitive(v, output);

                break;
            case byte:
                output.WriteByte((byte)o);
                break;
            case int:
                output.WriteInt((int)o);
                break;
            case long:
                output.WriteLong((long)o);
                break;
            case float:
                output.WriteFloat((float)o);
                break;
            case double:
                output.WriteDouble((double)o);
                break;
            case bool:
                output.WriteBoolean((bool)o);
                break;
            case string:
                output.WriteString((string)o);
                break;
            case byte[]:
                var bytes = (byte[])o;
                output.WriteInt(bytes.Length);
                output.WriteBytes(bytes);
                break;
            case int[]:
                var ints = (int[])o;
                output.WriteInt(ints.Length);
                foreach(var i in ints) output.WriteInt(i);
                break;
            case float[]:
                var fs = (float[])o;
                output.WriteInt(fs.Length);
                foreach(var i in fs) output.WriteFloat(i);
                break;
        }
    }

    //Packed ops:

    public static void Write(IBinaryCompound compound, FileHandle file)
    {
        if(!file.Exists) file.Mkfile();

        ByteBuffer buffer = new ByteBuffer();
        Encode(compound, buffer);
        PrimitiveIO.Write(file, buffer.Output());
    }

    public static IBinaryCompound Read(FileHandle file)
    {
        if(!file.Exists) throw new FileNotFoundException($"Cannot find compound coded file at {file.Path}");

        var bytes = PrimitiveIO.Read(file);
        ByteBuffer buffer = new ByteBuffer(bytes);
        return Decode(buffer);
    }

}