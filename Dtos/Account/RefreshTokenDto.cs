namespace Aplub.Domain.Dtos.Account {
    public class RefreshTokenDto {
        public string RefreshToken { get; set; }
        public string OldJwtToken { get; set; }
    }
}