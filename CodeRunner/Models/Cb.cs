namespace CodeRunner.Models;

/// <summary>
/// XML 对应的二进制数据标记定义
/// </summary>
public static class Cb
{
    // 通用控制
    public const byte BlockStart = 0xAA;
    public const byte BlockEnd = 0xFF;
    public const byte NullValue = 0x00;

    // 顶级元素
    public const byte TestData = 0xA0;

    // Primitives
    public const byte Primitives = 0x10;
    public const byte Primitive = 0x11;
    public const byte IntValue = 0x12;
    public const byte FloatValue = 0x13;
    public const byte BoolValue = 0x14;
    public const byte StringValue = 0x15;
    public const byte Base64Data = 0x16;

    // Person
    public const byte Person = 0x20;
    public const byte Person_Id = 0x21;
    public const byte Name = 0x22;
    public const byte Age = 0x23;
    public const byte Address = 0x24;
    public const byte City = 0x25;
    public const byte ZipCode = 0x26;
    public const byte Skills = 0x27;
    public const byte Skill = 0x28;
    public const byte Skill_Level = 0x29;

    // Products
    public const byte Products = 0x30;
    public const byte Product = 0x31;
    public const byte ID = 0x32;
    public const byte Price = 0x33;

    // SpecialChars
    public const byte SpecialChars = 0x40;
    public const byte SpecialChar = 0x41;
    public const byte AngleBrackets = 0x42;
    public const byte Ampersand = 0x43;
    public const byte Chinese = 0x44;
    public const byte Emoji = 0x45;

    // EdgeCases
    public const byte EdgeCases = 0x50;
    public const byte EdgeCase = 0x51;
    public const byte EmptyElement = 0x52;
    public const byte Whitespace = 0x53;

    // 列表控制
    public const byte ListStart = 0x60;
    public const byte ListEnd = 0x61;

    // 数据类型
    public const byte Type_Int32 = 0x70;
    public const byte Type_Float = 0x71;
    public const byte Type_Bool = 0x72;
    public const byte Type_String = 0x73;
    public const byte Type_Binary = 0x74;
}

