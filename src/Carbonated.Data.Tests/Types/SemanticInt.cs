namespace Carbonated.Data.Tests.Types
{
    /// <summary>
    /// Semantic wrapper around an int. Used for Value Converter tests.
    /// </summary>
    public class SemanticInt
    {
        public SemanticInt(int value) { Value = value; }
        public int Value { get; }
    }
}
