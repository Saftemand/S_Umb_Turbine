using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace S_Umb_Turbine.Model
{
    public class ChangePasswordModel
    {
        public string MemberNodeId { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
    }
}