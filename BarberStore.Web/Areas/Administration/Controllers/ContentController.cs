﻿using Microsoft.AspNetCore.Mvc;

namespace BarberStore.Web.Areas.Administration.Controllers
{
    public class ContentController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}