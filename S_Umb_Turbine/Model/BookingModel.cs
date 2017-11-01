using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace S_Umb_Turbine.Model
{
    public class BookingModel
    {
        public string Id;
        public string CreatedByMemberNodeId;
        public string ConferenceRoomNodeId;
        public string Title;
        public DateTime StartTime;
        public DateTime EndTime;
    }
}