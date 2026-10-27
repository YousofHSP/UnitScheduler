namespace Shared.Attribute;

public enum FormInputType
{
    Auto,
    Text,
    TextArea,
    Number,
    Password,
    Checkbox,
    Date,
    Select,
    MultiSelect
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class FormFieldAttribute : System.Attribute
{
    public FormInputType Type { get; set; } = FormInputType.Auto;
    public string? Placeholder { get; set; }
    public int Order { get; set; }
    public bool Hidden { get; set; }
    public bool ReadOnly { get; set; }
    public int ColSpan { get; set; } = 1;
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class FormOptionsAttribute : System.Attribute
{
    public string? Endpoint { get; set; }
    public string ValueField { get; set; } = "id";
    public string TextField { get; set; } = "name";
}

public sealed record SelectOption(
    string Value,
    string Text,
    bool Disabled = false
);
