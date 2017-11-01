using RumblingRhino.Util;
using S_Umb_Turbine.Model;
using S_Umb_Turbine.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Umbraco.Web;

namespace S_Umb_Turbine.Service
{
    public class BookingService
    {
        private static NoLogger log = new NoLogger();
        public static BookingViewModel CreateBooking(BookingModel booking)
        {
            BookingViewModel newBooking = null;
            string id = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ApplicationSettings.Instance.DBConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("Insert Into custom_booking (CreatedByMemberNodeId, ConferenceRoomNodeId, Title, StartTime, EndTime) VALUES(@CreatedByMemberNodeId,@ConferenceRoomNodeId, @Title, @StartTime, @EndTime);SELECT SCOPE_IDENTITY()", conn))
                    {
                        cmd.Parameters.Add("@CreatedByMemberNodeId", SqlDbType.NVarChar).Value = booking.CreatedByMemberNodeId;
                        cmd.Parameters.Add("@ConferenceRoomNodeId", SqlDbType.NVarChar).Value = booking.ConferenceRoomNodeId;
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = booking.Title;
                        cmd.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = booking.StartTime;
                        cmd.Parameters.Add("@EndTime", SqlDbType.DateTime2).Value = booking.EndTime;
                        id = cmd.ExecuteScalar().ToString();
                    }
                }
                newBooking = FindBookingById(id);
            }
            catch (Exception ex)
            {
                booking = null;
                log.ErrorException("Error in CreateBooking", ex);
            }
            return newBooking;
        }

        public static BookingViewModel UpdateBooking(BookingModel booking)
        {
            BookingViewModel updatedBooking = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(ApplicationSettings.Instance.DBConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("Update custom_booking set [ConferenceRoomNodeId] = @ConferenceRoomNodeId,[Title] = @Title,[StartTime] = @StartTime,[EndTime] = @EndTime where [ID] = @Id", conn))
                    {
                        cmd.Parameters.Add("@ConferenceRoomNodeId", SqlDbType.NVarChar).Value = booking.ConferenceRoomNodeId;
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = booking.Title;
                        cmd.Parameters.Add("@StartTime", SqlDbType.DateTime2).Value = booking.StartTime;
                        cmd.Parameters.Add("@EndTime", SqlDbType.DateTime2).Value = booking.EndTime;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = booking.Id;
                        cmd.ExecuteNonQuery();
                    }
                }
                updatedBooking = FindBookingById(booking.Id);
            }
            catch (Exception ex)
            {
                updatedBooking = null;
                log.ErrorException("Error in UpdateBooking", ex);
            }
            return updatedBooking;
        }

        public static bool DeleteBooking(string bookingId) {
            bool deleted = true;
            try
            {
                using (SqlConnection conn = new SqlConnection(ApplicationSettings.Instance.DBConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("delete from [custom_booking] where [ID] = @Id", conn))
                    {
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = bookingId;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                deleted = false;
                log.ErrorException("Error in DeleteBooking", ex);
            }
            return deleted;
        }

        public static BookingViewModel FindBookingById(string id)
        {
            BookingViewModel booking = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(ApplicationSettings.Instance.DBConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [custom_booking] WHERE [Id] = @Id", conn))
                    {
                        cmd.Parameters.Add("@Id", SqlDbType.NVarChar).Value = id;
                        SqlDataReader reader = cmd.ExecuteReader();
                        UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                        if (reader.Read())
                        {
                            booking = new BookingViewModel();
                            booking.Id = reader["Id"].ToString();
                            booking.BookedById = reader["CreatedByMemberNodeId"].ToString();
                            booking.BookedByName = umbracoHelper.TypedMember(booking.BookedById).Name;
                            booking.ConferenceRoomId = reader["ConferenceRoomNodeId"].ToString();
                            booking.ConferenceRoomName = umbracoHelper.TypedContent(booking.ConferenceRoomId).Name;
                            booking.StartTime = (DateTime)reader["startTime"];
                            booking.EndTime = (DateTime)reader["endTime"];
                            booking.Title = reader["title"] as string;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                booking = null;
                log.ErrorException("Error in FindBookingById", ex);
            }

            return booking;
        }

        public static List<BookingViewModel> FindBookingsByMember(string createdByMemberNodeId)
        {
            List<BookingViewModel> bookingsMadeByMember = new List<BookingViewModel>();
            try {
                using (SqlConnection conn = new SqlConnection(ApplicationSettings.Instance.DBConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [custom_booking] WHERE [CreatedByMemberNodeId] = @createdByMemberNodeId", conn))
                    {
                        cmd.Parameters.Add("@createdByMemberNodeId", SqlDbType.NVarChar).Value = createdByMemberNodeId;
                        SqlDataReader reader = cmd.ExecuteReader();
                        UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                        while (reader.Read()) {
                            BookingViewModel booking = new BookingViewModel();
                            booking.Id = reader["Id"].ToString();
                            booking.BookedById = reader["CreatedByMemberNodeId"].ToString();
                            booking.BookedByName = umbracoHelper.TypedMember(booking.BookedById).Name;
                            booking.ConferenceRoomId = reader["ConferenceRoomNodeId"].ToString();
                            booking.ConferenceRoomName = umbracoHelper.TypedContent(booking.ConferenceRoomId).Name;
                            booking.StartTime = (DateTime)reader["startTime"];
                            booking.EndTime = (DateTime)reader["endTime"];
                            booking.Title = reader["title"] as string;
                            bookingsMadeByMember.Add(booking);
                        }
                    }
                }
            } catch (Exception ex) {
                log.ErrorException("Error in FindBookingsByMember",ex);
            }

            return bookingsMadeByMember;
        }
        
        public static List<BookingViewModel> FindBookingsByConferenceRoomAndMember(string conferenceRoomId, string memberId)
        {
            List<BookingViewModel> bookingsOnConferenceRoomMadeByMember = new List<BookingViewModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ApplicationSettings.Instance.DBConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [custom_booking] WHERE [ConferenceRoomNodeId] = @conferenceRoomNodeId AND [CreatedByMemberNodeId] = @createdByMemberNodeId", conn))
                    {
                        cmd.Parameters.Add("@conferenceRoomNodeId", SqlDbType.NVarChar).Value = conferenceRoomId;
                        cmd.Parameters.Add("@createdByMemberNodeId", SqlDbType.NVarChar).Value = memberId;
                        SqlDataReader reader = cmd.ExecuteReader();
                        UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                        while (reader.Read())
                        {
                            BookingViewModel booking = new BookingViewModel();
                            booking.Id = reader["Id"].ToString();
                            booking.BookedById = reader["CreatedByMemberNodeId"].ToString();
                            booking.BookedByName = umbracoHelper.TypedMember(booking.BookedById).Name;
                            booking.ConferenceRoomId = reader["ConferenceRoomNodeId"].ToString();
                            booking.ConferenceRoomName = umbracoHelper.TypedContent(booking.ConferenceRoomId).Name;
                            booking.StartTime = (DateTime)reader["startTime"];
                            booking.EndTime = (DateTime)reader["endTime"];
                            booking.Title = reader["title"] as string;
                            bookingsOnConferenceRoomMadeByMember.Add(booking);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Error in FindBookingsByConferenceRoomAndMember", ex);
            }

            return bookingsOnConferenceRoomMadeByMember;
        }

        public static List<BookingViewModel> FindBookingsByConferenceRoom(string conferenceRoomId)
        {
            List<BookingViewModel> bookingsOnConferenceRoom = new List<BookingViewModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ApplicationSettings.Instance.DBConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [custom_booking] WHERE [ConferenceRoomNodeId] = @conferenceRoomNodeId", conn))
                    {
                        cmd.Parameters.Add("@conferenceRoomNodeId", SqlDbType.NVarChar).Value = conferenceRoomId;
                        SqlDataReader reader = cmd.ExecuteReader();
                        UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                        while (reader.Read())
                        {
                            BookingViewModel booking = new BookingViewModel();
                            booking.Id = reader["Id"].ToString();
                            booking.BookedById = reader["CreatedByMemberNodeId"].ToString();
                            booking.BookedByName = umbracoHelper.TypedMember(booking.BookedById).Name;
                            booking.ConferenceRoomId = reader["ConferenceRoomNodeId"].ToString();
                            booking.ConferenceRoomName = umbracoHelper.TypedContent(booking.ConferenceRoomId).Name;
                            booking.StartTime = (DateTime)reader["startTime"];
                            booking.EndTime = (DateTime)reader["endTime"];
                            booking.Title = reader["title"] as string;
                            bookingsOnConferenceRoom.Add(booking);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Error in FindBookingsByConferenceRoom", ex);
            }

            return bookingsOnConferenceRoom;
        }

        public static BookingStatusViewModel IsConferenceRoomAvailable(string conferenceRoomNodeId, DateTime startTime, DateTime endTime, string meetingToUpdateId)
        {
            BookingStatusViewModel status = new BookingStatusViewModel();
            status.Status = "SUCCESS";
            try
            {
                using (SqlConnection conn = new SqlConnection(ApplicationSettings.Instance.DBConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT startTime, endTime FROM [custom_booking] WHERE ConferenceRoomNodeId = @conferenceRoomNodeId AND ID != @Id AND startTime < @endTime AND @startTime < endTime", conn))
                    {
                        cmd.Parameters.Add("@conferenceRoomNodeId", SqlDbType.NVarChar).Value = conferenceRoomNodeId;
                        cmd.Parameters.Add("@Id", SqlDbType.NVarChar).Value = meetingToUpdateId;
                        cmd.Parameters.Add("@endTime", SqlDbType.DateTime2).Value = endTime;
                        cmd.Parameters.Add("@startTime", SqlDbType.DateTime2).Value = startTime;
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read()) {
                            status.Message = "RESERVED";
                        } else {
                            status.Message = "OPEN";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Error in IsConferenceRoomAvailable", ex);
                status.Status = "ERROR";
                status.Message = "Something went wrong during conference rooms availability.";
            }
            return status;
        }
    }
}