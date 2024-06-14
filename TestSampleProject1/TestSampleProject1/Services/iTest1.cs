using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using TestSampleProject1.Entities;

namespace TestSampleProject1.Services
{
    public interface ITest1
    {
        public List<UserDetails> GetUserDetails(int id);       
        public string SaveUserDetails(string Name, string Location);
        public string UpdateUserDetails(int id, string Name, string Location);
        public string DeleteUserDetails(int id);
        
    }

}
