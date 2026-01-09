using System;
using System.Collections.Generic;
using System.Text;

namespace Reoreo125.Memopad.Models.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class FallbackValueAttribute : Attribute
{
    public string SourcePropertyName { get; }
    public FallbackValueAttribute(string sourcePropertyName)
    {
        SourcePropertyName = sourcePropertyName;
    }
}
