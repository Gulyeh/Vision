using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Helpers;

namespace CodesService_API.Builders
{
    public class ResponseCodeBuilder
    {
        private ResponseCode response = new();
        public ResponseCode Build(){
            return response;
        }

        public void SetTitle(string title){
            response.Title = title;
        }

        public void SetProduct(string? productId = null){
            response.ProductId = productId;
        }

        public void SetGame(Guid? gameId = null){
            response.GameId = gameId;
        }

        public void SetCodeType(CodeTypes codeType){
            response.CodeType = codeType;
        }
    }
}