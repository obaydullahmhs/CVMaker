using CVStorage.Models;
using CVStorage.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CVStorage.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IPersonRepo _personRepository;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWebHostEnvironment env;

        [Obsolete]
        public UserController(IPersonRepo personRepo, IHostingEnvironment hostingEnvironment, IWebHostEnvironment env)
        {
            _personRepository = personRepo;
            _hostingEnvironment = hostingEnvironment;
            this.env = env;
        }
        public IActionResult AllUsers()
        {
            var model = _personRepository.GetAllPersons();
            return View(model);
        }

        public IActionResult Profile(string ID)
        {
            Person model = _personRepository.GetPerson(ID);
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string ID)
        {
            Person person = _personRepository.GetPerson(ID);
            PersonEditViewModel personEditViewModel = new PersonEditViewModel
            {
                ID = person.ID,
                Name = person.Name,
                Email = person.Email,
                Subject = person.Subject,
                University = person.University,
                SSC_GPA = person.SSC_GPA,
                HSC_GPA = person.HSC_GPA,
                Bachelor_CGPA = person.Bachelor_CGPA,
                Project = person.Project,
                Skills = person.Skills,
                ExistingPhotoPath = person.PhotoPath,
                Phone = person.Phone,
                IsAccepted = person.IsAccepted,

            };
            return View(personEditViewModel);
        }

        [HttpPost]
        [Obsolete]
        public IActionResult Edit(PersonEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Person person = _personRepository.GetPerson(model.ID);
                person.Name = model.Name;
                person.Email = model.Email;
                person.Subject = model.Subject;
                person.University = model.University;
                person.SSC_GPA = model.SSC_GPA;
                person.HSC_GPA = model.HSC_GPA;
                person.Bachelor_CGPA = model.Bachelor_CGPA;
                person.Phone = model.Phone;
                person.Project = model.Project;
                person.Skills = model.Skills;
                person.IsAccepted = model.IsAccepted;

                if (model.PhotoPath != null)
                {
                    //if (model.ExistingPhotoPath != null)
                    //{
                    //    string filePath = Path.Combine(IHostingEnvironment.WebRootPath,
                    //        "images", model.ExistingPhotoPath);
                    //    System.IO.File.Delete(filePath);
                    //}
                    person.PhotoPath = ProcessUploadedFile(model);
                }

                Person updatedPerson = _personRepository.Update(person);

                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View(model);
        }

        public IActionResult DownloadCV(string ID)
        {
            Person person = _personRepository.GetPerson(ID);
            HtmlToPdf htmlpdf = new HtmlToPdf();

            string path = string.Empty;
            path = System.IO.File.ReadAllText(env.WebRootPath + @"\pdfhtmlFile.html");

            path = path.Replace("myName", person.Name)
                .Replace("myEmail", person.Email)
                .Replace("myPhone", person.Phone)
                .Replace("myUniversity", person.University)
                .Replace("mySubject", person.Subject)
                .Replace("myCGPA", person.Bachelor_CGPA.ToString())
                .Replace("mySSC", person.SSC_GPA.ToString())
                .Replace("myHSC", person.HSC_GPA.ToString())
                .Replace("myProject", person.Project)
                .Replace("mySkill", person.Skills)
                .Replace("myPhoto", env.WebRootPath + @"/images/"+ (person.PhotoPath ?? "dummy.png"))
                .Replace("amiBUP", env.WebRootPath + @"/images/bup-logo.png");


            PdfDocument doc = htmlpdf.ConvertHtmlString(path);
            var bytes = doc.Save();
            return File(bytes, "application/pdf", person.Name + ".pdf");

        }

        [Obsolete]
        private string ProcessUploadedFile(PersonEditViewModel model)
        {
            string uniqueFileName = null;

            if (model.PhotoPath != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoPath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.PhotoPath.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

    }
}
