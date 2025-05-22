using CodeRunner.Models;
using System.Xml;
using System.Xml.Linq;
namespace CodeRunner.Utils;

public class TempDataDeserialize
{
    public static TValue? TryDeserialize<TValue>(Stream stream) where TValue : class
    {
        var readerSetting = new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            IgnoreComments = true,
            CloseInput = true
        };

        var reader = XmlReader.Create(stream, readerSetting);
        var document = XDocument.Load(reader, LoadOptions.SetLineInfo);

        var typeToConvert = typeof(TValue);
        var value = default(TValue);

        switch (typeToConvert.Name)
        {
            case nameof(TempData) when ParseTestData(document, out var testData):
                value = testData as TValue;
                break;
        }

        return value;
    }

    public static bool ParseTestData(XDocument document, out TempData? testData)
    {
        var root = document.Root;
        if (root.Name != Cs.TestData)
        {
            testData = null;
            return false;
        }

        var value = new TempData();

        var primitivesEle = root.Elements(Cs.Primitives);
        var personEle = root.Element(Cs.Person);
        var productsEle = root.Elements(Cs.Products);
        var specialCharsEle = root.Elements(Cs.SpecialChars);
        var edgeCasesEle = root.Elements(Cs.EdgeCases);

        foreach(var primitiveEle in primitivesEle.Elements(Cs.Primitive))
        {
            if(primitiveEle.Name == Cs.Primitive)
            {
                var primitive = new Primitive();

                foreach (var primitiveChildEle in primitiveEle.Elements())
                {
                    if (primitiveChildEle.Name == Cs.IntValue)
                    {
                        primitive.IntValue = int.Parse(primitiveChildEle.Value);
                    }

                    if (primitiveChildEle.Name == Cs.FloatValue)
                    {
                        primitive.FloatValue = float.Parse(primitiveChildEle.Value);
                    }

                    if (primitiveChildEle.Name == Cs.BoolValue)
                    {
                        primitive.BoolValue = bool.Parse(primitiveChildEle.Value);
                    }

                    if (primitiveChildEle.Name == Cs.StringValue)
                    {
                        primitive.StringValue = primitiveChildEle.Value;
                    }

                    if (primitiveChildEle.Name == Cs.Base64Data)
                    {
                        primitive.Base64Data = Convert.FromBase64String(primitiveChildEle.Value);
                    }
                }

                value.Primitives.Add(primitive);
            }
        }

        if (personEle is not null && personEle.Name == Cs.Person)
        {
            value.Person.Name = personEle.Element(Cs.Name)?.Value ?? string.Empty;
            value.Person.Age = int.Parse(personEle.Element(Cs.Age)?.Value ?? "0");

            var addressEle = personEle.Element(Cs.Address);
            if (addressEle is not null && addressEle.Name == Cs.Address)
            {
                value.Person.Address.City = addressEle.Element(Cs.City)?.Value ?? string.Empty;
                value.Person.Address.ZipCode = addressEle.Element(Cs.ZipCode)?.Value ?? string.Empty;
            }

            var skillsEle = personEle.Element(Cs.Skills);
            if (skillsEle is not null && skillsEle.Name == Cs.Skills)
            {
                foreach (var skillEle in skillsEle.Elements(Cs.Skill))
                {
                    var skill = new Skill();
                    skill.Level = int.Parse(skillEle.Attribute(Cs.Attr.Level)?.Value ?? "0");
                    skill.Value = skillEle.Value ?? string.Empty;
                    value.Person.Skills.Add(skill);
                }
            }
        }

        foreach (var productEle in productsEle.Elements(Cs.Product))
        {
            var product = new Product();

            product.Id = int.Parse(productEle.Attribute(Cs.Attr.Id)?.Value ?? "0");
            product.Name = productEle.Element(Cs.Name)?.Value ?? string.Empty;
            product.Price = decimal.Parse(productEle.Element(Cs.Price)?.Value ?? "0");

            value.Products.Add(product);
        }

        foreach (var specialCharEle in specialCharsEle.Elements(Cs.SpecialChar))
        {
            var specialChar = new SpecialChar();

            specialChar.AngleBrackets = specialCharEle.Element(Cs.AngleBrackets)?.Value ?? string.Empty;
            specialChar.Ampersand = specialCharEle.Element(Cs.Ampersand)?.Value ?? string.Empty;
            specialChar.Chinese = specialCharEle.Element(Cs.Chinese)?.Value ?? string.Empty;
            specialChar.Emoji = specialCharEle.Element(Cs.Emoji)?.Value ?? string.Empty;

            value.SpecialChars.Add(specialChar);
        }

        foreach(var edgeCaseEle in edgeCasesEle.Elements(Cs.EdgeCase))
        {
            var edgeCase = new EdgeCase();

            edgeCase.EmptyElement = edgeCaseEle.Element(Cs.EmptyElement)?.Value ?? string.Empty;
            edgeCase.Whitespace = edgeCaseEle.Element(Cs.Whitespace)?.Value ?? string.Empty;

            value.EdgeCases.Add(edgeCase);
        }

        testData = value;
        return true;
    }
}
