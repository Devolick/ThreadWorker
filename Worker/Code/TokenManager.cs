namespace ThreadWorker.Code
{
    /// <summary>
    /// Shell to transfer data token.
    /// </summary>
    public class TokenManager
    {
        /// <summary>
        /// Flexible class for data entry.
        /// </summary>
        public Context Context
        {
            get => token.Context;
        }

        private readonly Token token;

        private TokenManager() { }
        internal TokenManager(Token token)
        {
            this.token = token;
        }
    }
}
