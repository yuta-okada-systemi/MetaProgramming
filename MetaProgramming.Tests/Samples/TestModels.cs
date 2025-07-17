using System;

namespace Samples.Tests;

class StringModel
{
    public string StringValue { get; set; }

    public string IntValue { get; set; }

    public string DoubleValue { get; set; }

    public string DatetimeValue { get; set; }
}

class TypeModel
{
    public string StringValue { get; set; }

    public int IntValue { get; set; }

    public double DoubleValue { get; set; }

    public DateTime DatetimeValue { get; set; }
}

class NullableTypeModel
{
    public string StringValue { get; set; }

    public int? IntValue { get; set; }

    public double? DoubleValue { get; set; }

    public DateTime? DatetimeValue { get; set; }
}
