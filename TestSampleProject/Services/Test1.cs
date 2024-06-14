using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualBasic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using TestSampleProject1.DAL;
using TestSampleProject1.Entities;
using TestSampleProject1.Services;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TestSampleProject1.Services
{
    public class Test1 : ITest1
    {
        private DBContext context = null;
        private static IConfiguration _configuration;
        public List<UserDetails> users;
        internal object name;
        private string msg = "";

        public Test1(DBContext context, IConfiguration configuration)
        {
            this.context = context;
            _configuration = configuration;
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
        public List<UserDetails> GetUserDetails(int id)
        {
            try
            {
                // This For Through the SP 

                List<UserDetails> users = new List<UserDetails>();

                //users = this.context.ExecuteSP<UserDetails>("SelectTest1", 0).ToList();

                // This for through the Entity Framework
                //users = this.context.Test1.ToList();
                users = this.context.Test1.Where(a => a.SNO > id).OrderBy(a => a.SNO).ToList();
                //users = this.context.Test1.Where(a => a.SNO > 3).ToList().Where(a => a.SNO == 5).ToList();

                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SaveUserDetails(string Name, string Location)
        {
            try
            {
                var User = new UserDetails
                {
                    NAME = Name,
                    LOCATION = Location
                };

                //ExecuteNonQuery
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@Name", User.NAME));
                param.Add(new SqlParameter("@Location", User.LOCATION));
                int ival = this.context.ExecuteNonQuery("SaveTest1", param.ToArray());
                //End

                // This for through the Entity Framework
                //this.context.Test1.Add(User);
                //this.context.SaveChanges();

                msg = "Successfully Saved";
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
                msg = ex.Message.ToString();
                return msg;
            }
        }

        public string UpdateUserDetails(int id, string Name, string Location)
        {
            try
            {
                var User = new UserDetails
                {
                    SNO = id,
                    NAME = Name,
                    LOCATION = Location
                };

                //ExecuteNonQuery
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@SNo", User.SNO));
                param.Add(new SqlParameter("@Name", User.NAME));
                param.Add(new SqlParameter("@Location", User.LOCATION));
                int ival = this.context.ExecuteNonQuery("UpdateTest1", param.ToArray());
                //End

                // This for through the Entity Framework
                //this.context.Test1.Update(User);
                //this.context.SaveChanges();

                msg = "Successfully Updated";
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
                msg = ex.Message.ToString();
                return msg;
            }
        }

        public string DeleteUserDetails(int SlNo)
        {
            try
            {
                var User = new UserDetails
                {
                    SNO = SlNo,
                };

                //ExecuteNonQuery
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@SNo", User.SNO));
                int ival = this.context.ExecuteNonQuery("DeleteTest1", param.ToArray());
                //End

                // This for through the Entity Framework
                //this.context.Test1.Remove(User);
                //this.context.SaveChanges();

                msg = "Successfully Deleted";
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
                msg = ex.Message.ToString();
                return msg;
            }
        }

    }
}
