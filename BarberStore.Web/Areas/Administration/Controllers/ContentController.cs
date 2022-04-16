using System.Net.Mime;
using BarberStore.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class ContentController : BaseController
    {
        private readonly IHostEnvironment environment;
        private readonly IStoreService storeService;

        public ContentController(IHostEnvironment environment,
            IStoreService storeService)
        {
            this.environment = environment;
            this.storeService = storeService;
        }

        public IActionResult ManageProducts()
        {
            return this.View();
        }
        public IActionResult ManageAnnouncements()
        {
            return this.View();
        }

        public IActionResult ManageServices()
        {
            return this.View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(string name, string description, decimal price, IFormFile image)
        {
            if (await this.storeService.CreateNewProduct(name, image.FileName, price, description))
            {
                await SaveImage(image);
            }

            return RedirectToAction("ManageProducts");
        }

        private async Task<bool> SaveImage(IFormFile image)
        {
            try
            {
                string path = Path.Combine(this.environment.ContentRootPath, "wwwroot\\img");
                string fileName = Path.GetFileName(image.FileName);
                await using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                await image.CopyToAsync(stream);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
