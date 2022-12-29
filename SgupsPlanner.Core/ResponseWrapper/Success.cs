
namespace SgupsPlanner.Core.ResponseWrapper
{
    public class Success<T> : IResponseWrapper
    {
        public T Data { get; }

        public Success(T data)
        {
            Data = data;
        }
    }
}
