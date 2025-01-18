namespace MusteriTakipSistemi.Models
{
    public class Call
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CallDate { get; set; }
        public string Notes { get; set; }
        public float Status { get; set; }
    }
}
