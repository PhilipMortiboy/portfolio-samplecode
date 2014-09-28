using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AppServices.Results;

namespace AppServices
{
    public class EmailServices
    {
        public static void SendTutAllocEmail(Student myStudent, string tutor)
        {
            // Test emails could potenial be the same as real emails, so don't use this when testing
            var toEmail = ""; //myStudent.Email;

            //code from: http://labs.cms.gre.ac.uk/web/aspemailnet.asp
            var mail = new System.Net.Mail.MailMessage();
            mail.From = new MailAddress("");
            mail.To.Add(toEmail);
            mail.Body = "Dear " + myStudent.sSurname + " " + myStudent.sForename + ", you have been assigned " + tutor
                + " as your personal tutor";
            mail.Subject = "Tutor allocation";
            var client = new SmtpClient();
            client.Host = "smtp.gre.ac.uk";
            client.Send(mail);
        }
    }
}
