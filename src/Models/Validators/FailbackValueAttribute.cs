using System;
using System.Collections.Generic;
using System.Text;

namespace Reoreo125.Memopad.Models.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class FailbackValueAttribute : Attribute
{
    public string SourcePropertyName { get; }
    public FailbackValueAttribute(string sourcePropertyName)
    {
        SourcePropertyName = sourcePropertyName;
    }
}
