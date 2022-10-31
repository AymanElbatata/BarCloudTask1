using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WatchData()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveAllData(List<FirstTable> mydata)
        {
            int counter = 0;
            using (BarCloud1DBEntities mymodel = new BarCloud1DBEntities())
            {
                foreach (var item in mydata)
                {
                    var exist = mymodel.FirstTable.Where(a => a.Asset.ToLower() == item.Asset.ToLower() &&
                    a.Model.ToLower() == item.Model.ToLower() && a.Vendor.ToLower() == item.Vendor.ToLower() ).Any();
                    if (!exist)
                    {
                        mymodel.FirstTable.Add(item);
                        counter++;
                    }
                    mymodel.SaveChanges();
                }
            }
            return Json(new { Message = counter, JsonRequestBehavior.AllowGet });
        }



        public ActionResult JtableData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult READSpreadSheet(string txtENBTNSearch = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                using (BarCloud1DBEntities mydb = new BarCloud1DBEntities())
                {
                    if (txtENBTNSearch != null && txtENBTNSearch != "" && !string.IsNullOrEmpty(txtENBTNSearch))
                    {
                        var lst = mydb.FirstTable.Where(a => a.Asset.Contains(txtENBTNSearch.Trim()) || a.Vendor.Contains(txtENBTNSearch.Trim()) ||
                        a.Model.Contains(txtENBTNSearch.Trim()) || a.Description.Contains(txtENBTNSearch.Trim())).ToList();
                        return Json(new { Result = "OK", Records = lst.Skip(jtStartIndex).Take(jtPageSize), TotalRecordCount = lst.Count() });
                    }
                    else
                    {
                        var lst = mydb.FirstTable.ToList();
                        return Json(new { Result = "OK", Records = lst.Skip(jtStartIndex).Take(jtPageSize), TotalRecordCount = lst.Count() });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex });
            }
        }

        [HttpPost]
        public ActionResult InsertSpreadSheet(string Asset = null, string Model = null, string Vendor = null, string Description = null)
        {
            try
            {
                using (BarCloud1DBEntities mydb = new BarCloud1DBEntities())
                {

                    if (ModelState.IsValid)
                    {
                        //
                        var reqexist = mydb.FirstTable.Where(a => a.Asset == Asset && a.Model == Model &&
                        a.Vendor == Vendor).Count();
                        if (reqexist == 0)
                        {
                            FirstTable model = new FirstTable();
                            model.Vendor = Vendor;
                            model.Asset = Asset;
                            model.Model = Model;
                            model.Description = Description;

                            var data = mydb.FirstTable.Add(model);
                            mydb.SaveChanges();
                            return Json(new { Result = "OK", Record = data });
                        }
                        else
                        {
                            return Json(new { Result = "ERROR", Message = "There is a record with the same name" });
                        }
                    }
                    else
                    {
                        return Json(new { Result = "ERROR", Message = "Sorry, there is a missing in the DB" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex });

            }
        }

        [HttpPost]
        public ActionResult UpdateSpreadSheet(int id, string Asset = null, string Model = null, string Vendor = null, string Description = null)
        {
            try
            {
                using (BarCloud1DBEntities mydb = new BarCloud1DBEntities())
                {
                    if (ModelState.IsValid)
                    {
                        //check if  exist
                        //
                        //
                        var reqexist = mydb.FirstTable.Where(a => a.id != id && a.Asset == Asset && a.Model == Model &&
                        a.Vendor == Vendor).Count();

                        if (reqexist == 0)
                        {
                            var data = mydb.FirstTable.Find(id);
                            data.Vendor = Vendor;
                            data.Model = Model;
                            data.Asset = Asset;
                            data.Description = Description;
                            mydb.Entry(data).State = EntityState.Modified;
                            mydb.SaveChanges();
                            return Json(new { Result = "OK", Record = data });
                        }
                        else
                        {
                            return Json(new { Result = "ERROR", Message = "There is a record with the same name" });
                        }
                    }
                    else
                    {
                        return Json(new { Result = "ERROR", Message = "Sorry, there is a missing in the DB" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex });
            }
        }

        [HttpPost]
        public ActionResult DeleteSpreadSheet(int id)
        {
            try
            {
                using (BarCloud1DBEntities mydb = new BarCloud1DBEntities())
                {
                    mydb.Configuration.ProxyCreationEnabled = false;
                    var newobject = mydb.FirstTable.Find(id);
                    // Send row to be deleted
                    if (newobject != null)
                    {
                        mydb.FirstTable.Remove(newobject);
                        mydb.SaveChanges();
                        return Json(new { Result = "OK" });
                    }
                    else
                    {
                        return Json(new { Result = "ERROR", Message = "Sorry, there is a missing in the DB" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex });
            }
        }




    }
}