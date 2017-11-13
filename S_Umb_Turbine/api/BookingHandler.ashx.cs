using Newtonsoft.Json;
using RumblingRhino.Util;
using S_Umb_Turbine.Model;
using S_Umb_Turbine.Service;
using S_Umb_Turbine.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace S_Umb_Turbine.api
{
    /// <summary>
    /// Summary description for BookingHandler
    /// </summary>
    public class BookingHandler : IHttpHandler
    {

        NoLogger log = new NoLogger();

        public void ProcessRequest(HttpContext context)
        {
            string message = "";
            string action = context.Request.QueryString["action"];
            switch (action) {
                case "create":
                    message = createBooking(context);
                    break;
                case "update":
                    message = updateBooking(context);
                    break;
                case "delete":
                    message = deleteBooking(context);
                    break;
                case "findBookingsByConferenceRoomOrMember":
                    if (context.Request.Params["overview-show-only-current-bookings"] == "on") {
                        message = findBookingsByConferenceRoomAndMember(context);
                    } else
                    {
                        message = findBookingsByConferenceRoom(context);
                    }
                    break;
                case "findBookingsByMember":
                    message = findBookingsByMember(context);
                    break;
                    break;
                case "validateTime":
                    NameValueCollection param = context.Request.Params;
                    if (param["update-conference-room"] != null) {
                        string conferenceRoomId = param["update-conference-room"];
                        string bookingId = param["update-booking-id"];
                        DateTime startTime = ParseDateTime(param["update-start-date"]);
                        DateTime endTime = ParseDateTime(param["update-end-date"]);
                        if (startTime == DateTime.MinValue || endTime == DateTime.MinValue) {
                            message = "ERROR: Dates could not be parsed";
                            break;
                        }
                        message = isConferenceRoomAvailable(conferenceRoomId, startTime, endTime, bookingId);
                    } else if (param["create-conference-room"] != null) {
                        string conferenceRoomId = param["create-conference-room"];
                        DateTime startTime = ParseDateTime(param["create-start-date"]);
                        DateTime endTime = ParseDateTime(param["create-end-date"]);
                        if (startTime == DateTime.MinValue || endTime == DateTime.MinValue)
                        {
                            message = "ERROR: Dates could not be parsed";
                            break;
                        }
                        message = isConferenceRoomAvailable(conferenceRoomId, startTime, endTime);
                    }
                    break;
                case "throwerror":
                    try
                    {
                        throw new Exception("this is done on purpose...");
                    }
                    catch (Exception e)
                    {
                        log.ErrorException("Error in throwerror", e);
                    }
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(message);
        }

        private string createBooking(HttpContext context) {
            BookingModel booking = new BookingModel();
            NameValueCollection param = context.Request.Params;
            booking.CreatedByMemberNodeId = param["create-member-id"];
            booking.ConferenceRoomNodeId = param["create-conference-room"];
            booking.Title = param["create-title"];
            booking.StartTime = ParseDateTime(param["create-start-date"]);
            booking.EndTime = ParseDateTime(param["create-end-date"]);

            BookingStatusViewModel status = new BookingStatusViewModel();

            if (booking.StartTime == DateTime.MinValue || booking.EndTime == DateTime.MinValue) {
                status.Status = "ERROR";
                status.Message = "Dates could not be parsed";
                return JsonConvert.SerializeObject(status, Formatting.Indented);
            }
            
            BookingViewModel viewModel = BookingService.CreateBooking(booking);
            viewModel.ClassName = "booked-by-user";
            if (viewModel != null) {
                status.Status = "SUCCESS";
                status.Booking = viewModel;
            } else
            {
                status.Status = "EXCEPTION";
                status.Message = "Something went wrong during creation of booking.";
            }
            return JsonConvert.SerializeObject(status, Formatting.Indented);
        }

        private string updateBooking(HttpContext context)
        {
            BookingModel booking = new BookingModel();
            NameValueCollection param = context.Request.Params;
            booking.Id = param["update-booking-id"];
            booking.ConferenceRoomNodeId = param["update-conference-room"];
            booking.Title = param["update-title"];

            booking.StartTime = ParseDateTime(param["update-start-date"]);
            booking.EndTime = ParseDateTime(param["update-end-date"]);

            BookingStatusViewModel status = new BookingStatusViewModel();
            if (booking.StartTime == DateTime.MinValue || booking.EndTime == DateTime.MinValue) {
                status.Status = "ERROR";
                status.Message = "Dates could not be parsed";
                return JsonConvert.SerializeObject(status, Formatting.Indented);
            }
            
            BookingViewModel viewModel = BookingService.UpdateBooking(booking);
            viewModel.ClassName = "booked-by-user";
            if (viewModel != null)
            {
                status.Status = "SUCCESS";
                status.Booking = viewModel;
            }
            else
            {
                status.Status = "EXCEPTION";
                status.Message = "Something went wrong during update of booking.";
            }
            
            return JsonConvert.SerializeObject(status, Formatting.Indented);
        }

        private string deleteBooking(HttpContext context) {
            BookingStatusViewModel status = new BookingStatusViewModel();
            string id = context.Request.Params["delete-booking-id"];
            if (BookingService.DeleteBooking(id))
            {
                status.Status = "SUCCESS";
                status.Message = id;
            } else
            {
                status.Status = "EXCEPTION";
                status.Message = "Something went wrong during deletion of booking";
            }
            return JsonConvert.SerializeObject(status, Formatting.Indented);
        }

        private string findBookingsByConferenceRoom(HttpContext context) {
            string conferenceRoomId = context.Request.Params["overview-conference-room"] != null ? context.Request.Params["overview-conference-room"] : context.Request.QueryString["conference-room"];
            List<BookingViewModel> bookings = BookingService.FindBookingsByConferenceRoom(conferenceRoomId);
            string currentUserId = System.Web.Security.Membership.GetUser().ProviderUserKey.ToString();
            foreach (BookingViewModel booking in bookings)
            {
                if (booking.BookedById == currentUserId) {
                    booking.ClassName = "booked-by-user";
                }
                else
                {
                    booking.ClassName = "booked-by-other";
                }
            }
            return JsonConvert.SerializeObject(bookings, Formatting.Indented);
        }

        private string findBookingsByMember(HttpContext context)
        {
            List<BookingViewModel> bookings = BookingService.FindBookingsByMember(context.Request.Params["overview-member-id"]);
            string currentUserId = System.Web.Security.Membership.GetUser().ProviderUserKey.ToString();
            foreach (BookingViewModel booking in bookings)
            {
                if (booking.BookedById == currentUserId)
                {
                    booking.ClassName = "booked-by-user";
                }
                else
                {
                    booking.ClassName = "booked-by-other";
                }
            }
            return JsonConvert.SerializeObject(bookings, Formatting.Indented);
        }

        private string findBookingsByConferenceRoomAndMember(HttpContext context)
        {
            List<BookingViewModel> bookings = BookingService.FindBookingsByConferenceRoomAndMember(context.Request.Params["overview-conference-room"], context.Request.Params["overview-member-id"]);
            string currentUserId = System.Web.Security.Membership.GetUser().ProviderUserKey.ToString();
            foreach (BookingViewModel booking in bookings)
            {
                if (booking.BookedById == currentUserId)
                {
                    booking.ClassName = "booked-by-user";
                }
                else
                {
                    booking.ClassName = "booked-by-other";
                }
            }
            return JsonConvert.SerializeObject(bookings, Formatting.Indented);
        }

        public string isConferenceRoomAvailable(string conferenceRoomId, DateTime startTime, DateTime endTime, string meetingToUpdateId = "-1")
        {
            BookingStatusViewModel status = new BookingStatusViewModel();
            status = BookingService.IsConferenceRoomAvailable(conferenceRoomId, startTime, endTime, meetingToUpdateId);
            return JsonConvert.SerializeObject(status, Formatting.Indented);
        }

        private static DateTime ParseDateTime(string dateTimeString) {
            DateTime dateTime = DateTime.MinValue;

            if (!DateTime.TryParseExact(dateTimeString, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                if(!DateTime.TryParseExact(dateTimeString, "dd-MM-yyyyTHH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime))
                {
                    if (!DateTime.TryParseExact(dateTimeString, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime))
                    {
                        DateTime.TryParseExact(dateTimeString, "yyyy-MM-ddTHH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime);
                    }
                }
            }

            return dateTime;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}