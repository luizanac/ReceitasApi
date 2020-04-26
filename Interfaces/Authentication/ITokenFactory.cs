namespace ReceitasApi.Authentication {
    public interface ITokenFactory {
        string GenerateRefreshToken (int size = 32);
    }
}