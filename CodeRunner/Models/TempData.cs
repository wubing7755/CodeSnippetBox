namespace CodeRunner.Models;

public class TempData
{
    public List<Primitive> Primitives { get; set; }
    public Person Person { get; set; }
    public List<Product> Products { get; set; }
    public List<SpecialChar> SpecialChars { get; set; }
    public List<EdgeCase> EdgeCases { get; set; }

    public TempData()
    {
        Primitives = new List<Primitive>();
        Person = new Person();
        Products = new List<Product>();
        SpecialChars = new List<SpecialChar>();
        EdgeCases = new List<EdgeCase>();
    }
}

public class Primitive
{
    public int IntValue { get; set; }
    public float FloatValue { get; set; }
    public bool BoolValue { get; set; }
    public string StringValue { get; set; }
    public byte[] Base64Data { get; set; }

    public Primitive()
    {
        IntValue = 0;
        FloatValue = 0.0f;
        BoolValue = false;
        StringValue = string.Empty;
        Base64Data = Array.Empty<byte>();
    }
}

public class Person
{
    public int Id { get; set; } // Attribute
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
    public List<Skill> Skills { get; set; }

    public Person()
    {
        Id = 0;
        Name = string.Empty;
        Age = 0;
        Address = new Address();
        Skills = new List<Skill>();
    }
}

public class  Address
{
    public string City { get; set; }
    public string ZipCode { get; set; }

    public Address()
    {
        City = string.Empty;
        ZipCode = string.Empty;
    }
}

// public enum CityType
// {
//     Beijing,
//     Shanghai,
//     Guangzhou,
//     Shenzhen
// }

public class Skill
{
    public int Level { get; set; }  // Attribute
    public string Value { get; set; }

    public Skill()
    {
        Level = 0;
        Value = string.Empty;
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product()
    {
        Id = 0;
        Name = string.Empty;
        Price = 0.0m;
    }
}

public class SpecialChar
{
    public string AngleBrackets { get; set; }
    public string Ampersand { get; set; }
    public string Chinese { get; set; }
    public string Emoji { get; set; }

    public SpecialChar()
    {
        AngleBrackets = string.Empty;
        Ampersand = string.Empty;
        Chinese = string.Empty;
        Emoji = string.Empty;
    }
}

public class EdgeCase
{
    public string EmptyElement { get; set; }
    public string Whitespace { get; set; }

    public EdgeCase()
    {
        EmptyElement = string.Empty;
        Whitespace = string.Empty;
    }
}
