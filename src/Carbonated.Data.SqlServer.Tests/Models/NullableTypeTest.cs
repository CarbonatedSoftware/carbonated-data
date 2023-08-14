using System;

namespace Carbonated.Data.SqlServer.Tests.Models;

internal class NullableTypeTest
{
    public bool? Bool { get; set; }
    public byte? Byte { get; set; }
    public short? Short { get; set; }
    public int? Int { get; set; }
    public long? Long { get; set; }
    public float? Float { get; set; }
    public double? Double { get; set; }
    public decimal? Decimal { get; set; }
    public DateTime? DateTime { get; set; }
    public DateTime? DateTime2 { get; set; }
    public DateOnly? Date { get; set; }
    public TimeOnly? Time { get; set; }
    public Guid? GuidAsString { get; set; }
    public Guid? GuidAsUniqueId { get; set; }
    public char? Char { get; set; }
    public Numbers? IntEnum { get; set; }
    public Colors? StringEnum { get; set; }
}
