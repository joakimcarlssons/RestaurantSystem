using RS.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Tests.Mocks
{
    public class MockUserModel : UserModel
    {
        public string Salt { get; set; }

        public IEnumerable<RoleModel> UserRoles { get; set; }
    }
}
