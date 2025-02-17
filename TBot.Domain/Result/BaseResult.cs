using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Result
{
    public class BaseResult
    {
        public bool IsSuccess { get; set; }
        public string ResultMessage { get; set; } = default!;
        public BaseResult() { }
        public BaseResult(bool isSuccess, string resultMessage) 
        {
            IsSuccess = isSuccess;
            ResultMessage = resultMessage;
        }
    }
   
    public class BaseResult<T> : BaseResult
    {
        public T? Data { get; set; }
        public BaseResult() { }
        public BaseResult(bool isSuccess, string resultMessage, T data): base (isSuccess, resultMessage) 
        {
            Data = data;
        }
    }
}
