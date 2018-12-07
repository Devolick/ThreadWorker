namespace ThreadWorker.Code
{
    public class TokenManager
    {
        public int TaskIndex
        {
            get => token.TaskIndex;
        }
        public Context Context
        {
            get => token.Context;
        }

        private readonly Token token;

        private TokenManager() { }
        public TokenManager(Token token)
        {
            this.token = token;
        }


    }
}
