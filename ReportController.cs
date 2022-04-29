using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESP.Models;
using ESS.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace ESS.Controllers
{
    public class ReportController : Controller
    {

        private ApplicationDbContext _context;

        public ReportController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HostelReport()
        {
            string sHostel =  " SELECT [ID],[Hostel_Name],[Company],[Total_Rooms],[Persons_Capacity],[Asset_Capacity],[Occupied_Rooms],[Occupied_Assets],[Hostel_Location] " +
                                " ,[Entry_Datetime],[Edit_Datetime],[Occupied_Persons] FROM[HostelManagement].[dbo].[Hostel_Capacity]";
            var HostelDt = _context.Hostel_Capacity.SqlQuery(sHostel).ToList();
            if (HostelDt != null)
            {
                ViewBag.Hosteldata = HostelDt;
            }

            //string sRoomQuery = "SELECT [ID],[Room_id],[Hostel_id],[Person_Capacity],[Asset_Capacity],[Status],[Consumed_Space],[Free_Space],[Entry_Datetime],[Edit_Datetime] FROM [HostelManagement].[dbo].[Hostel_Rooms]";
            //var RoomDt = _context.Hostel_Rooms.SqlQuery(sRoomQuery).ToList();
            //if (RoomDt != null)
            //{
            //    ViewBag.RoomData = RoomDt;
            //}

            //string Status = " Select distinct(Living_Status) as LivStatus FROM [HostelManagement].[dbo].[Hostel_Persons]";
            //var StatusDt = _context.Database.SqlQuery<string>(Status).ToList();
            //if (StatusDt != null)
            //{
            //    ViewBag.LivingStatus = StatusDt;
            //}

            return View();
        }

        public ActionResult HostelEmployeesRpt(Hostel_Persons hostel_Persons, FormCollection form)
        {
            string myWhere = "1=1 and";

            var CreationDate = form["Creation_Date"];

            if (hostel_Persons.Hostel_id != null)
            {
                myWhere = myWhere + " Hostel_id = '" + hostel_Persons.Hostel_id + "' and";
                var HostelName = _context.Database.SqlQuery<string>("Select Hostel_Name From [HostelManagement].[dbo].[Hostel_Capacity] where ID = '" + hostel_Persons.Hostel_id + "'").First();
                Session["HostelName"] = HostelName;

            }

            if (hostel_Persons.Room_id != null && hostel_Persons.Room_id != "-1")
            {
                myWhere = myWhere + " Room_id    = '" + hostel_Persons.Room_id + "' and";
                Session["Room"] = hostel_Persons.Room_id;
            }

            if (CreationDate != "")
            {
                string dd = CreationDate.ToString().Substring(8, 2);
                string mm = CreationDate.ToString().Substring(5, 2);
                string yyyy = CreationDate.ToString().Substring(0, 4);

                string date = yyyy + mm + dd;

                myWhere = myWhere + " a.Entry_Datetime = '" + date + "' and";

                Session["EntryDate"] = dd + "-" + mm + "-" + yyyy;
            }

            if (hostel_Persons.Living_Status != null)
            {
                myWhere = myWhere + " Living_Status = '" + hostel_Persons.Living_Status + "' and";
                Session["Status"] = hostel_Persons.Living_Status;
            }


            myWhere = myWhere.Substring(0, (myWhere.Length) - 4);

            string sQuery = " SELECT a.ID,[Employee_id],Employee_name,a.Company,[Department],[Designation],b.hostel_name as Hostel_id,[Room_id],[Living_Status] " +
                            " ,[Left_Datetime],a.Entry_Datetime,a.Edit_datetime FROM [HostelManagement].[dbo].[Hostel_Persons] a " +
                            " join [HostelManagement].[dbo].[Hostel_Capacity] b on a.hostel_id = b.id where " + myWhere + "";


            var Result  = _context.Hostel_persons.SqlQuery(sQuery).ToList();
            Session["ReportSource"] = Result;
            TempData["DisplayMessage"] = "Display";
            return RedirectToAction("HostelReport");
        }

        public ActionResult FillRooms(int id)
        {
            var list = _context.Database.SqlQuery<string>("SELECT [Room_id] FROM [HostelManagement].[dbo].[Hostel_Rooms] where Hostel_id = '" + id + "'").ToList();
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        public ActionResult OccupiedAssets()
        {
            string sHostel = " SELECT [ID],[Hostel_Name],[Company],[Total_Rooms],[Persons_Capacity],[Asset_Capacity],[Occupied_Rooms],[Occupied_Assets],[Hostel_Location] " +
                                " ,[Entry_Datetime],[Edit_Datetime],[Occupied_Persons] FROM[HostelManagement].[dbo].[Hostel_Capacity]";
            var HostelDt = _context.Hostel_Capacity.SqlQuery(sHostel).ToList();
            if (HostelDt != null)
            {
                ViewBag.Hosteldata = HostelDt;
            }

            string sRoomQuery = "SELECT [ID],[Room_id],[Hostel_id],[Person_Capacity],[Asset_Capacity],[Status],[Consumed_Space],[Free_Space],[Entry_Datetime],[Edit_Datetime] FROM [HostelManagement].[dbo].[Hostel_Rooms]";
            var RoomDt = _context.Hostel_Rooms.SqlQuery(sRoomQuery).ToList();
            if (RoomDt != null)
            {
                ViewBag.RoomData = RoomDt;
            }

            return View();
        }

        public ActionResult OccupiedAssetsRpt(Occupied_Assets occupied_Assets, FormCollection form)
        {
            string myWhere = "1=1 and";

            var CreationDate = form["Creation_Date"];

            if (occupied_Assets.Hostel_id != 0)
            {
                myWhere = myWhere + " a.Hostel_id = '" + occupied_Assets.Hostel_id + "' and";
                var HostelName = _context.Database.SqlQuery<string>("Select Hostel_Name From [HostelManagement].[dbo].[Hostel_Capacity] where ID = '" + occupied_Assets.Hostel_id + "'").First();
                Session["HostelName"] = HostelName;

            }

            if (occupied_Assets.Room_id != null && occupied_Assets.Room_id != "-1")
            {
                myWhere = myWhere + " Room_id = '" + occupied_Assets.Room_id + "' and";
                Session["Room"] = occupied_Assets.Room_id;
            }

            if (CreationDate != "")
            {
                string dd = CreationDate.ToString().Substring(8, 2);
                string mm = CreationDate.ToString().Substring(5, 2);
                string yyyy = CreationDate.ToString().Substring(0, 4);

                string date = yyyy + mm + dd;

                myWhere = myWhere + " a.Entry_Datetime = '" + date + "' and";

                Session["EntryDate"] = dd + "-" + mm + "-" + yyyy;
            }

            myWhere = myWhere.Substring(0, (myWhere.Length) - 4);

            string sQuery = " Select a.ID,a.Hostel_id,c.Hostel_Name as HostelName,Room_id,a.Asset_id,b.asset_name as Name,a.Asset_Type_id,a.Entry_datetime,isnull(a.Edit_datetime,'') Edit_datetime,a.Active,a.quantity " +
                              " from[HostelManagement].[dbo].[Occupied_Assets] a join[HostelManagement].[dbo].[AMS_Assets_Master] b on a.Asset_id = b.asset_id " +
                              " join[HostelManagement].[dbo].[Hostel_Capacity] c on a.Hostel_id = c.ID  Where  " + myWhere + "";

            var Result = _context.Occupied_Assets.SqlQuery(sQuery).ToList();
            Session["OccAssetRptSource"] = Result;
            TempData["DisplayOccAssetMessage"] = "Display";
            return RedirectToAction("OccupiedAssets");
        }
    }
}