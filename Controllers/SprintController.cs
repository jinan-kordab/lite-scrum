using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Database;
using WebApplication4.Filters;
using WebApplication4.Helpers;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class SprintController : Controller
    {
        public Encryption encNewTeamMember;
        // GET: Sprint
        public ActionResult Index()
        {
            var epicId = Session["EpicID"] != null ? Session["EpicID"] : "0";
            //var epicId = "1";
            encNewTeamMember = new Encryption();
            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {
                var epicid = Convert.ToInt64(epicId);
                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

                var items = from itm in db.PRODUCTBACKLOGs.AsEnumerable()
                            where itm.FKEPICID == epicid && itm.ITEMLOCATION == "SPRBG"
                            select new ProductBacklog
                            {
                                ITEMID = itm.ITEMID,
                                ITEMTITLE = itm.ITEMTITLE,
                                CREATIONDATE = itm.CREATIONDATE,
                                ITEMNOTES = itm.ITEMNOTES.Replace("<br>", "\r\n"),
                                ITEMLOCATION = itm.ITEMLOCATION,
                                IndividualItems = (from indItem in db.ITEMs.AsEnumerable() where indItem.FKPRODUCTBACKLOGID == itm.ITEMID select new Item
                                {
                                     ITEMID = indItem.ITEMID,
                                  ITEMTITLE = indItem.ITEMTITLE,
                                  CREATIONDATE = indItem.CREATIONDATE,
                                  ITEMNOTES = indItem.ITEMNOTES,
                                  PersonImagePath = GetAndSaveClientPicture(indItem.FKPERSONID),
                                  FullName = GetPersonsFullName(indItem.FKPERSONID),
                                    ITEMLOCATION = indItem.ITEMLOCATION
                                }).ToList()
                            };

               

                ProductBacklog item = new ProductBacklog();
                item.Itemlist = items.ToList();

                int cCIndividualItemsCount = 0;
                foreach (var it in item.Itemlist)
                {
                    int cCount = it.IndividualItems.Count;
                    if (cCount > cCIndividualItemsCount)
                        cCIndividualItemsCount = cCount;
                }

                item.cIndividualItemsColumnCount = cCIndividualItemsCount;
                item.PersonImagePath = GetAndSavePersonsPicture(Convert.ToInt64(userid));
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

                item.LoggedInUser = Session["UserId"].ToString();


                //Get chart data

                //1-Get all clients for current user
                var currentUserClients = (from c in db.CLIENTs where c.FKPERSONID == userid select c.CLIENTID).ToList();

                item.ChartSprintList = (from sl in db.SPRINTARCHIVEs where currentUserClients.Contains(sl.FKCLIENTID) select sl.SPRINTARCHIVEID).ToList();
                if (item.ChartSprintList != null && item.ChartSprintList.Count != 0)
                {
                    item.ChartTasksDoneList = new List<int>();

                    for (int i = 0; i < item.ChartSprintList.Count; i++)
                    {
                        string sprintId = item.ChartSprintList[i].ToString();
                        int tasksDone = GetArchivedSprintDoneItemsCount(sprintId);
                        item.ChartTasksDoneList.Add(tasksDone);
                    }

                    //get VELOCITY
                    item.ChartVelocityList = new List<double>();

                    for (int v = 0; v < item.ChartSprintList.Count; v++)
                    {
                        int vcount = v + 1;
                        item.ChartVelocityList.Add(v == 0 ? (item.ChartTasksDoneList[v] / vcount) : double.Parse(((double)(item.ChartTasksDoneList[v] + item.ChartTasksDoneList[v - 1]) / (double)vcount).ToString()));
                    }

                    foreach (var scsl in item.ChartSprintList)
                    {
                        item.SChartSprintList += scsl.ToString() + ",";
                    }
                    foreach (var ctdl in item.ChartTasksDoneList)
                    {
                        item.SChartTasksDoneList += ctdl.ToString() + ",";
                    }

                    foreach (var cvl in item.ChartVelocityList)
                    {
                        item.SChartVelocityList += cvl.ToString() + ",";
                    }


                    item.SChartSprintList = item.SChartSprintList.Substring(0, item.SChartSprintList.Length - 1);
                    item.SChartTasksDoneList = item.SChartTasksDoneList.Substring(0, item.SChartTasksDoneList.Length - 1);
                    item.SChartVelocityList = item.SChartVelocityList.Substring(0, item.SChartVelocityList.Length - 1);
                }
                else
                {
                    item.SChartSprintList = "0";
                    item.SChartTasksDoneList = "0";
                    item.SChartVelocityList = "0";
                }


                //End Get chart data
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

        public ActionResult Archived()
        {
            SCRUMEntities db = new SCRUMEntities();
            ProductBacklog item = new ProductBacklog();
            encNewTeamMember = new Encryption();

            var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));

        
            item.PersonImagePath = GetAndSavePersonsPicture(Convert.ToInt64(userid));
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

            item.LoggedInUser = Session["UserId"].ToString();


            if (Session["LANG"] != null)
            {
                if (Session["LANG"].ToString() == "fr")
                {
                    return View("fr/Archived", item);
                }
                else
                {
                    return View("Archived", item);
                }
            }
            else
            {
                return View("Archived", item);
            }

        }

        [AllowAnonymous]
        public JsonResult GetArchivedSprints(JqueryDatatableParam param)
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

                List<ArchiveSprint> archSprint = new List<ArchiveSprint>();

                
                var usersClients = (from itm in db.CLIENTs.AsEnumerable()
                            where itm.FKPERSONID == userid
                            select new Client{CLIENTID = itm.CLIENTID}).ToList();

                List<long> usersClientsArray = new List<long>();
        
               
                foreach(var clnt in usersClients)
                {
                    usersClientsArray.Add(clnt.CLIENTID);

                }

                //for(int cc = 0; cc <= usersClients.Count -1; cc++)
                //{
                    
                //}


                archSprint = (from asp in db.SPRINTARCHIVEs.AsEnumerable()
                              where  usersClientsArray.Contains(asp.FKCLIENTID)
                                select new ArchiveSprint {
                                                  SPRINTARCHIVEID = asp.SPRINTARCHIVEID,
                                                  SPRINTARCHIVESTARTDATE = asp.SPRINTARCHIVESTARTDATE.ToString(),
                                                  SPRINTARCHIVEENDDATE = asp.SPRINTARCHIVEENDDATE.ToString()
                                                }).ToList();

                IEnumerable<ArchiveSprint> people = archSprint.AsEnumerable();

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    people = people.Where(x => x.SPRINTARCHIVEID.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                  || x.SPRINTARCHIVESTARTDATE.ToString().ToLower().Contains(param.sSearch.ToLower())
                                                  || x.SPRINTARCHIVEENDDATE.ToString().ToLower().Contains(param.sSearch.ToLower()));

                }

                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                var sortDirection = HttpContext.Request.QueryString["sSortDir_0"];
                if (sortColumnIndex == 1)
                {
                    people = sortDirection == "asc" ? people.OrderBy(c => c.SPRINTARCHIVESTARTDATE) : people.OrderByDescending(c => c.SPRINTARCHIVESTARTDATE);
                }
                else if (sortColumnIndex == 2)
                {
                    people = sortDirection == "asc" ? people.OrderBy(c => c.SPRINTARCHIVEENDDATE) : people.OrderByDescending(c => c.SPRINTARCHIVEENDDATE);
                }
                else if (sortColumnIndex == 3)
                {
                    people = sortDirection == "asc" ? people.OrderBy(c => c.SPRINTARCHIVEID) : people.OrderByDescending(c => c.SPRINTARCHIVEID);
                }
                else
                {
                    Func<ArchiveSprint, string> orderingFunction = e => sortColumnIndex == 0 ? e.SPRINTARCHIVESTARTDATE.ToString() : sortColumnIndex == 1 ? e.SPRINTARCHIVEENDDATE.ToString() : e.SPRINTARCHIVEID.ToString();
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

            ArchiveSprint pf = new ArchiveSprint();
                        pf.SPRINTARCHIVESTARTDATE = DateTime.Now.ToString();
                        pf.SPRINTARCHIVEENDDATE = DateTime.Now.ToString();
           
            return Json(new
            {
                param.sEcho,
                iTotalRecords = 0,
                iTotalDisplayRecords = 0,
                aaData = pf
            }, JsonRequestBehavior.AllowGet);


        }


        public ActionResult ArchiveSprint(ArchiveSprint archiveSprint)
        {
            var epicId = Session["EpicID"] != null ? Session["EpicID"] : "0";
            var sprintArchiveStartDate = Convert.ToDateTime(archiveSprint.SPRINTARCHIVESTARTDATE);
            var sprintArchiveEndDAte = Convert.ToDateTime(archiveSprint.SPRINTARCHIVEENDDATE);


            encNewTeamMember = new Encryption();
            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {
                var epicid = Convert.ToInt64(epicId);
                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));


                //Get all product backlogs and their respective tasks to archive. We need to know:
                // 1-What to archive
                // 2-What items to assign to particular archived sprint

                var items = from itm in db.PRODUCTBACKLOGs.AsEnumerable()
                            where itm.FKEPICID == epicid && itm.ITEMLOCATION == "SPRBG"
                            select new ProductBacklog
                            {
                                ITEMID = itm.ITEMID,
                                FKEPICID = itm.FKEPICID,
                                IndividualItems = (from indItem in db.ITEMs.AsEnumerable()
                                                   where indItem.FKPRODUCTBACKLOGID == itm.ITEMID
                                                   select new Item
                                                   {
                                                       ITEMID = indItem.ITEMID,
                                                   }).ToList()
                            };


                //1- Create new Archived Sprint
                SPRINTARCHIVE sprintArchive = new SPRINTARCHIVE();
                sprintArchive.SPRINTARCHIVESTARTDATE = sprintArchiveStartDate;
                sprintArchive.SPRINTARCHIVEENDDATE = sprintArchiveEndDAte;
                sprintArchive.FKEPICID = epicid;
                var clientId = (from c in db.EPICs where c.EPICID == epicid select new Epic { FKCLIENTID = c.FKCLIENTID }).SingleOrDefault();
                sprintArchive.FKCLIENTID = clientId.FKCLIENTID;
                db.SPRINTARCHIVEs.Add(sprintArchive);

                var newlySavedSprintArchiveID = sprintArchive.SPRINTARCHIVEID;

                //2-Create Sprint Archive Product Backlog
                foreach(var item in items)
                {
                    //Add Product Backlogs to archive
                    SPRINTARCHIVEBACKLOG sprintArchiveBacklog = new SPRINTARCHIVEBACKLOG();
                    sprintArchiveBacklog.FKSPRINTARCHIVE = newlySavedSprintArchiveID;
                    sprintArchiveBacklog.FKBACKLOGID = item.ITEMID;
                    sprintArchiveBacklog.FKBACKLOGEPICID = item.FKEPICID;
                    db.SPRINTARCHIVEBACKLOGs.Add(sprintArchiveBacklog);



                    //Update each product backlog and each task with ITEMLOCATION = ARCH value so that they do not appear in Home Scrum Individual Board
                    var pbacklogtoupdate = (from pb in db.PRODUCTBACKLOGs where pb.ITEMID == item.ITEMID select pb).SingleOrDefault();
                    pbacklogtoupdate.ITEMLOCATION = "ARCH";


                    foreach(var itemTask in item.IndividualItems)
                    {
                        var indTask = (from t in db.ITEMs where t.ITEMID == itemTask.ITEMID select t).SingleOrDefault();
                        indTask.ITEMLOCATION = (indTask.ITEMLOCATION.ToString() == "INPROG" || indTask.ITEMLOCATION.ToString() == "INPROGQA") ? "ARCHINPROG" : (indTask.ITEMLOCATION.ToString() == "TODO" || indTask.ITEMLOCATION.ToString() == "TODOQA") ? "ARCHTODO" : (indTask.ITEMLOCATION.ToString() == "QA" || indTask.ITEMLOCATION.ToString() == "QAQA") ? "ARCHQA" :"ARCHDONE";
                    }
                    
                }

                db.SaveChanges();

            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ViewArchivedSprintDetails(ArchiveSprint archivedSprint)
        {
            var archivedSprintId = archivedSprint.SPRINTARCHIVEID;
            Session["CurrentSprintArchiveViewId"] = archivedSprint.SPRINTARCHIVEID;

            return RedirectToAction("GetArchivedSprint", new { sprintid= archivedSprintId });
        }
        public ActionResult GetArchivedSprint(string sprintid)
        {
            var epicId = Session["EpicID"] != null ? Session["EpicID"] : "0";
            //var epicId = "1";
            encNewTeamMember = new Encryption();
            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {
                var epicid = Convert.ToInt64(epicId);
                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));






                var sprintArchiveId = Convert.ToInt64(sprintid);

                var sprintArchive = (from a in db.SPRINTARCHIVEs where a.SPRINTARCHIVEID == sprintArchiveId select a).SingleOrDefault();
                
                var archivedEpicId = (from sab in db.SPRINTARCHIVEBACKLOGs where sab.FKSPRINTARCHIVE == sprintArchiveId select sab.FKBACKLOGEPICID).FirstOrDefault();

                var archivedPBInSPrintArchiveBacklog = (from sab in db.SPRINTARCHIVEBACKLOGs where sab.FKSPRINTARCHIVE == sprintArchiveId select sab).ToList();

                List<long> spbids = new List<long>();

                foreach (var apbisab in archivedPBInSPrintArchiveBacklog)
                {
                    spbids.Add(apbisab.FKBACKLOGID);
                }

                


                var items = from itm in db.PRODUCTBACKLOGs.AsEnumerable()
                            where itm.FKEPICID == archivedEpicId && itm.ITEMLOCATION == "ARCH" && spbids.Contains(itm.ITEMID)
                            select new ProductBacklog
                            {
                                ITEMID = itm.ITEMID,
                                ITEMTITLE = itm.ITEMTITLE,
                                CREATIONDATE = itm.CREATIONDATE,
                                ITEMNOTES = itm.ITEMNOTES.Replace("<br>", "\r\n"),
                                ITEMLOCATION = itm.ITEMLOCATION,
                                IndividualItems = (from indItem in db.ITEMs.AsEnumerable()
                                                   where indItem.FKPRODUCTBACKLOGID == itm.ITEMID
                                                   select new Item
                                                   {
                                                       ITEMID = indItem.ITEMID,
                                                       ITEMTITLE = indItem.ITEMTITLE,
                                                       CREATIONDATE = indItem.CREATIONDATE,
                                                       ITEMNOTES = indItem.ITEMNOTES,
                                                       PersonImagePath = GetAndSaveClientPicture(indItem.FKPERSONID),
                                                       FullName = GetPersonsFullName(indItem.FKPERSONID),
                                                       ITEMLOCATION = indItem.ITEMLOCATION
                                                   }).ToList()
                            };



                ProductBacklog item = new ProductBacklog();
                item.Itemlist = items.ToList();
                item.SPRINTARCHIVESTARTDATE = sprintArchive.SPRINTARCHIVESTARTDATE.ToString();
                item.SPRINTARCHIVEENDDATE = sprintArchive.SPRINTARCHIVEENDDATE.ToString();
                int cCIndividualItemsCount = 0;
                foreach (var it in item.Itemlist)
                {
                    int cCount = it.IndividualItems.Count;
                    if (cCount > cCIndividualItemsCount)
                        cCIndividualItemsCount = cCount;
                }

                item.cIndividualItemsColumnCount = cCIndividualItemsCount;
                item.PersonImagePath = GetAndSavePersonsPicture(Convert.ToInt64(userid));
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

                item.LoggedInUser = Session["UserId"].ToString();


                if (Session["LANG"] != null)
                {
                    if (Session["LANG"].ToString() == "fr")
                    {
                        return View("fr/ArchivedEpicView", item);
                    }
                    else
                    {
                        return View("ArchivedEpicView", item);
                    }
                }
                else
                {
                    return View("ArchivedEpicView", item);
                }
            }

            return View("Index");
        }


        public int GetArchivedSprintDoneItemsCount(string sprintid)
        {
            int itemsDoneCount = 0;

            var epicId = Session["EpicID"] != null ? Session["EpicID"] : "0";
            //var epicId = "1";
            encNewTeamMember = new Encryption();
            if (Session["UserLoggedIn"].ToString().Trim() == "li")
            {
                var epicid = Convert.ToInt64(epicId);
                SCRUMEntities db = new SCRUMEntities();
                var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                    (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));


                var sprintArchiveId = Convert.ToInt64(sprintid);

                var sprintArchive = (from a in db.SPRINTARCHIVEs where a.SPRINTARCHIVEID == sprintArchiveId select a).SingleOrDefault();

                var archivedEpicId = (from sab in db.SPRINTARCHIVEBACKLOGs where sab.FKSPRINTARCHIVE == sprintArchiveId select sab.FKBACKLOGEPICID).FirstOrDefault();

                var archivedPBInSPrintArchiveBacklog = (from sab in db.SPRINTARCHIVEBACKLOGs where sab.FKSPRINTARCHIVE == sprintArchiveId select sab).ToList();

                List<long> spbids = new List<long>();

                foreach (var apbisab in archivedPBInSPrintArchiveBacklog)
                {
                    spbids.Add(apbisab.FKBACKLOGID);
                }


                var items = (from itm in db.PRODUCTBACKLOGs.AsEnumerable()
                             where itm.FKEPICID == archivedEpicId && itm.ITEMLOCATION == "ARCH" && spbids.Contains(itm.ITEMID)
                             select new ProductBacklog
                             {
                                 ITEMID = itm.ITEMID,
                                 ITEMTITLE = itm.ITEMTITLE,
                                 CREATIONDATE = itm.CREATIONDATE,
                                 ITEMNOTES = itm.ITEMNOTES.Replace("<br>", "\r\n"),
                                 ITEMLOCATION = itm.ITEMLOCATION,
                                 IndividualItems = (from indItem in db.ITEMs.AsEnumerable()
                                                    where indItem.FKPRODUCTBACKLOGID == itm.ITEMID
                                                    select new Item
                                                    {
                                                        ITEMID = indItem.ITEMID,
                                                        ITEMTITLE = indItem.ITEMTITLE,
                                                        CREATIONDATE = indItem.CREATIONDATE,
                                                        ITEMNOTES = indItem.ITEMNOTES,
                                                        PersonImagePath = GetAndSaveClientPicture(indItem.FKPERSONID),
                                                        FullName = GetPersonsFullName(indItem.FKPERSONID),
                                                        ITEMLOCATION = indItem.ITEMLOCATION
                                                    }).ToList()
                             }).ToList();


               

                foreach (var itmarch in items)
                {
                    foreach (var ii in itmarch.IndividualItems)
                    {
                        if (ii.ITEMLOCATION == "ARCHDONE")
                            itemsDoneCount = itemsDoneCount + 1; //itemsDoneCount ++
                    }
                }
            }

            return itemsDoneCount;
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
        public string GetPersonsFullName(long personsId)
        {
            SCRUMEntities db = new SCRUMEntities();
            var person = (from a in db.People where a.USERID == personsId select a).SingleOrDefault();

            return person.FIRSTNAME + " " + person.LASTNAME;
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
                SCRUMEntities db = new SCRUMEntities();

                IEnumerable<Person> people = (from p in db.People.AsEnumerable()
                                              select new Person
                                              {
                                                  FIRSTNAME = p.FIRSTNAME,
                                                  LASTNAME = p.LASTNAME,
                                                  EMAIL = p.EMAIL,
                                                  USERID = Server.HtmlEncode(encNewTeamMember.Encrypt_Aes_With_Custom_Key(Convert.ToString(p.USERID), (byte[])Session["ENCKEYNEWTEAMMEMBERS"], (byte[])Session["ENCIVNEWTEAMMEMBERS"]))
                                              }).ToList();

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
        

        public ActionResult GetAllPeople()
        {

            encNewTeamMember = new Encryption();
            
            var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
               (Server.UrlDecode(Session["UserId"].ToString()), (byte[])Session["userLoginKey"], (byte[])Session["userLoginIV"]));


            //var s = Session["ENCKEYNEWTEAMMEMBERS"].ToString();

            SCRUMEntities db = new SCRUMEntities();

            List<Person> people = new List<Person>();

            IEnumerable<TEAM> personsTeams = (from t in db.TEAMs.AsEnumerable() where t.FKPERSONID == userid select t).ToList();

            foreach(var personsTeam in personsTeams)
            {
                //get all users that are in those teams

                List<long> FKPERSONIDS = (from pit in db.TEAMs where pit.TEAMNAME == personsTeam.TEAMNAME select pit.FKPERSONID).ToList();

                foreach (var prsn in FKPERSONIDS)
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

                        people.Add(person);

                    
                }

                
            }

            //IEnumerable<Person> people = (from p in db.People.AsEnumerable()
            //                              select new Person
            //                              {
            //                                  FIRSTNAME = p.FIRSTNAME,
            //                                  LASTNAME = p.LASTNAME,
            //                                  EMAIL = p.EMAIL,
            //                                  USERID = p.USERID.ToString()
            //                              }).ToList();

            return Json(people, JsonRequestBehavior.AllowGet);
        }


        [Authentication]
        [HttpPost]
        public ActionResult AddNewItem(NewItem newItem)
        {
            //Get new item details
            var itemTitle = newItem.ItemTitle;
            var itemCreationDate = Convert.ToDateTime(newItem.ItemCreationDate);
            var attachmentsCount = newItem.Attachments.Count;
            var itemNotes = newItem.ItemDescription.Replace("\r\n", "<br>");
            var itemLocation = "TODO";
            var PersonId = newItem.PeopleSelect;
            var ProductBacklogId = newItem.hyujkzsaa;
            

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
            if ( newItem.Attachments[0] != null)
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



        [HttpPost]
        public RedirectToRouteResult Culture(Language lang)
        {
            var languageSelected = lang.lAction;
            Session["LANG"] = languageSelected;
            Uri MyUrl = Request.UrlReferrer;
            string redirToAction = "";

            if (MyUrl.ToString().Contains("Get"))
            {  
                ArchiveSprint aspr = new ArchiveSprint();
                aspr.SPRINTARCHIVEID = Convert.ToInt64(Session["CurrentSprintArchiveViewId"].ToString());
                return RedirectToAction("ViewArchivedSprintDetailsInternal", "Sprint", aspr );
            }
            else if (MyUrl.ToString().Contains("Archived"))
            {
                redirToAction = "Archived";
            }           
            else
            {
                redirToAction = "Index";
            }
           
            return RedirectToAction(redirToAction);
        }

        public ActionResult ViewArchivedSprintDetailsInternal(ArchiveSprint archivedSprint)
        {
            var archivedSprintId = archivedSprint.SPRINTARCHIVEID;
            Session["CurrentSprintArchiveViewId"] = archivedSprint.SPRINTARCHIVEID;

            return RedirectToAction("GetArchivedSprint", new { sprintid = archivedSprintId });
        }
    }
}