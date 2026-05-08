namespace HealthUrWelath.Application.BlueDart.Dtos
{
    public class EddResponse
    {
        public TransitResult GetDomesticTransitTimeForPinCodeandProductResult { get; set; }
    }

    public class TransitResult
    {
        public int AdditionalDays { get; set; }
        public int ApexAdditionalDays { get; set; }
        public string Area { get; set; }
        public string CityDesc_Destination { get; set; }
        public string CityDesc_Origin { get; set; }
        public string EDLMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string ExpectedDateDelivery { get; set; }
        public string ExpectedDatePOD { get; set; }
        public int GroundAdditionalDays { get; set; }
        public bool IsError { get; set; }
        public string ServiceCenter { get; set; }
    }
}
