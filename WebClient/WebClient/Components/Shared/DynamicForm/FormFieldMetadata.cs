using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Shared.Attribute;

namespace WebClient.Components.Shared.DynamicForm;

public sealed class FormFieldMetadata
{
    public required PropertyInfo Property { get; init; }
    public required FormFieldAttribute Attribute { get; init; }
    public required string Label { get; init; }
    public required FormInputType InputType { get; init; }

    public FormOptionsAttribute? OptionsAttribute { get; init; }

    public static List<FormFieldMetadata> Build(Type modelType)
    {
        return modelType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(i =>i.GetCustomAttribute<FormFieldAttribute>() != null)
            .Where(p =>
                p.CanRead &&
                p.CanWrite &&
                p.GetIndexParameters().Length == 0)
            .Select(p =>
            {
                var attribute =
                    p.GetCustomAttribute<FormFieldAttribute>()
                    ?? new FormFieldAttribute();

                var label =
                    p.GetCustomAttribute<DisplayAttribute>()?.GetName()
                    ?? p.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                    ?? p.Name;

                var inputType = attribute.Type == FormInputType.Auto
                    ? InferInputType(p.PropertyType)
                    : attribute.Type;

                return new FormFieldMetadata
                {
                    Property = p,
                    Attribute = attribute,
                    Label = label,
                    InputType = inputType,
                    OptionsAttribute =
                        p.GetCustomAttribute<FormOptionsAttribute>()
                };
            })
            .Where(f =>
                !f.Attribute.Hidden &&
                f.InputType != FormInputType.Auto)
            .OrderBy(f => f.Attribute.Order)
            .ThenBy(f => f.Property.MetadataToken)
            .ToList();
    }

    private static FormInputType InferInputType(Type type)
    {
        var actual = Nullable.GetUnderlyingType(type) ?? type;

        if (actual == typeof(bool))
            return FormInputType.Checkbox;

        if (actual == typeof(DateTime) ||
            actual == typeof(DateOnly))
            return FormInputType.Date;

        if (actual.IsEnum)
            return FormInputType.Select;

        if (actual == typeof(string))
            return FormInputType.Text;

        if (actual == typeof(byte) ||
            actual == typeof(short) ||
            actual == typeof(int) ||
            actual == typeof(long) ||
            actual == typeof(float) ||
            actual == typeof(double) ||
            actual == typeof(decimal))
            return FormInputType.Number;

        if (type.IsArray && type != typeof(byte[]))
            return FormInputType.MultiSelect;

        if (type.IsGenericType)
        {
            var generic = type.GetGenericTypeDefinition();

            if (generic == typeof(List<>) ||
                generic == typeof(ICollection<>) ||
                generic == typeof(IEnumerable<>))
                return FormInputType.MultiSelect;
        }

        return FormInputType.Text;
    }
}
