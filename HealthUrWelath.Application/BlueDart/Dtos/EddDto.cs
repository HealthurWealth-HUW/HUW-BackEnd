namespace HealthUrWelath.Application.BlueDart.Dtos
{
    public class EddDto
    {
        public string DestinationCity { get; set; }
        public string OriginCity { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DeliveryDateWhenNoEDD { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
