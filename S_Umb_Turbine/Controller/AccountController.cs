using Newtonsoft.Json;
using RumblingRhino.Util;
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
        private NoLogger log = new NoLogger();
        private const string PARTIAL_VIEW_PATH = "~/Views/Partials/Account/";

        public ActionResult RenderChangePassword()
        {
            return PartialView(string.Format("{0}{1}", PARTIAL_VIEW_PATH, "_ChangePassword.cshtml"));
        }

        public JsonResult ChangePassword(ChangePasswordModel model) {
            ChangePasswordViewModel message = new ChangePasswordViewModel();
            try {
                if (model.Password.ToLower() == "fejl") { throw new Exception("Sikke en fejl..."); }
                if (model.Password.Length >= 10)
                {
                    if (model.Password == model.PasswordRepeat && !string.IsNullOrWhiteSpace(model.Password))
                    {
                        MembershipUser member = Membership.GetUser();
                        if (member != null)
                        {
                            member.ChangePassword(member.ResetPassword(), model.Password);
                            message.Status = "SUCCESS";
                        }
                        else
                        {
                            message.Status = "ERROR";
                            message.Message = "NOT LOGGED IN";
                        }
                    }
                    else
                    {
                        message.Status = "ERROR";
                        message.Message = "NOT IDENTICAL OR EMPTY";
                    }
                }
                else
                {
                    message.Status = "ERROR";
                    message.Message = "NOT ENOUGH CHARACTERS";
                }
            }
            catch (Exception e)
            {
                log.ErrorException("Error in ChangePassword",e);
                message.Status = "EXCEPTION";
                message.Message = "AN UNKNOWN ERROR OCCURRED";
            }
                        
            return Json(JsonConvert.SerializeObject(message, Formatting.Indented), JsonRequestBehavior.AllowGet);
        }
    }
}