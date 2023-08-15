namespace Carbonated.Data.Tests.Types;

internal record RequiredRecord(string Name, int Age);

internal record RequiredAndOptionalRecord(string Name)
{
    public int Age { get; init; }
}

internal record OptionalRecord
{
    public string Name { get; init; }
    public int Age { get; init; }
}
