using System.ComponentModel.DataAnnotations;

namespace Shared.Extensions.DataAnnotations
{
    public class FieldLengthAttribute : MaxLengthAttribute
    {
        public FieldLengthAttribute(int maxLenght) : base(maxLenght)
        {
            ErrorMessage = "Up to {0} chars allowed";
        }
    }
}