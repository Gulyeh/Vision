using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMTPService_API.Dtos
{
    public class ResponseDto
    {
        public ResponseDto(bool _isSuccess, int status, object response)
        {
            isSuccess = _isSuccess;
            Status = status;
            Response = response;
        }

        public bool isSuccess { get; init; }
        public int Status { get; init; }
        public object Response { get; init; }
    }
}