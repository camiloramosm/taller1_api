using Bogus;

namespace TodoList.Modules.Todos.UnitTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Faker Faker = new();

    protected static string GenerateValidTitle()
    {
        var sentence = Faker.Lorem.Sentence(3, 7);
        return sentence.Length <= 50 ? sentence : sentence.Substring(0, 50);
    }
    
    protected static string GenerateValidDescription()
    {
        var paragraph = Faker.Lorem.Paragraph(1);
        return paragraph.Length <= 200 ? paragraph : paragraph.Substring(0, 200);
    }
    
    protected static string GenerateLongTitle() => Faker.Lorem.Sentence(100);
    
    protected static string GenerateLongDescription() => string.Join(" ", Faker.Lorem.Paragraphs(50));
}
