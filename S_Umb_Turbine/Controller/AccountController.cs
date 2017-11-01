using Newtonsoft.Json;
using S_Umb_Turbine.Model;
using S_Umb_Turbine.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Mvc;
using Umbraco.Web.Security;

namespace S_Umb_Turbine.Controller
{
    public class AccountController : SurfaceController
    {

        private const string PARTIAL_VIEW_PATH = "~/Views/Partials/Account/";

        public ActionResult RenderChangePassword()
        {
            return PartialView(string.Format("{0}{1}", PARTIAL_VIEW_PATH, "_ChangePassword.cshtml"));
        }

        public JsonResult ChangePassword(ChangePasswordModel model) {
            ChangePasswordViewModel message = new ChangePasswordViewModel();
            if (model.Password == model.PasswordRepeat && !string.IsNullOrWhiteSpace(model.Password)) {
                MembershipUser member = Membership.GetUser();
                if (member != null) {
                    member.ChangePassword(member.ResetPassword(), model.Password);
                    message.Status = "SUCCESS";
                } else
                {
                    message.Status = "ERROR";
                    message.Message = "Password was not updated. Member was not logged in.";
                }
            } else
            {
                message.Status = "ERROR";
                message.Message = "Password was not updated. Passwords were not identical or empty.";
            }
            return Json(JsonConvert.SerializeObject(message, Formatting.Indented), JsonRequestBehavior.AllowGet);
        }
    }
}