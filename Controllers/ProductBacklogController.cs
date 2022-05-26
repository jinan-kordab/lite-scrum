using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using WebApplication4.Database;
using WebApplication4.Filters;
using WebApplication4.Helpers;
using System.Data.Entity.SqlServer;

namespace WebApplication4.Controllers
{
    [Authentication]
    public class ProductBacklogController : Controller
    {
        public Encryption encNewTeamMember;

        public ActionResult Index()
        {

            var epicId = Convert.ToInt64(Session["EpicID"].ToString());
            encNewTeamMember = new Encryption();
            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {

                //Session["userLoginKey"] = enc.Key;
                //Session["userLoginIV"] = enc.IV;
                var epicid = Convert.ToInt64(epicId);
                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                var items = from itm in db.PRODUCTBACKLOGs
                            where itm.FKEPICID == epicid
                            select new ProductBacklog
                            {
                                ITEMID = itm.ITEMID,
                                ITEMTITLE = itm.ITEMTITLE,
                                CREATIONDATE = itm.CREATIONDATE,
                                ITEMNOTES = itm.ITEMNOTES,
                                ITEMLOCATION = itm.ITEMLOCATION
                            };

                ProductBacklog item = new ProductBacklog();
                item.Itemlist = items.ToList();
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
            return View("Index", "ProductBacklog");
        }
        public ActionResult GetAllProductBacklogs(string epicId)
        {
            Session["EpicID"] = epicId;
            encNewTeamMember = new Encryption();
            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {

                //Session["userLoginKey"] = enc.Key;
                //Session["userLoginIV"] = enc.IV;
                var epicid = Convert.ToInt64(epicId);
                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                var items = from itm in db.PRODUCTBACKLOGs
                            where itm.FKEPICID == epicid
                            select new ProductBacklog
                            {
                                ITEMID = itm.ITEMID,
                                ITEMTITLE = itm.ITEMTITLE,
                                CREATIONDATE = itm.CREATIONDATE,
                                ITEMNOTES = itm.ITEMNOTES,
                                ITEMLOCATION = itm.ITEMLOCATION
                            };

                ProductBacklog item = new ProductBacklog();
                item.Itemlist = items.ToList();
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
            return View("Index", "ProductBacklog");
        }



        [Authentication]
        [HttpPost]
        public ActionResult MoveToRefined(string pbID)
        {
            long itemId = Convert.ToInt64(pbID);
            SCRUMEntities db = new SCRUMEntities();
            PRODUCTBACKLOG item = (from i in db.PRODUCTBACKLOGs where i.ITEMID == itemId select i).SingleOrDefault();
            item.ITEMLOCATION = "RB";
            db.SaveChanges();
            return Json("true");
        }
        [Authentication]
        [HttpPost]
        public ActionResult MoveToSprintBacklog(string pbID)
        {
            long itemId = Convert.ToInt64(pbID);
            SCRUMEntities db = new SCRUMEntities();
            PRODUCTBACKLOG item = (from i in db.PRODUCTBACKLOGs where i.ITEMID == itemId select i).SingleOrDefault();
            item.ITEMLOCATION = "SPRBG";
            db.SaveChanges();
            return Json("true");
        }

        [Authentication]
        [HttpPost]
        public ActionResult MoveToBacklog(string pbID)
        {
            long itemId = Convert.ToInt64(pbID);
            SCRUMEntities db = new SCRUMEntities();
            PRODUCTBACKLOG item = (from i in db.PRODUCTBACKLOGs where i.ITEMID == itemId select i).SingleOrDefault();
            item.ITEMLOCATION = "PB";
            db.SaveChanges();
            return Json("true");
        }

        [Authentication]
        [HttpPost]
        public ActionResult MoveToSprintPlanning(string pbID)
        {
            long itemId = Convert.ToInt64(pbID);
            SCRUMEntities db = new SCRUMEntities();
            PRODUCTBACKLOG item = (from i in db.PRODUCTBACKLOGs where i.ITEMID == itemId select i).SingleOrDefault();
            item.ITEMLOCATION = "SPRPL";
            db.SaveChanges();
            return Json("true");
        }



        [Authentication]
        [HttpPost]
        public JsonResult GetItemDetails(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);

            SCRUMEntities db = new SCRUMEntities();


            var item = (from i in db.PRODUCTBACKLOGs.AsEnumerable()
                        where i.ITEMID == itemId
                        select new ProductBacklog
                        {
                            ITEMID = i.ITEMID,
                            ITEMTITLE = i.ITEMTITLE,
                            CREATIONDATE = i.CREATIONDATE,
                            ATTACHMENTSCOUNT = i.ATTACHMENTSCOUNT,
                            LINKCOUNT = i.LINKCOUNT,
                            ITEMNOTES = i.ITEMNOTES,
                            ITEMLOCATION = i.ITEMLOCATION,
                            LINKONE = i.PBHYPERLINKs.Count > 0 ? i.PBHYPERLINKs.Where(x => x.PBLINKDESCRIPTION == "LINK ONE").Select(s => s.PBLINKBODY).FirstOrDefault() : "NO LINK",
                            LINKTWO = i.PBHYPERLINKs.Count > 0 ? i.PBHYPERLINKs.Where(x => x.PBLINKDESCRIPTION == "LINK TWO").Select(s => s.PBLINKBODY).FirstOrDefault() : "NO LINK",
                            LINKTHREE = i.PBHYPERLINKs.Count > 0 ? i.PBHYPERLINKs.Where(x => x.PBLINKDESCRIPTION == "LINK THREE").Select(s => s.PBLINKBODY).FirstOrDefault() : "NO LINK",
                        }).SingleOrDefault();

            List<Attachment> AttList = (from a in db.PBATTACHMENTs
                                        where a.PBFKPRODUCTBACKLOG == itemId
                                        select new Attachment
                                        {
                                            ATTITEMID = a.PBATTITEMID,
                                            ATTACHMENTNAME = a.PBATTACHMENTNAME
                                            //ATTACHMENTTYPE = a.ATTACHMENTTYPE,
                                            // ATTACHMENTLENGTH = a.ATTACHMENTLENGTH,
                                            //   ATTACHMENTCONTENT = a.ATTACHMENTCONTENT
                                        }).ToList();

            item.AttList = AttList;

            return new JsonResult() { Data = item, MaxJsonLength = Int32.MaxValue };
        }
        [Authentication]
        [HttpPost]
        public ActionResult DeleteAttachment(int ItemId)
        {
            long attItemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            PBATTACHMENT item = (from i in db.PBATTACHMENTs where i.PBATTITEMID == attItemId select i).SingleOrDefault();
            db.PBATTACHMENTs.Remove(item);
            db.SaveChanges();
            return Json("true");
        }

        [Authentication]
        public FileContentResult GetFile(int attId)
        {
            long attachmentId = Convert.ToInt64(attId);

            byte[] fileContent = null;
            string fileType = "";
            string file_Name = "";

            SCRUMEntities db = new SCRUMEntities();
            var attDetails = (from a in db.PBATTACHMENTs
                              where a.PBATTITEMID == attachmentId
                              select a).SingleOrDefault();

            fileContent = attDetails.PBATTACHMENTCONTENT;
            fileType = attDetails.PBATTACHMENTTYPE;
            file_Name = attDetails.PBATTACHMENTNAME;

            return File(fileContent, fileType, file_Name);
        }



        [Authentication]
        [HttpPost]
        public ActionResult UpdateExistingItem(NewItem existingItem)
        {
           var MyUrl = Request.UrlReferrer;

            //Get new item details
            var existingItemIdToUpdate = Convert.ToInt64(existingItem.ItemId);
            var itemTitle = existingItem.ItemTitle;
            var itemCreationDate = Convert.ToDateTime(existingItem.ItemCreationDate);
            var attachmentsCount = existingItem.Attachments.Count;
            var itemNotes = existingItem.ItemDescription.Replace("\r\n", "<br>");

            var linkCount = 0;

            if (existingItem.LinkOne != null && existingItem.LinkOne.Trim() != "")
                linkCount++;
            if (existingItem.LinkTwo != null && existingItem.LinkTwo.Trim() != "")
                linkCount++;
            if (existingItem.LinkThree != null && existingItem.LinkThree.Trim() != "")
                linkCount++;

            //Save new Item in ITEM table and get PK of new row
            SCRUMEntities db = new SCRUMEntities();
            var existItem = (from i in db.PRODUCTBACKLOGs where i.ITEMID == existingItemIdToUpdate select i).SingleOrDefault();
            existItem.ITEMTITLE = itemTitle;
            existItem.CREATIONDATE = itemCreationDate;
            existItem.ATTACHMENTSCOUNT = attachmentsCount + existItem.ATTACHMENTSCOUNT;
            existItem.LINKCOUNT = linkCount;
            existItem.ITEMNOTES = itemNotes;
            // db.SaveChanges();


            //Get and save Attachments
            if (existingItem.Attachments[0] != null)
            {
                foreach (var attachment in existingItem.Attachments)
                {
                    var fileName = attachment.FileName;           //db field
                    var contentType = attachment.ContentType;     //db field
                    var contentLength = attachment.ContentLength; //db field

                    using (Stream st = attachment.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(st))
                        {
                            byte[] bytes = br.ReadBytes((Int32)st.Length);  //db field
                            PBATTACHMENT attchmnt = new PBATTACHMENT();
                            attchmnt.PBATTACHMENTNAME = fileName;
                            attchmnt.PBATTACHMENTTYPE = contentType;
                            attchmnt.PBATTACHMENTLENGTH = contentLength;
                            attchmnt.PBATTACHMENTCONTENT = bytes;
                            attchmnt.PBFKPRODUCTBACKLOG = existingItemIdToUpdate;
                            db.PBATTACHMENTs.Add(attchmnt);

                        }
                    }
                }
            }

            // Update links

            //HYPERLINK h1 = new HYPERLINK();
            var h1 = (from i in db.PBHYPERLINKs where (i.PBFKPBACKLOG == existingItemIdToUpdate && i.PBLINKDESCRIPTION == "LINK ONE") select i).SingleOrDefault();
            if (h1 != null)
            {
                h1.PBLINKDESCRIPTION = "LINK ONE";
                h1.PBLINKBODY = existingItem.LinkOne == null ? "NO LINK" : existingItem.LinkOne.Trim();

            }
            else
            {
                PBHYPERLINK hl1 = new PBHYPERLINK();
                hl1.PBLINKDESCRIPTION = "LINK ONE";
                hl1.PBLINKBODY = existingItem.LinkOne == null ? "NO LINK" : existingItem.LinkOne.Trim();
                hl1.PBFKPBACKLOG = existingItemIdToUpdate;
                db.PBHYPERLINKs.Add(hl1);
            }


            //HYPERLINK h1 = new HYPERLINK();
            var h2 = (from i in db.PBHYPERLINKs where (i.PBFKPBACKLOG == existingItemIdToUpdate && i.PBLINKDESCRIPTION == "LINK TWO") select i).SingleOrDefault();
            if (h2 != null)
            {
                h2.PBLINKDESCRIPTION = "LINK TWO";
                h2.PBLINKBODY = existingItem.LinkTwo == null ? "NO LINK" : existingItem.LinkTwo.Trim();

            }
            else
            {
                PBHYPERLINK hl2 = new PBHYPERLINK();
                hl2.PBLINKDESCRIPTION = "LINK TWO";
                hl2.PBLINKBODY = existingItem.LinkTwo == null ? "NO LINK" : existingItem.LinkTwo.Trim();
                hl2.PBFKPBACKLOG = existingItemIdToUpdate;
                db.PBHYPERLINKs.Add(hl2);
            }



            var h3 = (from i in db.PBHYPERLINKs where (i.PBFKPBACKLOG == existingItemIdToUpdate && i.PBLINKDESCRIPTION == "LINK THREE") select i).SingleOrDefault();
            if (h3 != null)
            {
                h3.PBLINKDESCRIPTION = "LINK THREE";
                h3.PBLINKBODY = existingItem.LinkThree == null ? "NO LINK" : existingItem.LinkThree.Trim();

            }
            else
            {
                PBHYPERLINK hl3 = new PBHYPERLINK();
                hl3.PBLINKDESCRIPTION = "LINK THREE";
                hl3.PBLINKBODY = existingItem.LinkThree == null ? "NO LINK" : existingItem.LinkThree.Trim();
                hl3.PBFKPBACKLOG = existingItemIdToUpdate;
                db.PBHYPERLINKs.Add(hl3);
            }






            db.SaveChanges();


            if (MyUrl.ToString().Contains("/Sprint"))
                return RedirectToAction("Index","Sprint"); 



            return RedirectToAction("GetAllProductBacklogs", new { epicId= Session["EpicID"].ToString() });
        }



        [Authentication]
        [HttpPost]
        public ActionResult DeleteItem(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            PRODUCTBACKLOG item = (from i in db.PRODUCTBACKLOGs where i.ITEMID == itemId select i).SingleOrDefault();
            db.PRODUCTBACKLOGs.Remove(item);
            db.SaveChanges();
            return Json("true");
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


        [HttpPost]
        public RedirectToRouteResult Culture(Language lang)
        {
            var languageSelected = lang.lAction;
            Session["LANG"] = languageSelected;
            return RedirectToAction("Index");
        }

    }
}