using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace S_Umb_Turbine.ViewModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BookingViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "bookedByName")]
        public string BookedByName { get; set; }
        [JsonProperty(PropertyName = "bookedById")]
        public string BookedById { get; set; }
        [JsonProperty(PropertyName = "conferenceRoomName")]
        public string ConferenceRoomName { get; set; }
        [JsonProperty(PropertyName = "conferenceRoomId")]
        public string ConferenceRoomId { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "start")]
        public DateTime StartTime { get; set; }
        [JsonProperty(PropertyName = "end")]
        public DateTime EndTime { get; set; }
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }
    }
}