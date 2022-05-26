using System;
using System.Collections.Generic;
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
    public class EpicController : Controller
    {
        public Encryption encNewTeamMember;

        // GET: Epic
        public ActionResult Index()
        {
            var clientID = Convert.ToInt64(Session["ClientID"].ToString());
            encNewTeamMember = new Encryption();

            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {
                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                var items = from itm in db.EPICs.AsEnumerable()
                            where itm.FKCLIENTID == clientID
                            select new Epic
                            {
                                EPICID = itm.EPICID,
                                EPICTITLE = itm.EPICTITLE,
                                EPICDESCRIPTION = itm.EPICDESCRIPTION,
                                FKCLIENTID = itm.FKCLIENTID
                            };


                var clientName = (from c in db.CLIENTs.AsEnumerable() where c.CLIENTID == clientID select c).SingleOrDefault();

                Epic item = new Epic();
                item.Clientlist = items.ToList();
                item.LoggedInUser = Session["UserId"].ToString();
                item.PersonImagePath = GetAndSaveClientPicture(Convert.ToInt64(userid));
                item.ClientName = clientName.CLIENTNAME;


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
            return View("Index", "Epic");
        }

        public ActionResult GetAllEpics(string cid)
        {
            var clientID = Convert.ToInt64(cid);
            Session["ClientID"] = clientID;
            encNewTeamMember = new Encryption();

          

            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {
                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                var items = from itm in db.EPICs.AsEnumerable()
                            where itm.FKCLIENTID== clientID
                            select new Epic
                            {
                                EPICID = itm.EPICID,
                                EPICTITLE = itm.EPICTITLE,
                                EPICDESCRIPTION = itm.EPICDESCRIPTION,
                                FKCLIENTID = itm.FKCLIENTID
                            };


                var clientName = (from c in db.CLIENTs.AsEnumerable() where c.CLIENTID == clientID select c).SingleOrDefault();

                Epic item = new Epic();
                item.Clientlist = items.ToList();
                item.LoggedInUser = Session["UserId"].ToString();
                item.PersonImagePath = GetAndSaveClientPicture(Convert.ToInt64(userid));
                item.ClientName = clientName.CLIENTNAME;
                

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
            return View("Index", "Epic");
        }

        [Authentication]
        [HttpPost]
        public JsonResult GetEpicDetails(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);

            SCRUMEntities db = new SCRUMEntities();

            var item = (from i in db.EPICs.AsEnumerable()
                        where i.EPICID == itemId
                        select new Epic
                        {
                           EPICTITLE = i.EPICTITLE,
                           EPICDESCRIPTION = i.EPICDESCRIPTION,
                           EPICID = i.EPICID
                        }).SingleOrDefault();

            return new JsonResult() { Data = item, MaxJsonLength = Int32.MaxValue };
        }

        [Authentication]
        [HttpPost]
        public ActionResult DeleteThisEpic(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            EPIC item = (from i in db.EPICs where i.EPICID == itemId select i).SingleOrDefault();
            db.EPICs.Remove(item);
            db.SaveChanges();
            return Json("true");
        }


        [Authentication]
        [HttpPost]
        public ActionResult EditExistingEpic(Epic newItem)
        {
            var epicID = newItem.EPICID;
            var epicTitle = newItem.EPICTITLE;
            var epicDescription = newItem.EPICDESCRIPTION.Replace("\r\n", "<br>"); 
            var clientIdToEdit = Convert.ToInt64(Session["ClientID"].ToString());

            Encryption encpt = new Encryption();
            //Save new Item in ITEM table and get PK of new row
            SCRUMEntities db = new SCRUMEntities();
            var item = (from c in db.EPICs where c.EPICID == epicID select c).SingleOrDefault();

            item.EPICTITLE = epicTitle;
            item.EPICDESCRIPTION = epicDescription;


            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authentication]
        [HttpPost]
        public ActionResult AddNewProductBacklog(NewItem newItem)
        {
            //Get new item details
            var itemTitle = newItem.ItemTitle;
            var itemCreationDate = Convert.ToDateTime(newItem.ItemCreationDate);
            var attachmentsCount = newItem.Attachments.Count;
            var itemNotes = newItem.ItemDescription.Replace("\r\n", "<br>");
            var itemLocation = newItem.CustomLocation == null? "PB": newItem.CustomLocation.ToString();

            var linkCount = 0;

            if (newItem.LinkOne != null && newItem.LinkOne.Trim() != "")
                linkCount++;
            if (newItem.LinkTwo != null && newItem.LinkTwo.Trim() != "")
                linkCount++;
            if (newItem.LinkThree != null && newItem.LinkThree.Trim() != "")
                linkCount++;

            Encryption encpt = new Encryption();
            //Save new Item in ITEM table and get PK of new row
            SCRUMEntities db = new SCRUMEntities();
            PRODUCTBACKLOG item = new PRODUCTBACKLOG();
            item.ITEMTITLE = itemTitle;
            item.CREATIONDATE = itemCreationDate;
            item.ATTACHMENTSCOUNT = attachmentsCount;
            item.LINKCOUNT = linkCount;
            item.ITEMNOTES = itemNotes;
            item.ITEMLOCATION = itemLocation;
            //Session["UserId"]
            //Session["USERK"]
            //Session["USERF"]
            //hyujkzsaa
            Byte[] USERK = (Byte[])Session["USERK"];
            Byte[] USERF = (Byte[])Session["USERF"];

            item.FKEPICID = Convert.ToInt64(newItem.hyujkzsaa);
            // item.FKEPICID = Convert.ToInt64(encpt.Decrypt_Aes_With_Custom_Key(Server.UrlDecode(newItem.pbhyujkzsaa), USERK, USERF));
            db.PRODUCTBACKLOGs.Add(item);
            db.SaveChanges();

            //New Item ID
            var newItemID = item.ITEMID;


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
                            PBATTACHMENT attchmnt = new PBATTACHMENT
                            {
                                PBATTACHMENTNAME = fileName,
                                PBATTACHMENTTYPE = contentType,
                                PBATTACHMENTLENGTH = contentLength,
                                PBATTACHMENTCONTENT = bytes,
                                PBFKPRODUCTBACKLOG = newItemID
                            };
                            db.PBATTACHMENTs.Add(attchmnt);
                            db.SaveChanges();
                        }
                    }
                }
            }
            //// Add links
            if (newItem.LinkOne != null && newItem.LinkOne.Trim() != "")
            {
                PBHYPERLINK h1 = new PBHYPERLINK();
                h1.PBLINKDESCRIPTION = "LINK ONE";
                h1.PBLINKBODY = newItem.LinkOne.Trim();
                h1.PBFKPBACKLOG = newItemID;
                db.PBHYPERLINKs.Add(h1);
                db.SaveChanges();
            }
            if (newItem.LinkTwo != null && newItem.LinkTwo.Trim() != "")
            {
                PBHYPERLINK h2 = new PBHYPERLINK();
                h2.PBLINKDESCRIPTION = "LINK TWO";
                h2.PBLINKBODY = newItem.LinkTwo.Trim();
                h2.PBFKPBACKLOG = newItemID;
                db.PBHYPERLINKs.Add(h2);
                db.SaveChanges();
            }
            if (newItem.LinkThree != null && newItem.LinkThree.Trim() != "")
            {
                PBHYPERLINK h3 = new PBHYPERLINK();
                h3.PBLINKDESCRIPTION = "LINK THREE";
                h3.PBLINKBODY = newItem.LinkThree.Trim();
                h3.PBFKPBACKLOG = newItemID;
                db.PBHYPERLINKs.Add(h3);
              db.SaveChanges();
            }

            // db.SaveChanges();
            return RedirectToAction("GetAllProductBacklogs","ProductBacklog", new { epicId= newItem.hyujkzsaa } );
        }

        public string GetAndSaveClientPicture(long pid)
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
        [HttpPost]
        public ActionResult AllProductBacklogs(string eid)
        {
            return RedirectToAction("GetAllProductBacklogs", "ProductBacklog", new { epicId = eid });
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