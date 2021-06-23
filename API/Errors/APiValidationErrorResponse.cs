using System.Collections.Generic;
namespace API.Errors
{
    public class APiValidationErrorResponse : APiResponse
    {
        public APiValidationErrorResponse() : base(400) // bad request
        {
        }

        public IEnumerable<string> Errors { get; set; }
    }
}
