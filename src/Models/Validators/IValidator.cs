using System.ComponentModel.DataAnnotations;

namespace Reoreo125.Memopad.Models.Validators;

public interface IValidator
{
    public ValidationResult? Validate(object value, ValidationContext validationContext);
    public bool Validate(object value);
}
