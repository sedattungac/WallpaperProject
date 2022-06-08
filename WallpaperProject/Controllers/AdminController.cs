using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using EntityLayer.Concrete;

namespace WallpaperProject.Controllers
{
    public class AdminController : Controller
    {
        AboutManager aboutManager = new AboutManager(new EfAboutDal());
        ImageCategoryManager imageCategoryManager = new ImageCategoryManager(new EfImageCategoryDal());
        ServiceManager serviceManager = new ServiceManager(new EfServiceDal());
        ContactManager contactManager = new ContactManager(new EfContactDal());
        ImageManager imageManager = new ImageManager(new EfImageDal());
        AdminManager adminManager = new AdminManager(new EfAdminDal());
        Context c = new Context();
        public IActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var adminId = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var fullName = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var userName = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var eMail = Request.Form["columns[4][search][value]"].FirstOrDefault();

                var value = (from customer in c.Admins select customer);

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data  
                var customerData = (from tempcustomer in c.Admins
                                    select tempcustomer);

                //Search

                if (!string.IsNullOrEmpty(adminId))
                    value = value.Where(x => x.AdminId.ToString().Contains(adminId));
                if (!string.IsNullOrEmpty(fullName))
                    value = value.Where(x => x.FullName.Contains(fullName));
                if (!string.IsNullOrEmpty(userName))
                    value = value.Where(x => x.UserName.Contains(userName));
                if (!string.IsNullOrEmpty(eMail))
                    value = value.Where(x => x.Email.Contains(eMail));




                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    value = value.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                var customerCols = c.Admins.Select(x => new
                {
                    adminId = x.AdminId,
                    fullName = x.FullName,
                    userName = x.UserName,
                    eMail = x.Email,
                    password = x.Password,
                }).ToList();


                //Search  

                if (!string.IsNullOrEmpty(searchValue))
                {
                    value = value.Where(m => m.FullName.Contains(searchValue)
                                                || m.UserName.Contains(searchValue)
                                                || m.Email.Contains(searchValue)
                                                || m.AdminId.ToString().Contains(searchValue));
                }


                //total number of rows count   
                recordsTotal = customerData.Count();
                //Paging   
                var data = value.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var edit = adminManager.TGetById(id);
            return View(edit);
        }
        [HttpPost]
        public IActionResult Edit(Admin admin)
        {
            adminManager.TUpdate(admin);
            return RedirectToAction("Index");
        }
    }
}
