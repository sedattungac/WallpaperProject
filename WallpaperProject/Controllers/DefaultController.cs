using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WallpaperProject.Controllers
{
    public class DefaultController : Controller
    {
        AboutManager aboutManager = new AboutManager(new EfAboutDal());
        ImageCategoryManager imageCategoryManager = new ImageCategoryManager(new EfImageCategoryDal());
        ServiceManager serviceManager = new ServiceManager(new EfServiceDal());
        ContactManager contactManager = new ContactManager(new EfContactDal());
        ImageManager imageManager = new ImageManager(new EfImageDal());
        Context c = new Context();
        public IActionResult Index()
        {
            var imageCategories = imageCategoryManager.TGetList();
            return View(imageCategories);
        }
        public IActionResult Images(int id)
        {
            var header = c.ImageCategories.Where(x => x.ImageCategoryId == id).Select(x => x.Title).FirstOrDefault();
            ViewBag.header = header;
            var images = c.Images.Include(x => x.ImageCategory).Where(x => x.ImageCategory.ImageCategoryId == x.ImageCategoryId).ToList();
            return View(images);
        }
        public IActionResult Services()
        {
            var services = serviceManager.TGetList();
            return View(services);
        }
        public IActionResult About()
        {
            var about = aboutManager.TGetList();
            return View(about);
        }
        [HttpGet]
        public IActionResult Contact()
        {

            return View();
        }
        [HttpPost]
        public IActionResult SendMessage(Contact contact)
        {
            contactManager.TAdd(contact);
            return RedirectToAction("Contact");
        }
    }
}
