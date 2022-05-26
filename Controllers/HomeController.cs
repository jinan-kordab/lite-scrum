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
using System.Security.Cryptography;

namespace WebApplication4.Controllers
{

    public class HomeController : Controller
    {

        public Encryption encNewTeamMember;

        [Authentication]
        public ActionResult Index()
        {
            encNewTeamMember = new Encryption();
            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {

                //Session["userLoginKey"] = enc.Key;
                //Session["userLoginIV"] = enc.IV;

                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                var items = from itm in db.ITEMs
                            where itm.FKPERSONID == userid
                            select new Item
                            {
                                ITEMID = itm.ITEMID,
                                ITEMTITLE = itm.ITEMTITLE,
                                CREATIONDATE = itm.CREATIONDATE,
                                ITEMNOTES = itm.ITEMNOTES,
                                ITEMLOCATION = itm.ITEMLOCATION
                            };

                Item item = new Item();
                item.Itemlist = items.ToList();
                item.LoggedInUser = Session["UserId"].ToString();
                item.PersonImagePath = GetAndSaveClientPicture(Convert.ToInt64(userid));
                var userDetails = (from a in db.People where a.USERID == userid select a).SingleOrDefault();


                var usersRank = "";

                //usersRank = userDetails.ISTEAMCREATOR ? "Team Creator" : "Regular Team Member";


                if (Session["LANG"] != null)
                {
                    if (Session["LANG"].ToString() == "fr")
                    {
                        usersRank = userDetails.ISTEAMCREATOR ? "Créateur d'équipe" : "Membre régulier de l'équipe";
                    }
                    else
                    {
                        usersRank = userDetails.ISTEAMCREATOR ? "Team Creator" : "Regular Team Member";
                    }
                }
                else
                {
                    usersRank = userDetails.ISTEAMCREATOR ? "Team Creator" : "Regular Team Member";
                }





                item.FullNamePlusRank = userDetails.FIRSTNAME + " " + userDetails.LASTNAME + "," + usersRank;
                var usersTeamDetails = (from t in db.TEAMs where t.FKPERSONID == userid select t).FirstOrDefault();
                item.UsersTeam = usersTeamDetails.TEAMNAME;

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
            return View("Index", "Login");
        }
        [Authentication]
        [HttpPost]
        public ActionResult AddNewItem(NewItem newItem)
        {
            encNewTeamMember = new Encryption();
            var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                  (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

            //Get new item details
            var itemTitle = newItem.ItemTitle;
            var itemCreationDate = Convert.ToDateTime(newItem.ItemCreationDate);
            var attachmentsCount = newItem.Attachments.Count;
            var itemNotes = newItem.ItemDescription.Replace("\r\n", "<br>");
            var itemLocation = "TODO";
            var PersonId = userid;
            var ProductBacklogId = newItem.PBelect;


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
            ITEM item = new ITEM();
            item.ITEMTITLE = itemTitle;
            item.CREATIONDATE = itemCreationDate;
            item.ATTACHMENTSCOUNT = attachmentsCount;
            item.LINKCOUNT = linkCount;
            item.ITEMNOTES = itemNotes;
            item.ITEMLOCATION = itemLocation;
            item.FKPERSONID = Convert.ToInt64(PersonId);
            item.FKPRODUCTBACKLOGID = Convert.ToInt64(ProductBacklogId);


            Byte[] USERK = (Byte[])Session["USERK"];
            Byte[] USERF = (Byte[])Session["USERF"];


            db.ITEMs.Add(item);
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
                            ATTACHMENT attchmnt = new ATTACHMENT();
                            attchmnt.ATTACHMENTNAME = fileName;
                            attchmnt.ATTACHMENTTYPE = contentType;
                            attchmnt.ATTACHMENTLENGTH = contentLength;
                            attchmnt.ATTACHMENTCONTENT = bytes;
                            attchmnt.FKITEMID = newItemID;
                            db.ATTACHMENTs.Add(attchmnt);
                            //db.SaveChanges();
                        }
                    }
                }
            }
            // Add links
            if (newItem.LinkOne != null && newItem.LinkOne.Trim() != "")
            {
                HYPERLINK h1 = new HYPERLINK();
                h1.LINKDESCRIPTION = "LINK ONE";
                h1.LINKBODY = newItem.LinkOne.Trim();
                h1.FKITEMID = newItemID;
                db.HYPERLINKs.Add(h1);
                //db.SaveChanges();
            }
            if (newItem.LinkTwo != null && newItem.LinkTwo.Trim() != "")
            {
                HYPERLINK h2 = new HYPERLINK();
                h2.LINKDESCRIPTION = "LINK TWO";
                h2.LINKBODY = newItem.LinkTwo.Trim();
                h2.FKITEMID = newItemID;
                db.HYPERLINKs.Add(h2);
                //db.SaveChanges();
            }
            if (newItem.LinkThree != null && newItem.LinkThree.Trim() != "")
            {
                HYPERLINK h3 = new HYPERLINK();
                h3.LINKDESCRIPTION = "LINK THREE";
                h3.LINKBODY = newItem.LinkThree.Trim();
                h3.FKITEMID = newItemID;
                db.HYPERLINKs.Add(h3);
                // db.SaveChanges();
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authentication]
        [HttpPost]
        public ActionResult UpdateExistingItem(NewItem existingItem)
        {
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
            var existItem = (from i in db.ITEMs where i.ITEMID == existingItemIdToUpdate select i).SingleOrDefault();
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
                            ATTACHMENT attchmnt = new ATTACHMENT();
                            attchmnt.ATTACHMENTNAME = fileName;
                            attchmnt.ATTACHMENTTYPE = contentType;
                            attchmnt.ATTACHMENTLENGTH = contentLength;
                            attchmnt.ATTACHMENTCONTENT = bytes;
                            attchmnt.FKITEMID = existingItemIdToUpdate;
                            db.ATTACHMENTs.Add(attchmnt);

                        }
                    }
                }
            }

            // Update links

            //HYPERLINK h1 = new HYPERLINK();
            var h1 = (from i in db.HYPERLINKs where (i.FKITEMID == existingItemIdToUpdate && i.LINKDESCRIPTION == "LINK ONE") select i).SingleOrDefault();
            if (h1 != null)
            {
                h1.LINKDESCRIPTION = "LINK ONE";
                h1.LINKBODY = existingItem.LinkOne == null ? "NO LINK" : existingItem.LinkOne.Trim();

            }
            else
            {
                HYPERLINK hl1 = new HYPERLINK();
                hl1.LINKDESCRIPTION = "LINK ONE";
                hl1.LINKBODY = existingItem.LinkOne == null ? "NO LINK" : existingItem.LinkOne.Trim();
                hl1.FKITEMID = existingItemIdToUpdate;
                db.HYPERLINKs.Add(hl1);
            }


            //HYPERLINK h1 = new HYPERLINK();
            var h2 = (from i in db.HYPERLINKs where (i.FKITEMID == existingItemIdToUpdate && i.LINKDESCRIPTION == "LINK TWO") select i).SingleOrDefault();
            if (h2 != null)
            {
                h2.LINKDESCRIPTION = "LINK TWO";
                h2.LINKBODY = existingItem.LinkTwo == null ? "NO LINK" : existingItem.LinkTwo.Trim();

            }
            else
            {
                HYPERLINK hl2 = new HYPERLINK();
                hl2.LINKDESCRIPTION = "LINK TWO";
                hl2.LINKBODY = existingItem.LinkTwo == null ? "NO LINK" : existingItem.LinkTwo.Trim();
                hl2.FKITEMID = existingItemIdToUpdate;
                db.HYPERLINKs.Add(hl2);
            }



            var h3 = (from i in db.HYPERLINKs where (i.FKITEMID == existingItemIdToUpdate && i.LINKDESCRIPTION == "LINK THREE") select i).SingleOrDefault();
            if (h3 != null)
            {
                h3.LINKDESCRIPTION = "LINK THREE";
                h3.LINKBODY = existingItem.LinkThree == null ? "NO LINK" : existingItem.LinkThree.Trim();

            }
            else
            {
                HYPERLINK hl3 = new HYPERLINK();
                hl3.LINKDESCRIPTION = "LINK THREE";
                hl3.LINKBODY = existingItem.LinkThree == null ? "NO LINK" : existingItem.LinkThree.Trim();
                hl3.FKITEMID = existingItemIdToUpdate;
                db.HYPERLINKs.Add(hl3);
            }






            db.SaveChanges();
            var MyUrl = Request.UrlReferrer;
            if (MyUrl.ToString().Contains("/Sprint"))
                return RedirectToAction("Index", "Sprint");

            return RedirectToAction("Index");
        }
        [Authentication]
        [HttpPost]
        public ActionResult MoveToInProgress(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            ITEM item = (from i in db.ITEMs where i.ITEMID == itemId select i).SingleOrDefault();
            item.ITEMLOCATION = item.ITEMLOCATION.Contains("QA") ? "INPROGQA" :"INPROGRESS";
            db.SaveChanges();
            return Json("true");
        }
        [Authentication]
        [HttpPost]
        public ActionResult MoveToToDo(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            ITEM item = (from i in db.ITEMs where i.ITEMID == itemId select i).SingleOrDefault();
            //item.ITEMLOCATION = "TODO";
            item.ITEMLOCATION = item.ITEMLOCATION.Contains("QA") ? "TODOQA" : "TODO";
            db.SaveChanges();
            return Json("true");
        }

        [Authentication]
        [HttpPost]
        public ActionResult MoveToToQA(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            ITEM item = (from i in db.ITEMs where i.ITEMID == itemId select i).SingleOrDefault();
            item.ITEMLOCATION = item.ITEMLOCATION.Contains("QA") ? "QAQA" : "QA";
            //item.ITEMLOCATION = "QA";
            db.SaveChanges();
            return Json("true");
        }



        [Authentication]
        [HttpPost]
        public ActionResult MoveToDone(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            ITEM item = (from i in db.ITEMs where i.ITEMID == itemId select i).SingleOrDefault();
            item.ITEMLOCATION = item.ITEMLOCATION.Contains("QAQA") ? "DONEQA" : "DONE";
            //item.ITEMLOCATION = "DONE";
            db.SaveChanges();
            return Json("true");
        }
        [Authentication]
        [HttpPost]
        public ActionResult DeleteItem(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            ITEM item = (from i in db.ITEMs where i.ITEMID == itemId select i).SingleOrDefault();
            db.ITEMs.Remove(item);
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                var a = exp.ToString();
            }
            return Json("true");
        }
        [Authentication] 
        [HttpPost]
        public JsonResult GetItemDetails(string ItemId)
        {
            long itemId = Convert.ToInt64(ItemId);

            SCRUMEntities db = new SCRUMEntities();


            var item = (from i in db.ITEMs
                        where i.ITEMID == itemId
                        select new Item
                        {
                            ITEMID = i.ITEMID,
                            ITEMTITLE = i.ITEMTITLE,
                            CREATIONDATE = i.CREATIONDATE,
                            ATTACHMENTSCOUNT = i.ATTACHMENTSCOUNT,
                            LINKCOUNT = i.LINKCOUNT,
                            ITEMNOTES = i.ITEMNOTES,
                            ITEMLOCATION = i.ITEMLOCATION,
                            LINKONE = i.HYPERLINKs.Count > 0 ? i.HYPERLINKs.Where(x => x.LINKDESCRIPTION == "LINK ONE").Select(s => s.LINKBODY).FirstOrDefault() : "NO LINK",
                            LINKTWO = i.HYPERLINKs.Count > 0 ? i.HYPERLINKs.Where(x => x.LINKDESCRIPTION == "LINK TWO").Select(s => s.LINKBODY).FirstOrDefault() : "NO LINK",
                            LINKTHREE = i.HYPERLINKs.Count > 0 ? i.HYPERLINKs.Where(x => x.LINKDESCRIPTION == "LINK THREE").Select(s => s.LINKBODY).FirstOrDefault() : "NO LINK",
                        }).SingleOrDefault();

            List<Attachment> AttList = (from a in db.ATTACHMENTs
                                        where a.FKITEMID == itemId
                                        select new Attachment
                                        {
                                            ATTITEMID = a.ATTITEMID,
                                            ATTACHMENTNAME = a.ATTACHMENTNAME
                                            //ATTACHMENTTYPE = a.ATTACHMENTTYPE,
                                            // ATTACHMENTLENGTH = a.ATTACHMENTLENGTH,
                                            //   ATTACHMENTCONTENT = a.ATTACHMENTCONTENT
                                        }).ToList();

            item.AttList = AttList;

            return new JsonResult() { Data = item, MaxJsonLength = Int32.MaxValue };
        }
        [Authentication]
        public FileContentResult GetFile(int attId)
        {
            long attachmentId = Convert.ToInt64(attId);

            byte[] fileContent = null;
            string fileType = "";
            string file_Name = "";

            SCRUMEntities db = new SCRUMEntities();
            var attDetails = (from a in db.ATTACHMENTs
                              where a.ATTITEMID == attachmentId
                              select a).SingleOrDefault();

            fileContent = attDetails.ATTACHMENTCONTENT;
            fileType = attDetails.ATTACHMENTTYPE;
            file_Name = attDetails.ATTACHMENTNAME;

            return File(fileContent, fileType, file_Name);
        }
        [Authentication]
        [HttpPost]
        public ActionResult DeleteAttachment(int ItemId)
        {
            long attItemId = Convert.ToInt64(ItemId);
            SCRUMEntities db = new SCRUMEntities();
            ATTACHMENT item = (from i in db.ATTACHMENTs where i.ATTITEMID == attItemId select i).SingleOrDefault();
            db.ATTACHMENTs.Remove(item);
            db.SaveChanges();
            return Json("true");
        }

        [AllowAnonymous]
        public JsonResult GetPeople(JqueryDatatableParam param)
        {

            encNewTeamMember = new Encryption();

            if (Session["ENCKEYNEWTEAMMEMBERS"] == null)
            {


                Session["ENCKEYNEWTEAMMEMBERS"] = encNewTeamMember.Key;
                Session["ENCIVNEWTEAMMEMBERS"] = encNewTeamMember.IV;
            }


            if (Session["UserLoggedIn"] != null)
            {
                encNewTeamMember = new Encryption();

                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                   (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));


                var s = Session["ENCKEYNEWTEAMMEMBERS"].ToString();

                SCRUMEntities db = new SCRUMEntities();

                List<Person> ppl = new List<Person>();

                IEnumerable<TEAM> personsTeams = (from t in db.TEAMs.AsEnumerable() where t.FKPERSONID == userid select t).ToList();

                foreach (var personsTeam in personsTeams)
                {
                    //get all users that are in those teams

                    List<long> FKPERSONIDS = (from pit in db.TEAMs where pit.TEAMNAME == personsTeam.TEAMNAME select pit.FKPERSONID).ToList();

                    foreach (var prsn in FKPERSONIDS)
                    {
                        if (prsn != userid)
                        {
                            Person person = (from p in db.People.AsEnumerable()
                                             where p.USERID == prsn
                                             select new Person
                                             {
                                                 USERID = p.USERID.ToString(),
                                                 NICKNAME = p.NICKNAME,
                                                 FIRSTNAME = p.FIRSTNAME,
                                                 LASTNAME = p.LASTNAME
                                             }).SingleOrDefault();

                            ppl.Add(person);

                        }
                    }
                }

                IEnumerable<Person> people = ppl.AsEnumerable();

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    people = people.Where(x => x.FIRSTNAME.ToLower().Contains(param.sSearch.ToLower())
                                                  || x.LASTNAME.ToLower().Contains(param.sSearch.ToLower())
                                                  || x.EMAIL.ToLower().Contains(param.sSearch.ToLower())
                                                  || x.USERID.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();


                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"];
                if (sortColumnIndex == 1)
                {
                    people = sortDirection == "asc" ? people.OrderBy(c => c.FIRSTNAME) : people.OrderByDescending(c => c.FIRSTNAME);
                }
                else if (sortColumnIndex == 2)
                {
                    people = sortDirection == "asc" ? people.OrderBy(c => c.LASTNAME) : people.OrderByDescending(c => c.LASTNAME);
                }
                else if (sortColumnIndex == 3)
                {
                    people = sortDirection == "asc" ? people.OrderBy(c => c.EMAIL) : people.OrderByDescending(c => c.EMAIL);
                }
                else if (sortColumnIndex == 4)
                {
                    people = sortDirection == "asc" ? people.OrderBy(c => c.USERID) : people.OrderByDescending(c => c.USERID);
                }
                else
                {
                    Func<Person, string> orderingFunction = e => sortColumnIndex == 0 ? e.FIRSTNAME : sortColumnIndex == 1 ? e.LASTNAME : sortColumnIndex == 2 ? e.EMAIL : e.USERID.ToString();
                    people = sortDirection == "asc" ? people.OrderBy(orderingFunction) : people.OrderByDescending(orderingFunction);
                }

                var displayResult = people.Skip(param.iDisplayStart)
                  .Take(param.iDisplayLength).ToList();
                var totalRecords = people.Count();

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = displayResult
                }, JsonRequestBehavior.AllowGet);
            }

            Person pf = new Person();
            pf.FIRSTNAME = "";
            pf.LASTNAME = "";
            pf.EMAIL = "";
            pf.USERID = "0";

            return Json(new
            {
                param.sEcho,
                iTotalRecords = 0,
                iTotalDisplayRecords = 0,
                aaData = pf
            }, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        public JsonResult AddMembersToTeam(string[][] arrayofpersons)
        {
            SCRUMEntities db = new SCRUMEntities();
            //Encryption encpt = new Encryption();
            encNewTeamMember = new Encryption();
            var firstval = arrayofpersons[0][1].ToString();

            string currentUserTeamName = "";
            long currentUserID = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key(Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"])); ;

            foreach (var person in arrayofpersons)
            {
                var TeamName = person[0].ToString();
                long PersonId = Convert.ToInt64(person[1].ToString());
                //var PersonId = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key(Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                currentUserTeamName = TeamName;

                TEAM team = new TEAM();
                team.TEAMNAME = TeamName;
                team.FKPERSONID = PersonId;

                db.TEAMs.Add(team);
            }

            TEAM teamUsr = new TEAM();
            teamUsr.TEAMNAME = currentUserTeamName;
            teamUsr.FKPERSONID = currentUserID;
            db.TEAMs.Add(teamUsr);

            db.SaveChanges();

            return Json("True");
        }


        public ActionResult GetAllClientsListForNewTask()
        {
            encNewTeamMember = new Encryption();
            var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                   (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

            SCRUMEntities db = new SCRUMEntities();
            IEnumerable<Client> clients = (from p in db.CLIENTs.AsEnumerable() where p.FKPERSONID == userid
                                           select new Client
                                           {
                                               CLIENTID = p.CLIENTID,
                                               CLIENTNAME = p.CLIENTNAME
                                           }).ToList();

            return Json(clients, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllEpicsListForNewTask(string ClientId)
        {
            long cid = Convert.ToInt64(ClientId);
            SCRUMEntities db = new SCRUMEntities();
            IEnumerable<Epic> clients = (from p in db.EPICs.AsEnumerable() where p.FKCLIENTID == cid
                                         select new Epic
                                           {
                                               EPICID = p.EPICID,
                                               EPICTITLE = p.EPICTITLE
                                           }).ToList();

            return Json(clients, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPBsForAnEpic(string EpicId)
        {
            long eId = Convert.ToInt64(EpicId);
            SCRUMEntities db = new SCRUMEntities();
            IEnumerable<ProductBacklog> pbs = (from p in db.PRODUCTBACKLOGs.AsEnumerable() where p.FKEPICID == eId && p.ITEMLOCATION == "PB"
                                               select new ProductBacklog
                                         {
                                             ITEMID = p.ITEMID,
                                             ITEMTITLE = p.ITEMTITLE

                                         }).ToList();

            return Json(pbs, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetAllTeamsPerUser()
        {
            encNewTeamMember = new Encryption();
            var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                 (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));


            SCRUMEntities db = new SCRUMEntities();
            IEnumerable<TEAM> teams = (from p in db.TEAMs.AsEnumerable() where p.FKPERSONID == userid
                                          select new TEAM
                                          {
                                            TEAMID = p.TEAMID,
                                            TEAMNAME = p.TEAMNAME
                                          }).ToList();

            return Json(teams, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult RegisterNewUser(Person newPerson)
        {

            Encryption enc = new Encryption();

            var TEAMNAME = newPerson.TEAMNAME;
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
                p.ISTEAMCREATOR = false;
                db.People.Add(p);

                var newPersonID = p.USERID;

                //Add person to selected team
                encNewTeamMember = new Encryption();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                   (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));
               
                var team = new TEAM();
               
                   team = (from t in db.TEAMs where t.FKPERSONID == userid select t).SingleOrDefault();
                

                TEAM newTeam = new TEAM();
                newTeam.TEAMNAME = team.TEAMNAME;
                newTeam.FKPERSONID = newPersonID;
                db.TEAMs.Add(newTeam);


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

        //RegisterNewUserInternal
        [HttpPost]
        public ActionResult RegisterNewUserInternal(Person newPerson)
        {

            Encryption enc = new Encryption();

            var TEAMNAME = newPerson.TEAMNAME;
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
                p.ISTEAMCREATOR = false;
                db.People.Add(p);
                db.SaveChanges();
                var newPersonID = p.USERID;

                //Add person to selected team
                encNewTeamMember = new Encryption();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                   (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                //var team = new TEAM();
                //team = (from t in db.TEAMs where t.FKPERSONID == userid select t).SingleOrDefault();

                long tID = Convert.ToInt64(TEAMNAME);

                string TeamName = (from t in db.TEAMs where t.TEAMID == tID select t.TEAMNAME).SingleOrDefault().ToString();
                TEAM newTeam = new TEAM();
                newTeam.TEAMNAME = TeamName;
                newTeam.FKPERSONID = newPersonID;
                db.TEAMs.Add(newTeam);
                db.SaveChanges();

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
                            db.SaveChanges();
                        }
                    }

                }

                //db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult GetUserDetails()
        {
            encNewTeamMember = new Encryption();
            var userId = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

            SCRUMEntities db = new SCRUMEntities();


            var item = (from i in db.People.AsEnumerable()
                        where i.USERID == userId
                        select new Person
                        {
                           USERID = i.USERID.ToString(),
                           NICKNAME = i.NICKNAME,
                           FIRSTNAME = i.FIRSTNAME,
                           LASTNAME = i.LASTNAME,
                           EMAIL = i.EMAIL,
                           REGISTRATIONDATE = i.REGISTRATIONDATE,
                           PASSWORD = i.PASSWORD,
                           ISTEAMCREATOR = i.ISTEAMCREATOR
                        }).SingleOrDefault();

         
            Picture userPic = (from a in db.PICTUREs.AsEnumerable()
                                              where a.FKPERSONID == userId
                               select new Picture
                                              {
                                                  PERSONPICTUREID = a.PERSONPICTUREID,
                                                  PERSONPICTURENAME = a.PERSONPICTURENAME,
                                                  PERSONPICTURETYPE = a.PERSONPICTURETYPE,
                                                  PERSONPICTURELENGTH = a.PERSONPICTURELENGTH,
                                                  PERSONPICTURECONTENT = a.PERSONPICTURECONTENT
                                              }).SingleOrDefault();

            
            item.ClientImagePath = GetAndSaveClientPicture(userId);
            return new JsonResult() { Data = item, MaxJsonLength = Int32.MaxValue };
        }


        [HttpPost]
        public ActionResult EditExistingUser(Person newItem)
        {

            encNewTeamMember = new Encryption();
            var userId = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));


            var userNickname = newItem.NICKNAME;
            var userFIRSTNAME = newItem.FIRSTNAME;
            var userLASTNAME = newItem.LASTNAME;
            var userEMAIL = newItem.EMAIL;
            //var userREGISTRATIONDATE = newItem.REGISTRATIONDATE;

         
            Encryption encpt = new Encryption();
            //Save new Item in ITEM table and get PK of new row
            SCRUMEntities db = new SCRUMEntities();
            var item = (from c in db.People where c.USERID == userId select c).SingleOrDefault();

            item.NICKNAME = userNickname;
            item.FIRSTNAME = userFIRSTNAME;
            item.LASTNAME = userLASTNAME;
            item.EMAIL = userEMAIL;
            //item.REGISTRATIONDATE = userREGISTRATIONDATE;


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
                            var ca = (from ct in db.PICTUREs where ct.FKPERSONID == userId select ct).SingleOrDefault();
                            if (ca != null)
                            {
                                ca.PERSONPICTURENAME = fileName;
                                ca.PERSONPICTURETYPE = contentType;
                                ca.PERSONPICTURELENGTH = contentLength;
                                ca.PERSONPICTURECONTENT = bytes;
                            }
                            else
                            {
                                using (Stream sti = attachment.InputStream)
                                {
                                    using (BinaryReader bri = new BinaryReader(st))
                                    {
                                        byte[] bytesi = br.ReadBytes((Int32)st.Length);  //db field
                                        PICTURE attchmnt = new PICTURE();
                                        attchmnt.PERSONPICTURENAME = fileName;
                                        attchmnt.PERSONPICTURETYPE = contentType;
                                        attchmnt.PERSONPICTURELENGTH = contentLength;
                                        attchmnt.PERSONPICTURECONTENT = bytes;
                                        attchmnt.FKPERSONID = userId;
                                        db.PICTUREs.Add(attchmnt);
                                        //db.SaveChanges();
                                    }
                                }

                            }
                        }
                    }
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }




        [Authentication]
        [HttpPost]
        public ActionResult ChangeItemsOwner(Item itemtoupdate)
        {
            //Get new item details
            var ItemId = itemtoupdate.ITEMID;
            var FkPersonsId = itemtoupdate.PERSONID;

            
            SCRUMEntities db = new SCRUMEntities();

            var currentItem = (from i in db.ITEMs where i.ITEMID == ItemId select i).SingleOrDefault();
            currentItem.FKPERSONID = FkPersonsId;
            currentItem.ITEMLOCATION = "TODOQA";

            db.SaveChanges();
            return RedirectToAction("Index");
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