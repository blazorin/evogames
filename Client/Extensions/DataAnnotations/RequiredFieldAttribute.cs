using System.ComponentModel.DataAnnotations;

namespace Client.Extensions.DataAnnotations
{
    public class RequiredFieldAttribute : RequiredAttribute
    {
        public RequiredFieldAttribute()
        {
            ErrorMessage = "This field is required";
        }
    }
}