using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace S_Umb_Turbine.ViewModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BookingStatusViewModel
    {
        [JsonProperty(PropertyName ="status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "booking")]
        public BookingViewModel Booking { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}