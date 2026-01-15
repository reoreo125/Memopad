namespace Reoreo125.Memopad.Models.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class FallbackValueFromMethodAttribute : Attribute
{
    public string SourceMethodName { get; }
    public object? SourcePropertyName { get; }
    public FallbackValueFromMethodAttribute(string sourceMethodName, object? sourcePropertyName = null)
    {
        SourceMethodName = sourceMethodName;
        SourcePropertyName = sourcePropertyName;
    }
}
