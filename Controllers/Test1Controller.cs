using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Collections;
using TestSampleProject1.DAL;
using TestSampleProject1.Entities;
using TestSampleProject1.Services;
using static TestSampleProject1.Controllers.Test1Controller;

namespace TestSampleProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Test1Controller : ControllerBase
    {

        public readonly ITest1 _test1 = null;
        string returnmsg = null;
        public class UserTest
        {
            public required int SlNo;
            public required String Name;
            public required String Location;
        }
        public Test1Controller(DBContext dBContext, ITest1 test1)
        {
            this._test1 = test1;
        }

        [HttpGet("GetUserList")]
        [AllowAnonymous]
        //public IEnumerable<UserDetails> GetUserList()
        //public IEnumerable<UserDetails> GetUserList([FromQuery] Dictionary<string, string> tags)
        public IEnumerable GetUserList([FromQuery] Dictionary<string, string> tags)
        {
            int RowID = Convert.ToInt32(tags["irowid"]);
            List<UserDetails> processResponse = this._test1.GetUserDetails(RowID);
            return processResponse;

        }

        [HttpPost("SaveUserList/{name},{location}")]
        [AllowAnonymous]
        //public IEnumerable SaveUserList([FromBody] UserTest usertest, string name, string location).
        public IEnumerable SaveUserList (string name, string location)
        {
            //returnmsg = this._test1.SaveUserDetails(usertest.Name, usertest.Location);
            returnmsg = this._test1.SaveUserDetails(name, location);
            return returnmsg;
        }

        [HttpPut("UpdateUserList/{rowid},{name},{location}")]
        [AllowAnonymous]
        public IEnumerable UpdateUserList([FromBody] UserTest usertest, int rowid, string name, string location)
        {
            //returnmsg = this._test1.UpdateUserDetails(usertest.SlNo, usertest.Name, usertest.Location);
            returnmsg = this._test1.UpdateUserDetails(rowid, name, location);
            return returnmsg;
        }

        [HttpDelete("DeleteUserList/{id}")]
        [AllowAnonymous]
        public IEnumerable DeleteUserList(int id)
        {
            returnmsg = this._test1.DeleteUserDetails(id);
            return returnmsg;
        }
    }
}

