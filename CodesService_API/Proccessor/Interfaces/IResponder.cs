using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Dtos;
using CodesService_API.Entites;

namespace CodesService_API.Proccessor.Interfaces
{
    public interface IResponder
    {
        ResponseDto GetResponse(Codes data);
    }
}