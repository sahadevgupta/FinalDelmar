using FormsLoyalty.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormsLoyalty.Services
{
    public interface IMagazineManager
    {
        Task<string> GetAuthorizationToken(string Username, string Password);
        Task<List<MagazineModel>> GetMagazinesByToken(string token);
    }
}