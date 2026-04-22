using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        string Register(string username, string password,string role);
        string Login(string username, string password);
    }
}
