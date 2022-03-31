using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HashidsNet;

namespace CodesService_API.Helpers
{
    public class DecodeHash
    {
        private readonly IHashids hashids;

        public DecodeHash(IHashids hashids)
        {
            this.hashids = hashids;
        }

        public int Decode(string hash){
            var decodeId = hashids.Decode(hash);

            if(decodeId.Length > 1)
            {
                StringBuilder builder = new StringBuilder();
                foreach(int number in decodeId){
                    builder.Append(number);
                }
                return int.Parse(builder.ToString());
            }

            return decodeId[0];
        } 
    }
}