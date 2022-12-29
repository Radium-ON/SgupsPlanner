
namespace SgupsPlanner.Core.ResponseWrapper
{
    public class Error : IResponseWrapper
    {
        public string Message { get; }

        public Error(string message)
        {
            Message = message;
        }
    }
}
