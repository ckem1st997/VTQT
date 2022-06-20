namespace VTQT.Core.Domain.Security
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public int RefreshExpiresIn { get; set; }

        public string RefreshToken { get; set; }

        public string TokenType { get; set; }

        public string IdToken { get; set; }

        public string Scope { get; set; }
    }
}
