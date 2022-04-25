using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageService_API.Entites;

namespace MessageService_API.Builders
{
    public class ChatBuilder
    {
        private Chat _chat = new();
        
        public void SetUser1(Guid user){
            _chat.User1 = user;
        }

        public void SetUser2(Guid user){
            _chat.User2 = user;
        }

        public Chat Build(){
            return _chat;
        }
    }
}