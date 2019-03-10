using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using AarhusWebDevCoop.ViewModels;
using System.Net.Mail;
using Umbraco.Core.Models;

namespace AarhusWebDevCoop.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        // GET: ContactFormSurface
        public ActionResult Index()
        {
            return PartialView("ContactForm", new ContactForm());
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();
            /*   
               MailMessage mailMessage = new MailMessage();
               mailMessage.To.Add("chrisbrohus@gmail.com");
               mailMessage.Subject = model.Subject;
               mailMessage.From = new MailAddress(model.Email, model.Name);
               mailMessage.Body = model.Message;

               using(SmtpClient smtp = new SmtpClient())
               {
                   smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                   smtp.UseDefaultCredentials = false;
                   smtp.EnableSsl =  true;
                   smtp.Host = "smtp.gmail.com";
                   smtp.Port = 587;
                   smtp.Credentials =  new System.Net.NetworkCredential("username@gmail.com", "password");
                   // send mail
                   smtp.Send(mailMessage);
               }
              */
            IContent msg = Services.ContentService.CreateContent(model.Subject, CurrentPage.Id, "message");
            msg.SetValue("messageName", model.Name);
            msg.SetValue("email", model.Email);
            msg.SetValue("subject", model.Subject);
            msg.SetValue("messageContent", model.Message);
            Services.ContentService.Save(msg);
            TempData["success"] = true;
            return RedirectToCurrentUmbracoPage();

        }
    }
}