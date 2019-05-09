using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TvRecorderFinall.Models
{
    public class Notification
    {
        [Display(Name = "Sap")]
        [Range(100000,999999)]
        public int Sap { get; set; }

        [Display(Name = "IdNotifiaction")]
        [Range(10000, 999999)]
        public int IdNotification { get; set; }

        [Display(Name = "CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Login")]
        public string Login { get; set; }
        [Display(Name = "NameFile")]
        public string NameFile { get; set; }

        public List<HttpPostedFile> File { get; set; }

        

        public Notification()
        {
            File = new List<HttpPostedFile>();
        }

        public static Notification Parse(TvRecorderFinall.Record record)
        {
            Notification notification = new Notification
            {
                Sap = record.SAP,
                IdNotification = record.IdNotification,
                CreatedAt = record.Date,
                Login = record.Login,
                NameFile = record.FileName
            };
            return notification;
        }
        
    }
}
