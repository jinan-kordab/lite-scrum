using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Database;
using WebApplication4.Models;
using WebApplication4.Helpers;
using System.Text;
using WebApplication4.Filters;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;

namespace WebApplication4.Controllers
{
    public class LoginController : Controller
    {

        public ActionResult Index()
        {

            if (Session["LANG"] != null)
            {
                if (Session["LANG"].ToString() == "fr")
                {
                    //Session["LoginFailureEn"] = "Your Nick Name or Password is incorrect !";
                    //Session["LoginFailureFr"] = "Votre pseudonyme ou votre mot de passe est incorrect!";
                    if (Session["LoginFailureFr"] != null)
                    {
                        AuthResponse aur = new AuthResponse();
                        aur.InvalidLogin = Session["LoginFailureFr"].ToString();
                        return View("fr/Index",aur);
                    }
                    else
                    {
                        return View("fr/Index");
                    }
                }
                else
                {
                    if (Session["LoginFailureEn"] != null)
                    {
                        AuthResponse aur = new AuthResponse();
                        aur.InvalidLogin = Session["LoginFailureEn"].ToString();
                        return View("Index", aur);
                    }
                    else
                    {
                        return View("Index");
                    }
                }
            }
            else
            {
                if (Session["LoginFailureEn"] != null)
                {
                    AuthResponse aur = new AuthResponse();
                    aur.InvalidLogin = Session["LoginFailureEn"].ToString();
                    return View("Index", aur);
                }
                else
                {
                    return View("Index");
                }
            }
        }

