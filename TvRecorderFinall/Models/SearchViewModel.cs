using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TvRecorderFinall.Models
{
    public class SearchViewModel
    {
        public int Sap { get; set; }
        public int IdNotification { get; set; }
        public string Login { get; set; }
        public IEnumerable<Notification> Notifications { get; set; }
    }
}