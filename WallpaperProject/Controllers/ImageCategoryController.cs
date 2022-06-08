using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace WallpaperProject.Controllers
{
    public class ImageCategoryController : Controller
    {
        ImageCategoryManager imageCategoryManager = new ImageCategoryManager(new EfImageCategoryDal());
        Context c = new Context();
        public IActionResult Index()
        {
            return View();
        }
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
                var categoryId = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var title = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var imageUrl = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var status = Request.Form["columns[4][search][value]"].FirstOrDefault();

                var value = (from customer in c.ImageCategories select customer);

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data  
                var customerData = (from tempcustomer in c.Admins
                                    select tempcustomer);

                //Search

                if (!string.IsNullOrEmpty(categoryId))
                    value = value.Where(x => x.ImageCategoryId.ToString().Contains(categoryId));
                if (!string.IsNullOrEmpty(title))
                    value = value.Where(x => x.Title.Contains(title));
                if (!string.IsNullOrEmpty(imageUrl))
                    value = value.Where(x => x.ImageUrl.Contains(imageUrl));
                if (!string.IsNullOrEmpty(status))
                    value = value.Where(x => x.Status.ToString().Contains(status));




                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    value = value.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                var customerCols = c.ImageCategories.Select(x => new
                {
                    imagesCategoryId = x.ImageCategoryId,
                    title = x.Title,
                    imageUrl = x.ImageUrl,
                    status = x.Status,
                }).ToList();


                //Search  

                if (!string.IsNullOrEmpty(title))
                {
                    value = value.Where(m => m.Title.Contains(title));
                }


                //total number of rows count   
                recordsTotal = customerData.Count();
                //Paging   
                var data = value.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var edit = imageCategoryManager.TGetById(id);
            return View(edit);
        }
        [HttpPost]
        public IActionResult Edit(ImageCategory imageCategory)
        {
            imageCategoryManager.TUpdate(imageCategory);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Add()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Add(ImageCategory imageCategory)
        {
            imageCategoryManager.TAdd(imageCategory);
            return RedirectToAction("Index");
        }
    }
}
