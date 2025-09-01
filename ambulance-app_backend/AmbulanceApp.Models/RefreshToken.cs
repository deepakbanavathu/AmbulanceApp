namespace AmbulanceApp.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public string UserId { get; set; } // Assuming you want to link the token to a user
    }
}
