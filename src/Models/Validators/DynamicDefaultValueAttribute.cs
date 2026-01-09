using System;
using System.Collections.Generic;
using System.Text;

namespace Reoreo125.Memopad.Models.Validators;

public class DynamicDefaultValueAttribute : Attribute
{
    public string SourcePropertyName { get; }
    public DynamicDefaultValueAttribute(string sourcePropertyName)
    {
        SourcePropertyName = sourcePropertyName;
    }
}
