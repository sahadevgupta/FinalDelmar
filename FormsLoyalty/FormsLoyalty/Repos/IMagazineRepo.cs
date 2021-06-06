using System.Threading.Tasks;

namespace FormsLoyalty.Repos
{
    public interface IMagazineRepo
    {
        Task<string> GetMagazinesByToken(string token, string methodname);
        Task<string> GetToken(object jObject, string methodname);
    }
}