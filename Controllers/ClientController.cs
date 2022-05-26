using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Database;
using WebApplication4.Filters;
using WebApplication4.Helpers;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class ClientController : Controller
    {
        public Encryption encNewTeamMember;

  [Authentication]
        public ActionResult Index()
        {
            //Client item = new Client();
            ////item.Itemlist = items.ToList();
            //item.LoggedInUser = Session["UserId"].ToString();

           encNewTeamMember = new Encryption();
            if ((Session["UserLoggedIn"] != null && Session["UserLoggedIn"].ToString().Trim() == "li"))
            {

                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                var items = from itm in db.CLIENTs.AsEnumerable()
                            where itm.FKPERSONID == userid
                            select new Client
                            {
                                CLIENTID = itm.CLIENTID,
                                CLIENTNAME = itm.CLIENTNAME,
                                CLIENTLOCATION = itm.CLIENTLOCATION,
                                CLIENTDESCRIPTION = itm.CLIENTDESCRIPTION,
                                CLIENTPHONENUMBER = itm.CLIENTPHONENUMBER,
                                ClientImagePath = GetAndSaveClientPicture(GetAttachmentId(itm.CLIENTID))
                            };

                Client item = new Client();
                var initialClients = items.OrderBy(x =>x.CLIENTNAME).ToList();

                item.Clientlist = items.ToList();
                //Also, we need to get all clients for which Team Creator has assigned usesr an item for.
                //Even if the user himself or herself did not create that client 
                //Very important !
                //Otherwise if Team Creator assigned you to a task, you would not know for which client it is, and you
                //will not be able to see that scrum board as well.

                //1-Get all items that the user has in All Sprints

                var allUserAssignedItems = (from userAssignedItem in db.ITEMs
                                            where userAssignedItem.FKPERSONID == userid
                                            select userAssignedItem).ToList();

                foreach (var userItem in allUserAssignedItems)
                {

                    var ProductBacklg = (from pb in db.PRODUCTBACKLOGs where pb.ITEMID.Equals(userItem.FKPRODUCTBACKLOGID) select pb).SingleOrDefault();

                    var clientEpic = (from epc in db.EPICs where epc.EPICID == ProductBacklg.FKEPICID select epc).SingleOrDefault();

                    var client = (from cl in db.CLIENTs.AsEnumerable()
                                  where cl.CLIENTID == clientEpic.FKCLIENTID
                                  select new Client
                                  {
                                      CLIENTID = cl.CLIENTID,
                                      CLIENTNAME = cl.CLIENTNAME,
                                      CLIENTLOCATION = cl.CLIENTLOCATION,
                                      CLIENTDESCRIPTION = cl.CLIENTDESCRIPTION,
                                      CLIENTPHONENUMBER = cl.CLIENTPHONENUMBER,
                                      ClientImagePath = GetAndSaveClientPicture(GetAttachmentId(cl.CLIENTID))
                                  }).FirstOrDefault();

                    var containsFlag = false;

                    for(int i =0; i < initialClients.Count; i ++)
                    {
                        if (initialClients[i].CLIENTID == client.CLIENTID)
                        {
                            containsFlag = true;
                        }
                    }

                    if (containsFlag == false)
                    {
                        item.Clientlist.Add(client);
                    }
                   
                }

                item.Clientlist = item.Clientlist.GroupBy(x => x.CLIENTID).Select(y => y.First()).ToList();

                


                item.LoggedInUser = Session["UserId"].ToString();
                item.PersonImagePath = GetAndSavePersonsPicture(Convert.ToInt64(userid));

                if (Session["LANG"] != null)
                {
                    if (Session["LANG"].ToString() == "fr")
                    {
                        return View("fr/Index", item);
                    }
                    else
                    {
                        return View("Index", item);
                    }
                }
                else
                {
                    return View("Index", item);
                }

            }
            return View("Index", "Client");
        }



        [Authentication]
        [HttpPost]
        public ActionResult AddNewClient(Client newItem)
        {
            var itemTitle = newItem.CLIENTNAME;
            var clientLocation = newItem.CLIENTLOCATION;
            var clientDescription = newItem.CLIENTDESCRIPTION.Replace("\r\n", "<br>");
            var clientPhoneNumber = newItem.CLIENTPHONENUMBER;
            var attachmentsCount = newItem.Attachments.Count;

            

            Encryption encpt = new Encryption();
            //Save new Item in ITEM table and get PK of new row
            SCRUMEntities db = new SCRUMEntities();
            CLIENT item = new CLIENT();
            item.CLIENTNAME = itemTitle;
            item.CLIENTLOCATION = clientLocation;
            item.ATTACHMENTSCOUNT = attachmentsCount;
            item.CLIENTPHONENUMBER = clientPhoneNumber;
            item.CLIENTDESCRIPTION = clientDescription;

            //Session["UserId"]
            //Session["USERK"]
            //Session["USERF"]
            //hyujkzsaa
            Byte[] USERK = (Byte[])Session["USERK"];
            Byte[] USERF = (Byte[])Session["USERF"];

            item.FKPERSONID = Convert.ToInt64(encpt.Decrypt_Aes_With_Custom_Key(Server.UrlDecode(newItem.hyujkzsaa), USERK, USERF));

            db.CLIENTs.Add(item);

            //New Item ID
            var newItemID = item.CLIENTID;


            //Get and save Attachments
            if (newItem.Attachments[0] != null)
            {
                foreach (var attachment in newItem.Attachments)
                {
                    var fileName = attachment.FileName;           //db field
                    var contentType = attachment.ContentType;     //db field
                    var contentLength = attachment.ContentLength; //db field

                    using (Stream st = attachment.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(st))
                        {
                            byte[] bytes = br.ReadBytes((Int32)st.Length);  //db field
                            CLIENTATTACHMENT attchmnt = new CLIENTATTACHMENT();
                            attchmnt.CATTACHMENTNAME = fileName;
                            attchmnt.CATTACHMENTTYPE = contentType;
                            attchmnt.CATTACHMENTLENGTH = contentLength;
                            attchmnt.CATTACHMENTCONTENT = bytes;
                            attchmnt.FKCLIENTID = newItemID;
                            db.CLIENTATTACHMENTs.Add(attchmnt);
                        }
                    }
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //

        [Authentication]
        [HttpPost]
        public ActionResult EditExistingClient(Client newItem)
        {


            var itemTitle = newItem.CLIENTNAME;
            var clientLocation = newItem.CLIENTLOCATION;
            var clientDescription = newItem.CLIENTDESCRIPTION.Replace("\r\n", "<br>");
            var clientPhoneNumber = newItem.CLIENTPHONENUMBER;
            var attachmentsCount = newItem.Attachments.Count;

            var clientIdToEdit = Convert.ToInt64(newItem.hyujkzsaa);

            Encryption encpt = new Encryption();
            //Save new Item in ITEM table and get PK of new row
            SCRUMEntities db = new SCRUMEntities();
            var item = (from c in db.CLIENTs where c.CLIENTID == clientIdToEdit select c).SingleOrDefault();

            item.CLIENTNAME = itemTitle;
            item.CLIENTLOCATION = clientLocation;
            item.ATTACHMENTSCOUNT = attachmentsCount;
            item.CLIENTPHONENUMBER = clientPhoneNumber;
            item.CLIENTDESCRIPTION = clientDescription;


            //hyujkzsaa
            Byte[] USERK = (Byte[])Session["USERK"];
            Byte[] USERF = (Byte[])Session["USERF"];

           // item.FKPERSONID = Convert.ToInt64(encpt.Decrypt_Aes_With_Custom_Key(Server.UrlDecode(newItem.hyujkzsaa), USERK, USERF));

           // db.CLIENTs.Add(item);

            //New Item ID
            //var newItemID = item.CLIENTID;


            //Get and save Attachments
            if (newItem.Attachments[0] != null)
            {
                foreach (var attachment in newItem.Attachments)
                {
                    //C:\\Users\\korda\\Documents\\JINAN\\PICTURES\\IMG_2375.GIF

                    var fileName = attachment.FileName;           //db field

                    //IE workaround 
                    if(fileName.Contains("\\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }

                    var contentType = attachment.ContentType;     //db field
                    var contentLength = attachment.ContentLength; //db field

                    using (Stream st = attachment.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(st))
                        {
                            byte[] bytes = br.ReadBytes((Int32)st.Length);  //db field
                            var ca = (from ct in db.CLIENTATTACHMENTs where ct.FKCLIENTID == clientIdToEdit select ct).SingleOrDefault();

                            //CLIENTATTACHMENT attchmnt = new CLIENTATTACHMENT();
                            ca.CATTACHMENTNAME = fileName;
                            ca.CATTACHMENTTYPE = contentType;
                            ca.CATTACHMENTLENGTH = contentLength;
                            ca.CATTACHMENTCONTENT = bytes;
                            //ca.FKCLIENTID = newItemID;
                           // db.CLIENTATTACHMENTs.Add(attchmnt);
                        }
                    }
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authentication]
        [HttpPost]
        public ActionResult DeleteItem(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            CLIENT item = (from i in db.CLIENTs where i.CLIENTID == itemId select i).SingleOrDefault();
            db.CLIENTs.Remove(item);
            db.SaveChanges();
            return Json("true");
        }

        //
        [Authentication]
        [HttpPost]
        public ActionResult CreateEpic(Epic newEpic)
        {
            long clientID = Convert.ToInt64(newEpic.epichyujkzsaa);
            SCRUMEntities db = new SCRUMEntities();
            EPIC epic = new EPIC();
            epic.EPICTITLE = newEpic.EPICTITLE;
            epic.EPICDESCRIPTION = newEpic.EPICDESCRIPTION.Replace("\r\n", "<br>"); ;
            epic.FKCLIENTID = clientID;

            db.EPICs.Add(epic);
            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges

                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return RedirectToAction("GetAllEpics", "Epic", new { cid = clientID.ToString()});
        }


        [Authentication]
        [HttpPost]
        public JsonResult GetItemDetails(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);

            SCRUMEntities db = new SCRUMEntities();


            var item = (from i in db.CLIENTs
                        where i.CLIENTID == itemId
                        select new Client
                        {
                            CLIENTID = i.CLIENTID,
                            CLIENTNAME = i.CLIENTNAME,
                            CLIENTLOCATION = i.CLIENTLOCATION,
                            CLIENTDESCRIPTION = i.CLIENTDESCRIPTION,
                            CLIENTPHONENUMBER = i.CLIENTPHONENUMBER,
                            ATTACHMENTSCOUNT = i.ATTACHMENTSCOUNT,
                           
                        }).SingleOrDefault();

            List<ClientAttachment> AttList = (from a in db.CLIENTATTACHMENTs
                                        where a.FKCLIENTID == itemId
                                        select new ClientAttachment
                                        {
                                             CATTACHMENTID = a.CATTACHMENTID,
                                              CATTACHMENTNAME = a.CATTACHMENTNAME,
                                               CATTACHMENTTYPE = a.CATTACHMENTTYPE,
                                                CATTACHMENTLENGTH = a.CATTACHMENTLENGTH,
                                                 CATTACHMENTCONTENT = a.CATTACHMENTCONTENT
                                            
                                        }).ToList();

            item.AttList = AttList;
            item.ClientImagePath = GetAndSaveClientPicture(item.AttList[0].CATTACHMENTID);
            return new JsonResult() { Data = item, MaxJsonLength = Int32.MaxValue };
        }

        [Authentication]
        public string GetAndSaveClientPicture(long attId)
        {
            long attachmentId = Convert.ToInt64(attId);

            byte[] fileContent = null;
            string fileType = "";
            string file_Name = "";

            SCRUMEntities db = new SCRUMEntities();
            var attDetails = (from a in db.CLIENTATTACHMENTs
                              where a.CATTACHMENTID == attachmentId
                              select a).SingleOrDefault();

            if(attDetails != null)
            {
                fileContent = attDetails.CATTACHMENTCONTENT;
                fileType = attDetails.CATTACHMENTTYPE;
                file_Name = attDetails.CATTACHMENTNAME;
                var path = Server.MapPath("~/ClientImages/") + file_Name;

                System.IO.File.WriteAllBytes(path, fileContent);
            }
            else
            {
                file_Name = "primeclient.PNG";
            }

            

            return "../../ClientImages/" + file_Name;
        }

        public string GetAndSavePersonsPicture(long pid)
        {
            long attachmentId = Convert.ToInt64(pid);

            byte[] fileContent = null;
            string fileType = "";
            string file_Name = "";

            SCRUMEntities db = new SCRUMEntities();
            var attDetails = (from a in db.PICTUREs
                              where a.FKPERSONID == attachmentId
                              select a).SingleOrDefault();

            if (attDetails == null)
            {
                return "../../ProfilePictures/default.png";
            }
            else
            {
                fileContent = attDetails.PERSONPICTURECONTENT;
                fileType = attDetails.PERSONPICTURETYPE;
                file_Name = attDetails.PERSONPICTURENAME;
                var path = Server.MapPath("~/ProfilePictures/") + file_Name;
                System.IO.File.WriteAllBytes(path, fileContent);
                return "../../ProfilePictures/" + file_Name;
            }
        }


        [Authentication]
        public long GetAttachmentId(long clientId)
        {
            long itemId = clientId;
            SCRUMEntities db = new SCRUMEntities();
            List<ClientAttachment> AttList = (from a in db.CLIENTATTACHMENTs
                                              where a.FKCLIENTID == itemId
                                              select new ClientAttachment
                                              {
                                                  CATTACHMENTID = a.CATTACHMENTID,
                                                  CATTACHMENTNAME = a.CATTACHMENTNAME,
                                                  CATTACHMENTTYPE = a.CATTACHMENTTYPE,
                                                  CATTACHMENTLENGTH = a.CATTACHMENTLENGTH,
                                                  CATTACHMENTCONTENT = a.CATTACHMENTCONTENT

                                              }).ToList();

            long CattId = 0;

            if(AttList.Count != 0)
            {
                CattId = AttList[0].CATTACHMENTID;
            }

            return CattId;
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