using System.ComponentModel.DataAnnotations;

namespace Shared.Extensions.DataAnnotations
{
    public class RequiredFieldAttribute : RequiredAttribute
    {
        public RequiredFieldAttribute()
        {
            ErrorMessage = "This field is required";
        }
    }
}