        public ActionResult IndexFr()
        {
            return View("fr/Index");
        }
        [HttpPost]
        public ActionResult RegisterNewUser(Person newPerson)
        {

            Encryption enc = new Encryption();
            
            var NICKNAME = newPerson.NICKNAME;
            var FIRSTNAME = newPerson.FIRSTNAME;
            var LASTNAME = newPerson.LASTNAME;
            var EMAIL = newPerson.EMAIL;
            var REGISTRATIONDATE = newPerson.REGISTRATIONDATE;
            var PASSWORD = enc.Encrypt_Aes(newPerson.PASSWORD);
            var CONFIRMPASSWORD = enc.Encrypt_Aes(newPerson.CONFIRMPASSWORD);
            var key = enc.Key;
            var IV = enc.IV;

            

            if (PASSWORD.Trim() == CONFIRMPASSWORD.Trim())
            {
                //Check if NickName already exists

                SCRUMEntities db = new SCRUMEntities();

                PERSON p = new PERSON();
                p.NICKNAME = NICKNAME;
                p.FIRSTNAME = FIRSTNAME;
                p.LASTNAME = LASTNAME;
                p.EMAIL = EMAIL;
                p.PASSWORD = PASSWORD.Trim();
                p.REGISTRATIONDATE = REGISTRATIONDATE;
                p.PKEY = key;
                p.PIV = IV;
                db.People.Add(p);

                var newPersonID = p.USERID;

                //Get and save Attachments
                if (newPerson.PROFILEPICTURE != null)
                {
                    var fileName = newPerson.PROFILEPICTURE.FileName;           //db field
                    var contentType = newPerson.PROFILEPICTURE.ContentType;     //db field
                    var contentLength = newPerson.PROFILEPICTURE.ContentLength; //db field

                    using (Stream st = newPerson.PROFILEPICTURE.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(st))
                        {
                            byte[] bytes = br.ReadBytes((Int32)st.Length);  //db field
                            PICTURE attchmnt = new PICTURE();
                            attchmnt.PERSONPICTURENAME = fileName;
                            attchmnt.PERSONPICTURETYPE = contentType;
                            attchmnt.PERSONPICTURELENGTH = contentLength;
                            attchmnt.PERSONPICTURECONTENT = bytes;
                            attchmnt.FKPERSONID = newPersonID;
                            db.PICTUREs.Add(attchmnt);
                            //db.SaveChanges();
                        }
                    }

                }

                db.SaveChanges();
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public ActionResult RegisterNewTeam(Person newPerson)
        {

            Encryption enc = new Encryption();
            string TEAMNAME = newPerson.TEAMNAME;
            

            var NICKNAME = newPerson.NICKNAME;
            var FIRSTNAME = newPerson.FIRSTNAME;
            var LASTNAME = newPerson.LASTNAME;
            var EMAIL = newPerson.EMAIL.Trim();
            var REGISTRATIONDATE = Convert.ToDateTime(newPerson.REGISTRATIONDATE);
            var PASSWORD = enc.Encrypt_Aes(newPerson.PASSWORD);
            var CONFIRMPASSWORD = enc.Encrypt_Aes(newPerson.CONFIRMPASSWORD);
            var key = enc.Key;
            var IV = enc.IV;


            if (PASSWORD.Trim() == CONFIRMPASSWORD.Trim())
            {
                //Check if NickName already exists

                SCRUMEntities db = new SCRUMEntities();

                PERSON p = new PERSON();
                p.NICKNAME = NICKNAME;
                p.FIRSTNAME = FIRSTNAME;
                p.LASTNAME = LASTNAME;
                p.EMAIL = EMAIL;
                p.PASSWORD = PASSWORD.Trim();
                p.REGISTRATIONDATE = Convert.ToDateTime(REGISTRATIONDATE);
                p.PKEY = key;
                p.PIV = IV;
                p.ISTEAMCREATOR = true;
                db.People.Add(p);

                var newPersonID = p.USERID;


                //Get and save Attachments
                if (newPerson.PROFILEPICTURE != null)
                {
                    var fileName = newPerson.PROFILEPICTURE.FileName;           //db field
                    var contentType = newPerson.PROFILEPICTURE.ContentType;     //db field
                    var contentLength = newPerson.PROFILEPICTURE.ContentLength; //db field

                    using (Stream st = newPerson.PROFILEPICTURE.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(st))
                        {
                            byte[] bytes = br.ReadBytes((Int32)st.Length);  //db field
                            PICTURE attchmnt = new PICTURE();
                            attchmnt.PERSONPICTURENAME = fileName;
                            attchmnt.PERSONPICTURETYPE = contentType;
                            attchmnt.PERSONPICTURELENGTH = contentLength;
                            attchmnt.PERSONPICTURECONTENT = bytes;
                            attchmnt.FKPERSONID = newPersonID;
                            db.PICTUREs.Add(attchmnt);
                            //db.SaveChanges();
                        }
                    }

                }



                TEAM team = new TEAM();
                    team.TEAMNAME = TEAMNAME;
                    team.FKPERSONID = newPersonID;

                db.TEAMs.Add(team);


                db.SaveChanges();
            }

            //Send Registration Confirmation
             //string smtpAddress = "smtp.gmail.com";
             //int portNumber = 587;
            // bool enableSSL = true;
             //string emailFromAddress = "lightscrum@gmail.com"; //Sender Email Address  
             //string password = "ioioioio09"; //Sender Password  
             //string emailToAddress = "kordabjinan@gmail.com"; //Receiver Email Address  
             //string subject = "Welcome to Light Scrum";


            string smtpAddress = ConfigurationManager.AppSettings.Get("SMTPServer");
            int portNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SMTPPort"));
            bool enableSSL = false;
            string emailFromAddress = ConfigurationManager.AppSettings.Get("FromEmail"); //Sender Email Address  
            string password = ConfigurationManager.AppSettings.Get("FromEmailPwd");    //Sender Password  
            string emailToAddress = EMAIL;                                            //Receiver Email Address  
            string subject = "";                                                            //string currentDomain = ConfigurationManager.AppSettings.Get("ResetPwdDomain");

            if (Session["LANG"] != null)
            {
                if (Session["LANG"].ToString() == "fr")
                {
                    subject = "Bienvenue à Light Scrum";
                }
                else
                {
                    subject = "Welcome to Light Scrum";
                }
            }
            else
            {
                subject = "Welcome to Light Scrum";
            }

            







            string body = "";
            if(Session["LANG"] != null)
            {
                if (Session["LANG"].ToString() == "fr")
                {

                    body += "<!DOCTYPE html>";
                    body += "<html>";
                    body += "<head>";
                    body += "<meta charset = \"utf-8\"/>";
                    body += "<title> Bienvenue à Light Scrum</title>";
                    body += "</head>";
                    body += "<body>";
                    body += "<div style=\"text-align: center;padding: 10px;background-color:#f1e7fe\">";
                    body += "<h2 style=\"color: #6600CC;background-color:ghostwhite;font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;text-align:left\">";
                    body += "<img alt = \"Light Scrum\"  style=\"width: 77px;height: 61px;\" longdesc=\"Light Scrum\" src=\"cid:logo8C.png\" /><img alt = \"\" style=\"height: 37px; margin-bottom:10px;width:95%\" src=\"cid:emailnavfr.png\" /></h2>";
                    body += "<h1 style=\"color: #8e44ad;\">Bienvenue à Light Scrum</h1>";
                    body += "<p  style=\"color: #8e44ad;\">";
                    body += "<img style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneTodo.png\"/><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneInProgress.png\" /><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneDone.png\" /></p>";
                    body += "<p  style=\"color: #8e44ad;\">";
                    body += "&nbsp;</p>";
                    body += "<table>";
                    body += "<tr>";
                    body += "<td style = \"width:33%\" ></td>";
                    body += "<td style=\"width:33% \">";

                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Merci de vous inscrire.</dt>";
                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Votre pseudonyme est : <u>" + NICKNAME + "</u></dt>";
                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Si vous avez oublié votre mot de passe, vous pouvez toujours le réinitialiser avec ce pseudonyme.</dt>";
                    body += "<div  style=\"text-align: justify;\">";
                    body += "<div style=\"text-align: center;\">";
                    body += "<dt  style=\"color: #660066;text-align: center;\"> <em><span style=\"font-size: x-large;\"> Vous pouvez commencer à utiliser Light Scrum</span></em><span style=\" font-size: x-large;\"> maintenant !</span></dt>";
                    body += "</div>";
                    body += "</div>";
                    body += "</td>";
                    body += "<td style = \"width:33% \" ></td>";
                    body += "</tr>";
                    body += "<tfoot>";
                    body += "<tr>";
                    body += "<td style=\"text-align:center\">Copyright Light Scrum 2020</td>";
                    body += "</tr>";
                    body += "</tfoot>";
                    body += "</table>";


                    body += "<p style=\"color: #8e44ad;\">";
                    body += "&nbsp;</p>";
                    body += "</div>";
                    body += "</body>";
                    body += "</html>";
                }
                else
                {
                    body += "<!DOCTYPE html>";
                    body += "<html>";
                    body += "<head>";
                    body += "<meta charset = \"utf-8\"/>";
                    body += "<title> Welcome to Light Scrum</title>";
                    body += "</head>";
                    body += "<body>";
                    body += "<div style=\"text-align: center;padding: 10px;background-color:#f1e7fe\">";
                    body += "<h2 style=\"color: #6600CC;background-color:ghostwhite;font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;text-align:left\">";
                    body += "<img alt = \"Light Scrum\"  style=\"width: 77px;height: 61px;\" longdesc=\"Light Scrum\" src=\"cid:logo8C.png\" /><img alt = \"\" style=\"height: 37px; margin-bottom:10px;width:90%\" src=\"cid:emailnav.png\" /></h2>";
                    body += "<h1 style=\"color: #8e44ad;\">Welcome to Light Scrum</h1>";
                    body += "<p  style=\"color: #8e44ad;\">";
                    body += "<img style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneTodo.png\"/><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneInProgress.png\" /><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneDone.png\" /></p>";
                    body += "<p  style=\"color: #8e44ad;\">";
                    body += "&nbsp;</p>";
                    body += "<table>";
                    body += "<tr>";
                    body += "<td style = \"width:33%\" ></td>";
                    body += "<td style=\"width:33% \">";

                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Thank you for registering.</dt>";
                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Your nickname is: <u>" + NICKNAME + "</u></dt>";
                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">If you forgot your password you can always reset it with this nickname.</dt>";
                    body += "<div  style=\"text-align: justify;\">";
                    body += "<div style=\"text-align: center;\">";
                    body += "<dt  style=\"color: #660066;text-align: center;\"> <em><span style=\"font-size: x-large;\"> You can start using Light Scrum</span></em><span style=\" font-size: x-large;\"> right now !</span></dt>";
                    body += "</div>";
                    body += "</div>";
                    body += "</td>";
                    body += "<td style = \"width:33% \" ></td>";
                    body += "</tr>";
                    body += "<tfoot>";
                    body += "<tr>";
                    body += "<td style=\"text-align:center\">Copyright Light Scrum 2020</td>";
                    body += "</tr>";
                    body += "</tfoot>";
                    body += "</table>";


                    body += "<p style=\"color: #8e44ad;\">";
                    body += "&nbsp;</p>";
                    body += "</div>";
                    body += "</body>";
                    body += "</html>";
                }

            }
            else
            {
                body += "<!DOCTYPE html>";
                body += "<html>";
                body += "<head>";
                body += "<meta charset = \"utf-8\"/>";
                body += "<title> Welcome to Light Scrum</title>";
                body += "</head>";
                body += "<body>";
                body += "<div style=\"text-align: center;padding: 10px;background-color:#f1e7fe\">";
                body += "<h2 style=\"color: #6600CC;background-color:ghostwhite;font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;text-align:left\">";
                body += "<img alt = \"Light Scrum\"  style=\"width: 77px;height: 61px;\" longdesc=\"Light Scrum\" src=\"cid:logo8C.png\" /><img alt = \"\" style=\"height: 37px; margin-bottom:10px;width:90%\" src=\"cid:emailnav.png\" /></h2>";
                body += "<h1 style=\"color: #8e44ad;\">Welcome to Light Scrum</h1>";
                body += "<p  style=\"color: #8e44ad;\">";
                body += "<img style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneTodo.png\"/><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneInProgress.png\" /><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneDone.png\" /></p>";
                body += "<p  style=\"color: #8e44ad;\">";
                body += "&nbsp;</p>";
                body += "<table>";
                body += "<tr>";
                body += "<td style = \"width:33%\" ></td>";
                body += "<td style=\"width:33% \">";

                body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Thank you for registering.</dt>";
                body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Your nickname is: <u>" + NICKNAME + "</u></dt>";
                body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">If you forgot your password you can always reset it with this nickname.</dt>";
                body += "<div  style=\"text-align: justify;\">";
                body += "<div style=\"text-align: center;\">";
                body += "<dt  style=\"color: #660066;text-align: center;\"> <em><span style=\"font-size: x-large;\"> You can start using Light Scrum</span></em><span style=\" font-size: x-large;\"> right now !</span></dt>";
                body += "</div>";
                body += "</div>";
                body += "</td>";
                body += "<td style = \"width:33% \" ></td>";
                body += "</tr>";
                body += "<tfoot>";
                body += "<tr>";
                body += "<td style=\"text-align:center\">Copyright Light Scrum 2020</td>";
                body += "</tr>";
                body += "</tfoot>";
                body += "</table>";


                body += "<p style=\"color: #8e44ad;\">";
                body += "&nbsp;</p>";
                body += "</div>";
                body += "</body>";
                body += "</html>";
            }
           



            System.Net.Mail.Attachment attLogo = new System.Net.Mail.Attachment(Server.MapPath("/Images/Email/logo8C.png"));
            System.Net.Mail.Attachment attNav = new System.Net.Mail.Attachment(Server.MapPath("/Images/Email/emailnav.png"));
            System.Net.Mail.Attachment attNavfr = new System.Net.Mail.Attachment(Server.MapPath("/Images/Email/emailnavfr.png"));
            System.Net.Mail.Attachment attTODO = new System.Net.Mail.Attachment(Server.MapPath("/Images/Landing/SectionOneTodo.png"));
            System.Net.Mail.Attachment attINPROG = new System.Net.Mail.Attachment(Server.MapPath("/Images/Landing/SectionOneInProgress.png"));
            System.Net.Mail.Attachment attDONE = new System.Net.Mail.Attachment(Server.MapPath("/Images/Landing/SectionOneDone.png"));

            attLogo.ContentId = "logo8C.png";
            attNav.ContentId = "emailnav.png";
            attNavfr.ContentId = "emailnavfr.png";
            attTODO.ContentId = "SectionOneTodo.png";
            attINPROG.ContentId = "SectionOneInProgress.png";
            attDONE.ContentId = "SectionOneDone.png";


            using (MailMessage mail = new MailMessage())
            {
                 
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(emailToAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Attachments.Add(attLogo);
                
                if (Session["LANG"] != null)
                {
                    if (Session["LANG"].ToString() == "fr")
                    {
                        mail.Attachments.Add(attNavfr);
                    }
                    else
                    {
                        mail.Attachments.Add(attNav);
                    }
                }
                else
                {
                    mail.Attachments.Add(attNav);
                }

                mail.Attachments.Add(attTODO);
                mail.Attachments.Add(attINPROG);
                mail.Attachments.Add(attDONE);

                using (SmtpClient smtp = new SmtpClient("localhost"))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }




            return RedirectToAction("Index", "Login");
        }

        public ActionResult ResetPasswordLink(Person newPerson)
        {
             Encryption enc = new Encryption();

            
            string PasswordResetLink = "";
            string useEmail = "";
            var NICKNAME = newPerson.NICKNAME.Trim();
           

            SCRUMEntities db = new SCRUMEntities();
            var auth = (from a in db.People
                        where a.NICKNAME == NICKNAME
                        select a).SingleOrDefault();

            if (auth != null)
            {
                useEmail = auth.EMAIL.Trim();


                string smtpAddress = ConfigurationManager.AppSettings.Get("SMTPServer");
                int portNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SMTPPort"));
                bool enableSSL = false;
                string emailFromAddress = ConfigurationManager.AppSettings.Get("FromEmail"); //Sender Email Address  
                string password = ConfigurationManager.AppSettings.Get("FromEmailPwd");    //Sender Password  
                string emailToAddress = useEmail;                                            //Receiver Email Address  
                string currentDomain = ConfigurationManager.AppSettings.Get("ResetPwdDomain");
                string subject = "Light Scrum Password Reset";

                var encNickName = enc.SimpleEncrypt(NICKNAME);
                var encEmail = enc.SimpleEncrypt(useEmail);
                var lang = "";

                if (Session["LANG"] != null)
                {
                    if (Session["LANG"].ToString() == "fr")
                    {
                        lang = "FR";
                    }
                    else
                    {
                        lang = "EN";
                    }
                }
                else
                {
                    lang = "EN";
                }
                lang = enc.SimpleEncrypt(lang);

                var h = encNickName + "|" + encEmail + "|" + lang;
                PasswordResetLink = currentDomain + "/Login/" + "ResetPassword?h=" + h;



                string body = "";

                if (Session["LANG"] != null)
                {
                    if (Session["LANG"].ToString() == "fr")
                    {

                        body += "<!DOCTYPE html>";
                        body += "<html>";
                        body += "<head>";
                        body += "<meta charset = \"utf-8\"/>";
                        body += "<title> Bienvenue à Light Scrum</title>";
                        body += "</head>";
                        body += "<body>";
                        body += "<div style=\"text-align: center;padding: 10px;background-color:#f1e7fe\">";
                        body += "<h2 style=\"color: #6600CC;background-color:ghostwhite;font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;text-align:left\">";
                        body += "<img alt = \"Light Scrum\"  style=\"width: 77px;height: 61px;\" longdesc=\"Light Scrum\" src=\"cid:logo8C.png\" /><img alt = \"\" style=\"height: 37px; margin-bottom:10px;width:95%\" src=\"cid:emailnavfr.png\" /></h2>";
                        body += "<h1 style=\"color: #8e44ad;\">Bienvenue à Light Scrum</h1>";
                        body += "<p  style=\"color: #8e44ad;\">";
                        body += "<img style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneTodo.png\"/><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneInProgress.png\" /><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneDone.png\" /></p>";
                        body += "<p  style=\"color: #8e44ad;\">";
                        body += "&nbsp;</p>";
                        body += "<table>";
                        body += "<tr>";
                        body += "<td style = \"width:33%\" ></td>";
                        body += "<td style=\"width:33% \">";

                        body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Bonjour " + NICKNAME + "</dt>";
                        body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Vous avez demandé à réinitialiser votre mot de passe. Si ce n'est pas vous, veuillez ne pas tenir compte de ce courriel.</u></dt>";
                        body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Pour réinitialiser votre mot de passe, veuillez cliquer sur le lien suivant: " + PasswordResetLink + "</dt>";
                        body += "<div  style=\"text-align: justify;\">";
                        body += "<div style=\"text-align: center;\">";
                        body += "<dt  style=\"color: #660066;text-align: center;\"> <em><span style=\"font-size: x-large;\"> Merci.</span></dt>";
                        body += "</div>";
                        body += "</div>";
                        body += "</td>";
                        body += "<td style = \"width:33% \" ></td>";
                        body += "</tr>";
                        body += "<tfoot>";
                        body += "<tr>";
                        body += "<td style=\"text-align:center\">Copyright Light Scrum 2020</td>";
                        body += "</tr>";
                        body += "</tfoot>";
                        body += "</table>";


                        body += "<p style=\"color: #8e44ad;\">";
                        body += "&nbsp;</p>";
                        body += "</div>";
                        body += "</body>";
                        body += "</html>";
                    }
                    else
                    {
                        body += "<!DOCTYPE html>";
                        body += "<html>";
                        body += "<head>";
                        body += "<meta charset = \"utf-8\"/>";
                        body += "<title> Welcome to Light Scrum</title>";
                        body += "</head>";
                        body += "<body>";
                        body += "<div style=\"text-align: center;padding: 10px;background-color:#f1e7fe\">";
                        body += "<h2 style=\"color: #6600CC;background-color:ghostwhite;font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;text-align:left\">";
                        body += "<img alt = \"Light Scrum\"  style=\"width: 77px;height: 61px;\" longdesc=\"Light Scrum\" src=\"cid:logo8C.png\" /><img alt = \"\" style=\"height: 37px; margin-bottom:10px;width:90%\" src=\"cid:emailnav.png\" /></h2>";
                        body += "<h1 style=\"color: #8e44ad;\">Welcome to Light Scrum</h1>";
                        body += "<p  style=\"color: #8e44ad;\">";
                        body += "<img style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneTodo.png\"/><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneInProgress.png\" /><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneDone.png\" /></p>";
                        body += "<p  style=\"color: #8e44ad;\">";
                        body += "&nbsp;</p>";
                        body += "<table>";
                        body += "<tr>";
                        body += "<td style = \"width:33%\" ></td>";
                        body += "<td style=\"width:33% \">";

                        body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Hello "+NICKNAME+ "</dt>";
                        body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">You requested to reset your password. If it was not you, please disregard this email.</u></dt>";
                        body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">To reset your password, please click on the following link: "+PasswordResetLink+"</dt>";
                        body += "<div  style=\"text-align: justify;\">";
                        body += "<div style=\"text-align: center;\">";
                        body += "<dt  style=\"color: #660066;text-align: center;\"> <em><span style=\"font-size: x-large;\">Thank you.</span></dt>";
                        body += "</div>";
                        body += "</div>";
                        body += "</td>";
                        body += "<td style = \"width:33% \" ></td>";
                        body += "</tr>";
                        body += "<tfoot>";
                        body += "<tr>";
                        body += "<td style=\"text-align:center\">Copyright Light Scrum 2020</td>";
                        body += "</tr>";
                        body += "</tfoot>";
                        body += "</table>";


                        body += "<p style=\"color: #8e44ad;\">";
                        body += "&nbsp;</p>";
                        body += "</div>";
                        body += "</body>";
                        body += "</html>";
                    }

                }
                else
                {
                    body += "<!DOCTYPE html>";
                    body += "<html>";
                    body += "<head>";
                    body += "<meta charset = \"utf-8\"/>";
                    body += "<title> Welcome to Light Scrum</title>";
                    body += "</head>";
                    body += "<body>";
                    body += "<div style=\"text-align: center;padding: 10px;background-color:#f1e7fe\">";
                    body += "<h2 style=\"color: #6600CC;background-color:ghostwhite;font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;text-align:left\">";
                    body += "<img alt = \"Light Scrum\"  style=\"width: 77px;height: 61px;\" longdesc=\"Light Scrum\" src=\"cid:logo8C.png\" /><img alt = \"\" style=\"height: 37px; margin-bottom:10px;width:90%\" src=\"cid:emailnav.png\" /></h2>";
                    body += "<h1 style=\"color: #8e44ad;\">Welcome to Light Scrum</h1>";
                    body += "<p  style=\"color: #8e44ad;\">";
                    body += "<img style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneTodo.png\"/><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneInProgress.png\" /><img  style=\"width: 147px;height: 259px;\" src=\"cid:SectionOneDone.png\" /></p>";
                    body += "<p  style=\"color: #8e44ad;\">";
                    body += "&nbsp;</p>";
                    body += "<table>";
                    body += "<tr>";
                    body += "<td style = \"width:33%\" ></td>";
                    body += "<td style=\"width:33% \">";

                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">Hello " + NICKNAME + "</dt>";
                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">You requested to reset your password. If it was not you, please disregard this email.</u></dt>";
                    body += "<dt style=\"font-size: x-large;text-align: center;color: #660066;\">To reset your password, please click on the following link: " + PasswordResetLink + "</dt>";
                    body += "<div  style=\"text-align: justify;\">";
                    body += "<div style=\"text-align: center;\">";
                    body += "<dt  style=\"color: #660066;text-align: center;\"> <em><span style=\"font-size: x-large;\">Thank you.</span></dt>";
                    body += "</div>";
                    body += "</div>";
                    body += "</td>";
                    body += "<td style = \"width:33% \" ></td>";
                    body += "</tr>";
                    body += "<tfoot>";
                    body += "<tr>";
                    body += "<td style=\"text-align:center\">Copyright Light Scrum 2020</td>";
                    body += "</tr>";
                    body += "</tfoot>";
                    body += "</table>";


                    body += "<p style=\"color: #8e44ad;\">";
                    body += "&nbsp;</p>";
                    body += "</div>";
                    body += "</body>";
                    body += "</html>";
                }


                System.Net.Mail.Attachment attLogo = new System.Net.Mail.Attachment(Server.MapPath("/Images/Email/logo8C.png"));
                System.Net.Mail.Attachment attNav = new System.Net.Mail.Attachment(Server.MapPath("/Images/Email/emailnav.png"));
                System.Net.Mail.Attachment attNavfr = new System.Net.Mail.Attachment(Server.MapPath("/Images/Email/emailnavfr.png"));
                System.Net.Mail.Attachment attTODO = new System.Net.Mail.Attachment(Server.MapPath("/Images/Landing/SectionOneTodo.png"));
                System.Net.Mail.Attachment attINPROG = new System.Net.Mail.Attachment(Server.MapPath("/Images/Landing/SectionOneInProgress.png"));
                System.Net.Mail.Attachment attDONE = new System.Net.Mail.Attachment(Server.MapPath("/Images/Landing/SectionOneDone.png"));

                attLogo.ContentId = "logo8C.png";
                attNav.ContentId = "emailnav.png";
                attNavfr.ContentId = "emailnavfr.png";
                attTODO.ContentId = "SectionOneTodo.png";
                attINPROG.ContentId = "SectionOneInProgress.png";
                attDONE.ContentId = "SectionOneDone.png";


                using (MailMessage mail = new MailMessage())
                {

                    mail.From = new MailAddress(emailFromAddress);
                    mail.To.Add(emailToAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    mail.Attachments.Add(attLogo);

                    if (Session["LANG"] != null)
                    {
                        if (Session["LANG"].ToString() == "fr")
                        {
                            mail.Attachments.Add(attNavfr);
                        }
                        else
                        {
                            mail.Attachments.Add(attNav);
                        }
                    }
                    else
                    {
                        mail.Attachments.Add(attNav);
                    }



                    mail.Attachments.Add(attTODO);
                    mail.Attachments.Add(attINPROG);
                    mail.Attachments.Add(attDONE);

                    using (SmtpClient smtp = new SmtpClient("localhost"))
                    {
                        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }



            }
            else
            {
                Session["LoginFailureEn"] = "The Nick Name for Password Change you have entered does not exist !";
                Session["LoginFailureFr"] = "Le pseudonyme pour le changement de mot de passe que vous avez saisi n'existe pas !";
            }

           




            return RedirectToAction("Index", "Login");
        }



        public ActionResult ResetPassword(string h)
        {
            Encryption enc = new Encryption();

            var splitArray = h.Split('|');

            var NickName = enc.SimpleDecrypt(splitArray[0].ToString());
            var Email = enc.SimpleDecrypt(splitArray[1]);
            var Lang = enc.SimpleDecrypt(splitArray[2]);


            AuthResponse r = new AuthResponse();
            r.NickName = NickName;
            r.Email = Email;

            if (Lang != null)
            {
                if (Lang == "FR")
                {
                    Session["LANG"] = "fr";
                    return View("fr/ResetPassword", r);
                }
                else
                {
                    return View("ResetPassword", r);
                }
            }
            else
            {
                return View("ResetPassword", r);
            }

        }

        public ActionResult ChangePassword(Login personCredentials)
        {
            var userNewPassword = personCredentials.NewPassword.Trim();
            var confirmNewPassword = personCredentials.ConfirmNewPassword.Trim();

            var userEmail = personCredentials.userEmail.Trim();
            var userName = personCredentials.userName.Trim();

            AuthResponse r = new AuthResponse();

            if (userNewPassword == confirmNewPassword)
            {
                Encryption enc = new Encryption();
                SCRUMEntities db = new SCRUMEntities();

                var user = (from a in db.People
                            where a.NICKNAME == userName && a.EMAIL == userEmail
                            select a).SingleOrDefault();

                var PASSWORD = enc.Encrypt_Aes(userNewPassword);
                var key = enc.Key;
                var IV = enc.IV;

                user.PASSWORD = PASSWORD;
                user.PKEY = key;
                user.PIV = IV;
                db.SaveChanges();

                Session["LoginFailureEn"] = "Password Reset Successfully !";
                Session["LoginFailureFr"] = "Le mot de passe a été modifié avec succès !";
               
            }
            else
            {
                Session["LoginFailureEn"] = "Your passwords do not match (New password, Confirm new password), please try again.";
                Session["LoginFailureFr"] = "Vos mots de passe ne correspondent pas (Nouveau mot de passe, Confirmer le nouveau mot de passe), veuillez réessayer.";
            }

            

            return RedirectToAction("Index", "Login");
        }




        [HttpPost]
        public RedirectToRouteResult LoginPerson(Login personCredentials)
        {

            Encryption enc = new Encryption();
            Session["userLoginKey"] = enc.Key;
            Session["userLoginIV"] = enc.IV;

            var userName = personCredentials.userName;
            var userLoginPassword = personCredentials.userPassword.Trim();

            SCRUMEntities db = new SCRUMEntities();
            var auth = (from a in db.People
                        where a.NICKNAME == userName
                        select a).SingleOrDefault();
           
            if (auth != null)
            {
                var decryptedPassword = enc.Decrypt_Aes_With_Custom_Key(auth.PASSWORD.Trim(),auth.PKEY,auth.PIV);

                if (decryptedPassword == userLoginPassword)
                {
                    Session["UserLoggedIn"] = "li";
                    Session["UserId"] = Server.UrlEncode(enc.Encrypt_Aes_With_Custom_Key(auth.USERID.ToString(),(byte[])Session["userLoginKey"],(byte[])Session["userLoginIV"]));
                    Session["USERK"] = enc.Key;
                    Session["USERF"] = enc.IV;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Session["LoginFailureEn"] = "Your Password is incorrect !";
                    Session["LoginFailureFr"] = "Votre mot de passe est incorrect!";
                }
            }
            else
            {
                Session["LoginFailureEn"] = "Your Nick Name is incorrect !";
                Session["LoginFailureFr"] = "Votre pseudonyme est incorrect!";
            }
            
                return RedirectToAction("Index", "Login");

        }

        [Authentication]
        [HttpPost]
        public RedirectToRouteResult Logout()
        {
            string action = "";
            
            if (Session["LANG"] != null)
            {
                if (Session["LANG"].ToString() == "fr")
                {
                    action = "IndexFr";
                    
                    //int sessionCount = Session.Count - 1;
                    for(int i =0; i < Session.Count; i ++)
                    {
                        if (Session.Contents.Keys[i] != "LANG")
                        {
                            Session.Remove(Session.Keys[i]);
                        }
                    }
                    //Session.Abandon();
                    return RedirectToAction("IndexFr", "Login");
                }
                else
                {
                    action = "Index";

                    for (int i = 0; i < Session.Count; i++)
                    {
                        if (Session.Contents.Keys[i] != "LANG")
                        {
                            Session.Remove(Session.Keys[i]);
                        }
                    }
                    //Session.Abandon();
                    return RedirectToAction("Index", "Login");
                }
            }

            return RedirectToAction(action, "Login");
        }

    

        [HttpPost]
        public JsonResult CheckNickName(string NickName)
        {
            SCRUMEntities db = new SCRUMEntities();

            var nickname = (from n in db.People where n.NICKNAME == NickName select n).SingleOrDefault();

            if (nickname != null) //if the user exists
            {
                return Json("false");
            }

            return Json("true");
        }

        [HttpPost]
        public JsonResult CheckGoogleSignIn(string id, string name,string imageurl, string email)
        {
            SCRUMEntities db = new SCRUMEntities();

            //var nickname = (from n in db.People where n.NICKNAME == NickName select n).SingleOrDefault();

            //if (nickname != null) //if the user exists
            //{
            //    return Json("false");
            //}

            return Json("true");
        }
        //CheckGoogleSignIn
        //CheckTeamName
        [HttpPost]
        public JsonResult CheckTeamName(string TeamName)
        {
            SCRUMEntities db = new SCRUMEntities();

            var teamName = (from n in db.TEAMs where n.TEAMNAME == TeamName select n).SingleOrDefault();

            if (teamName != null) //if the team exists
            {
                return Json("false");
            }

            return Json("true");
        }


        [HttpPost]
        public RedirectToRouteResult Culture(Language lang)
        {
            var languageSelected = lang.lAction;
            Session["LANG"] = languageSelected;
            return RedirectToAction("Index");
        }



    }
}