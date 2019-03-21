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
            /* Return the partial view "ContactForm" */
            return PartialView("ContactForm", new ContactForm());
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            /* Tjek om ModelState er valid. Hvis ikke, returneres CurrentUmbracoPage(), for at vise siden med evt. fejlmeddelelser */
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();
               
             /* 
               Opret en ny email besked (MailMessage) og sæt modtager, emne (subject), afsender, og besked text (body)
               ud fra det brugeren har indtastet i kontakt formularen (model)
             */ 
             MailMessage mailMessage = new MailMessage();
             mailMessage.To.Add("chrisbrohus@gmail.com");
             mailMessage.Subject = model.Subject;
             mailMessage.From = new MailAddress(model.Email, model.Name);
             mailMessage.Body = model.Message;

             /*
               Opret en ny SmtpClient (mail klient), og sæt de relevante værdier.
               Her oprettes en SmtpClient der anvender en navngiven GMAIL konto
               med det angivne kontonavn og password.

               smtp.send er udeladt her for at undgå fejl/forsinkelser ved afsendelse (SmtpException) 
               da der ikke er angivet et rigtigt konto/password
             */
             using(SmtpClient smtp = new SmtpClient())
             {
                 smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                 smtp.UseDefaultCredentials = false;
                 smtp.EnableSsl =  true;
                 smtp.Host = "smtp.gmail.com";
                 smtp.Port = 587;
                 smtp.Credentials =  new System.Net.NetworkCredential("account@gmail.com", "password");
                 // send mail
                 //smtp.Send(mailMessage);
             }
             
             /* 
                Brug Umbraco ContentService til at oprette indhold udenom Umbraco backend
                Her oprettes en ny instans af content typen "message", med CurrentPage (i dette tilfælde kontaktsiden) som parent
             */
             IContent msg = Services.ContentService.CreateContent(model.Subject, CurrentPage.Id, "message");
             /*
                Sæt properties på content types baseret på det der er indtastes af brugeren i kontakt formularen 
             */
             msg.SetValue("messageName", model.Name);
             msg.SetValue("email", model.Email);
             msg.SetValue("subject", model.Subject);
             msg.SetValue("messageContent", model.Message);
             /* Gem beskeden */
             Services.ContentService.Save(msg);
             /* Marker at siden blev gemt ved at sætte TempData["success"] til true */
             TempData["success"] = true;
             /* Returner til nuværende side uden fejl */
             return RedirectToCurrentUmbracoPage();

        }
    }
}