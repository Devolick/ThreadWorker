using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadWorker.Code
{
    public class TokenManager
    {
        private readonly Token token;

        private TokenManager() { }
        public TokenManager(Token token)
        {
            this.token = token;
        }


    }
}
