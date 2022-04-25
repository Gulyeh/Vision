using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Dtos;

namespace Identity_API.Processors.Interfaces
{
    public interface IEmail
    {
        EmailDataDto Build();
        Task GenerateEmailData(string baseUri);
    }
}