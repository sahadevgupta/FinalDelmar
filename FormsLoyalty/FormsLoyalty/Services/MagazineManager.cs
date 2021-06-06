using FormsLoyalty.Models;
using FormsLoyalty.Repos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Services
{
    public class MagazineManager : IMagazineManager
    {
        IMagazineRepo _magazineRepo;
        public MagazineManager(IMagazineRepo magazineRepo)
        {
            _magazineRepo = magazineRepo;
        }

        public async Task<string> GetAuthorizationToken(string Username, string Password)
        {
            string methodName = "integration/admin/token/";
            var jObject = new { username = Username, password = Password };
            var token = await _magazineRepo.GetToken(jObject, methodName);
            return JsonConvert.DeserializeObject<string>(token);
        }
        public async Task<List<MagazineModel>> GetMagazinesByToken(string token)
        {
            string methodName = "mpblog/post?page=1&limit=20";
            var content = await _magazineRepo.GetMagazinesByToken(token, methodName);
            return JsonConvert.DeserializeObject<List<MagazineModel>>(content);
        }
    }
}
