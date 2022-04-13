using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageService_API.Helpers;

namespace MessageService_API.Services.IServices
{
    public interface IBaseHttpService
    {
        void Dispose();
        Task<T?> SendAsync<T>(ApiRequest apiRequest);
    }
}