namespace CodeRunner.Models;

/// <summary>
/// XML 节点和属性名称常量
/// </summary>
public static class Cs
{
    // 根节点
    public const string TestData = "TestData";

    // Primitives 相关
    public const string Primitives = "Primitives";
    public const string Primitive = "Primitive";
    public const string IntValue = "IntValue";
    public const string FloatValue = "FloatValue";
    public const string BoolValue = "BoolValue";
    public const string StringValue = "StringValue";
    public const string Base64Data = "Base64Data";

    // Person 相关
    public const string Person = "Person";
    public const string Id = "Id"; // 同时用于Person和Product
    public const string Name = "Name"; // 同时用于Person和Product
    public const string Age = "Age";

    // Address 相关
    public const string Address = "Address";
    public const string City = "City";
    public const string ZipCode = "ZipCode";

    // Skill 相关
    public const string Skills = "Skills";
    public const string Skill = "Skill";
    public const string Level = "Level";
    public const string Value = "Value"; // 通用值字段

    // Product 相关
    public const string Products = "Products";
    public const string Product = "Product";
    public const string Price = "Price";

    // SpecialChar 相关
    public const string SpecialChars = "SpecialChars";
    public const string SpecialChar = "SpecialChar";
    public const string AngleBrackets = "AngleBrackets";
    public const string Ampersand = "Ampersand";
    public const string Chinese = "Chinese";
    public const string Emoji = "Emoji";

    // EdgeCase 相关
    public const string EdgeCases = "EdgeCases";
    public const string EdgeCase = "EdgeCase";
    public const string EmptyElement = "EmptyElement";
    public const string Whitespace = "Whitespace";

    // XML 特性常量（如果需要）
    public static class Attr
    {
        public const string Id = "id"; // 对应 [XmlAttribute("id")]
        public const string Level = "level";
    }

    // 命名空间常量（如果需要）
    public static class Ns
    {
        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Xsd = "http://www.w3.org/2001/XMLSchema";
    }
}
