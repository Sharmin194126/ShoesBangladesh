using System.Text.Json.Serialization;

namespace ShoesBangladeshWebApp.Response
{
    public class AuthResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("user")]
        public UserResponse? User { get; set; }
    }

    public class UserResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
    }
}
