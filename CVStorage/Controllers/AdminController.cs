using CVStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVStorage.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly IPersonRepo _personRepo;

        public AdminController(IPersonRepo personRepo)
        {
            this._personRepo = personRepo;
        }

        public IActionResult Pending()
        {
            var model = _personRepo.GetAllPersons();
            return View(model);
        }

        public IActionResult AcceptedUsers()
        {
            var model = _personRepo.GetAllPersons();
            return View(model);
        }

        public IActionResult AcceptUser(string ID)
        {
            var model = _personRepo.GetPerson(ID);
            model.IsAccepted = true;
            var person = _personRepo.Update(model);
            return RedirectToAction("Pending", "Admin", new { area = "" });
        }
    }
}
