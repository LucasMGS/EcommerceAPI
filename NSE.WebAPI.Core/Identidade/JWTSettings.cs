namespace NSE.WebAPI.Core.Identidade
{ 
    public class JWTSettings
    {
        public string Secret { get; set; }
        public int ExpirationHours { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}