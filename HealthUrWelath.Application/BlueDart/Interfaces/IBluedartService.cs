using HealthUrWelath.Application.BlueDart.Dtos;

namespace HealthUrWelath.Application.BlueDart.Interfaces
{
    public interface IBluedartService
    {
        Task<EddDto> GetEDD(string pincode);
    }
}
