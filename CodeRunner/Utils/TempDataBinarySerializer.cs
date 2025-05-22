using CodeRunner.Models;
using System.Buffers.Binary;
using System.Text;
namespace CodeRunner.Utils;

public class TempDataBinarySerializer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] ToBigEndianBinary(TempData data)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        // 顶级元素标记
        writer.Write(Cb.TestData);

        // 将 TempData.Primitives 序列化为二进制数据
        writer.Write(Cb.BlockStart);
        writer.Write(Cb.Primitives);

        // 写入Primitives列表长度
        BinaryPrimitives.WriteInt32BigEndian(GetSpan(writer, 4), data.Primitives.Count);

        // 序列化每个Primitive对象
        foreach (var primitive in data.Primitives)
        {
            SerializePrimitive(writer, primitive);
        }

        writer.Write(Cb.BlockEnd);

        return ms.ToArray();
    }


    private static void SerializePrimitive(BinaryWriter writer, Primitive primitive)
    {
        writer.Write(Cb.BlockStart);

        // Primitive对象开始标记
        writer.Write(Cb.Primitive);

        // 序列化IntValue
        writer.Write(Cb.IntValue);
        BinaryPrimitives.WriteInt32BigEndian(GetSpan(writer, 4), primitive.IntValue);

        // 序列化FloatValue
        writer.Write(Cb.FloatValue);
        var floatBytes = BitConverter.GetBytes(primitive.FloatValue);
        if (BitConverter.IsLittleEndian) Array.Reverse(floatBytes);
        writer.Write(floatBytes);

        // 序列化BoolValue
        writer.Write(Cb.BoolValue);
        writer.Write(primitive.BoolValue ? (byte)1 : (byte)0);

        // 序列化StringValue
        writer.Write(Cb.StringValue);
        WriteString(writer, primitive.StringValue);

        // 序列化Base64Data
        writer.Write(Cb.Base64Data);
        BinaryPrimitives.WriteInt32BigEndian(GetSpan(writer, 4), primitive.Base64Data.Length);
        writer.Write(primitive.Base64Data);

        writer.Write(Cb.BlockEnd);
    }

    private static void WriteString(BinaryWriter writer, string value)
    {
        if (value == null)
        {
            writer.Write(Cb.NullValue);
            return;
        }
        var bytes = Encoding.UTF8.GetBytes(value);
        // 写入字符串长度（4字节大端序）
        BinaryPrimitives.WriteInt32BigEndian(GetSpan(writer, 4), bytes.Length);
        writer.Write(bytes);
    }

    private static Span<byte> GetSpan(BinaryWriter writer, int size)
    {
        var pos = writer.BaseStream.Position;
        // 占位
        writer.Write(new byte[size]);
        return ((MemoryStream)writer.BaseStream).GetBuffer().AsSpan((int)pos, size);
    }
}
