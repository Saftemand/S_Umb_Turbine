using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace S_Umb_Turbine.Controller
{
    public class BookingController : SurfaceController
    {
        private const string PARTIAL_VIEW_PATH = "~/Views/Partials/Booking/";

        public ActionResult RenderCalendar() {
            return PartialView(string.Format("{0}{1}", PARTIAL_VIEW_PATH, "_Calendar.cshtml"));
        }

        public ActionResult RenderCreateBookingModal()
        {
            return PartialView(string.Format("{0}{1}", PARTIAL_VIEW_PATH, "_CreateBookingModal.cshtml"));
        }

        public ActionResult RenderUpdateBookingModal()
        {
            return PartialView(string.Format("{0}{1}", PARTIAL_VIEW_PATH, "_UpdateBookingModal.cshtml"));
        }

        public ActionResult RenderDetailedViewBookingModal()
        {
            return PartialView(string.Format("{0}{1}", PARTIAL_VIEW_PATH, "_DetailedViewBookingModal.cshtml"));
        }

        public ActionResult RenderDeleteBookingModal()
        {
            return PartialView(string.Format("{0}{1}", PARTIAL_VIEW_PATH, "_DeleteBookingModal.cshtml"));
        }

    }
}