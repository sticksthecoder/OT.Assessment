namespace OT.Assessment.App.Data
{
    public class CasinoWagerDto
    {
        public Guid WagerId { get; set; }
        public string Game { get; set; }
        public string Provider { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
