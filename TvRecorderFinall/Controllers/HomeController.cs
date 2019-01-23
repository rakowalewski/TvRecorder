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

namespace TvRecorderFinall.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
          //  ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
          //  ViewBag.Message = "Your contact page.";

            return View();
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

                    //trzeba wpisać poprawny adres gdzie mają być kopiowane pliki
                    var path = Path.Combine(Server.MapPath("\\upload"), model.NameFile);
                    file.SaveAs(path);

                    //Zapis do bazy
                    test_tvrecorderEntities db = new test_tvrecorderEntities();
                    Record records = new Record();
                    WindowsIdentity wi = WindowsIdentity.GetCurrent();
                    model.Login = wi.Name;
                    model.CreatedAt = DateTime.UtcNow;
                    records.IdNotification = model.IdNotifiaction;
                    records.SAP = model.Sap;
                    records.Date = model.CreatedAt;
                    records.Login = model.Login;
                    records.FileName = model.NameFile;
                    db.Record.Add(records);
                    db.SaveChanges();

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