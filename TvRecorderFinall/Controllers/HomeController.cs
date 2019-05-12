using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using TvRecorderFinall.Models;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data.Entity;


namespace TvRecorderFinall.Controllers
{
    
    public class HomeController : Controller
    {
        #region MyRegion
        /* private List<Notification> notificationsList;


         public HomeController(Notification model)
         {

              string connectionString= @"data source=zyskplustst;initial catalog=test_tvrecorder;integrated security=True;MultipleActiveResultSets=True";
             test_tvrecorderEntities db = new test_tvrecorderEntities();
            Record records = new Record();

             var query = "Select * from Record where sap = @SAP";
             var result = new List<Notification>();
             using (var connection = new SqlConnection(connectionString))
             {
                 connection.Open();
                 using (var cmd = new SqlCommand(query, connection))
                 {
                     cmd.Parameters.AddWithValue("SAP", model.Sap);
                     var reader = cmd.ExecuteReader();
                     while (reader.Read())
                     {
                         var record = new Notification();
                         record.Sap = (int)reader["SAP"];
                         record.IdNotifiaction = (int)reader["IdNotifiaction"];
                         record.CreatedAt = (DateTime)reader["Date"];
                         record.Login = reader["Login"].ToString();
                         record.NameFile = reader["FileName"].ToString();
                     }
                 }
             }

         }
         */
        #endregion



        [HttpGet]
        public ActionResult Index()
        {
                  
            return View();
        }
        [HttpGet]
        public ViewResult About()
        {
            var history = GetNotification();
            return View(history);
        }
        [HttpGet]
        public ActionResult Contact()
        {
            var model = new SearchViewModel
            {
                Notifications = new List<Notification>()
            };
            return View("Contact", model);
        }
        [HttpPost]
        public ActionResult Contact(SearchViewModel model)
        {
            model.Notifications = GetSapHistory(model.Sap);
            if (model.Sap != 0)
            {
                model.Notifications = GetSapHistory(model.Sap);
            }
            else if (model.IdNotification != 0)
            {
                model.Notifications = GetIdNotificationHistory(model.IdNotification);
            }
            else if (model.Login == "null")
            {
                model.Notifications = GetLoginHistory(model.Login);
            }
            return View("Contact", model);
            
        }
        
        //[HttpGet]
        //public ActionResult Contact()
        //{

        //    return View("~/Views/Home/Contact.cshtml");
        //    //return PartialView(history);
        //}

        //private IEnumerable<Notification> CheckCondition(Notification model)
        //{
        //    var list = GetSapHistory(model.Sap);
        //    if (model.Sap != 0)
        //    {
        //        list = GetSapHistory(model.Sap);
                
        //    }
        //    else if (model.IdNotifiaction != 0)
        //    {
        //        list = GetIdNotificationHistory(model.IdNotifiaction);
                
        //    }
        //    else if (model.Login == "null")
        //    {
        //       list = GetLoginHistory(model.Login);
                

        //    }
        //    return list;
           
        //}

        private IEnumerable<Notification> GetLoginHistory(string login)
        {
            var recorder = new test_tvrecorderEntities();
            var notification = recorder.Record.Where(x => x.Login == login);
            List<Notification> list = new List<Notification>();
            foreach (var item in notification)
            {
                list.Add(Notification.Parse(item));
            }
            return list;
        }
        private IEnumerable<Notification> GetIdNotificationHistory(int idNotification)
        {
            var recorder = new test_tvrecorderEntities();
            var notification = recorder.Record.Where(x => x.IdNotification == idNotification);
            List<Notification> list = new List<Notification>();
            foreach (var item in notification)
            {
                list.Add(Notification.Parse(item));
            }
            return list;
        }
        private IEnumerable<Notification> GetSapHistory(int sap)
        {
            var recorder = new test_tvrecorderEntities();
            var notification = recorder.Record.Where(x => x.SAP == sap).ToList();
            List<Notification> list = new List<Notification>();
            foreach (var item in notification)
            {
                list.Add(Notification.Parse(item));
            }
            return list;
        }

        //cała historia zgłoszeń
        private IEnumerable<Notification> GetNotification()
        {
            var recorder = new test_tvrecorderEntities();
            var notification = recorder.Record.ToList();
            List<Notification> list = new List<Notification>();
            foreach (var item in notification)
            {
                list.Add(Notification.Parse(item));
            }
            return list;
        }

        
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, Notification model)
        {

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    var fileName = Path.GetFileName(file.FileName);

                    using (SHA256 mySHA256 = SHA256.Create())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(fileName);
                        byte[] hashValue = mySHA256.ComputeHash(bytes);

                        foreach (byte x in hashValue)
                        {
                            model.NameFile += String.Format("{0:x2}", x);
                        }
                    }
                    //TODO: zapis do folderu
                    //trzeba wpisać poprawny adres gdzie mają być kopiowane pliki
                    var path = Path.Combine(Server.MapPath(@"\\upload"), model.NameFile);
                    //var path = @"C:\Users\rakowalewski\Desktop\test_nagranie" + model.NameFile;
                    file.SaveAs(path);

                    //Zapis do bazy
                    test_tvrecorderEntities db = new test_tvrecorderEntities();
                    Record records = new Record();
                    WindowsIdentity wi = WindowsIdentity.GetCurrent();
                   // model.Login = wi.Name;
                   
                    model.CreatedAt = DateTime.UtcNow;

                    records.IdNotification = model.IdNotification;
                    records.SAP = model.Sap;
                    records.Date = model.CreatedAt;
                    records.Login = model.Login;
                    records.FileName = model.NameFile;
                    db.Record.Add(records);
                    db.SaveChanges();
                    db.Dispose();

                    ViewBag.Message = "File uploaded successfully";
                    return RedirectToAction("Index");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }

       
    }
}