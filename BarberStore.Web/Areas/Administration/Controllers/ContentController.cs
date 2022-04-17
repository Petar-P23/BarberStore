using BarberStore.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class ContentController : BaseController
    {
        private readonly IHostEnvironment environment;
        private readonly IStoreService storeService;
        private readonly IServicesService servicesService;
        private readonly IAnnouncementService announcementService;

        public ContentController(IHostEnvironment environment,
            IStoreService storeService,
            IServicesService servicesService,
            IAnnouncementService announcementService)
        {
            this.environment = environment;
            this.storeService = storeService;
            this.servicesService = servicesService;
            this.announcementService = announcementService;
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
            if (await this.storeService.CreateNewProductAsync(name, image.FileName, price, description))
            {
                await SaveImage(image);
            }

            return RedirectToAction("ManageProducts");
        }

        [HttpPost]
        public async Task<IActionResult> AddService(string name, string description, decimal price)
        {
            await this.servicesService.CreateServiceAsync(name, description, price);

            return RedirectToAction("ManageServices");
        }

        [HttpPost]
        public async Task<IActionResult> AddAnnouncement(string mainText)
        {
            await this.announcementService.CreateAnnouncementAsync(mainText);

            return RedirectToAction("ManageAnnouncements");
        }
        [HttpGet]
        public async Task<IActionResult> RemoveAnnouncement(string id, string returnUrl)
        {
            await this.announcementService.RemoveAnnouncementAsync(id);

            return Redirect(returnUrl);
        }
        [HttpGet]
        public async Task<IActionResult> RemoveService(string id, string returnUrl)
        {
            await this.servicesService.RemoveServiceAsync(id);

            return Redirect(returnUrl);
        }
        [HttpGet]
        public async Task<IActionResult> RemoveProduct(string id, string returnUrl)
        {
            await this.storeService.RemoveProductAsync(id);

            return Redirect(returnUrl);
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
