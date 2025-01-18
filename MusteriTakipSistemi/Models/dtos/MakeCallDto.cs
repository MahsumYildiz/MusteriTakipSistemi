namespace MusteriTakipSistemi.Models.dtos
{
    public class MakeCallDto
    {
        public int CustomerId { get; set; }
        public int AdminId { get; set; }
        public float Status { get; set; }
        public string Notes { get; set; }
    }
}
