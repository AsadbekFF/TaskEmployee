using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TaskEmployee.Models;
using System.Collections;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TaskEmployee.Controllers
{
    public class HomeController : Controller
    {
        private readonly static ApplicationDbContext dbContext = 
            new(new DbContextOptions<ApplicationDbContext>());

        public IActionResult Index()
        {
            ViewData["Employees"] = dbContext.Employees.ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost("FileUpload")]
        public IActionResult Index(List<IFormFile> files)
        {
            int failed = 0;
            int successfull = 0;

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    Upload(ref failed, ref successfull, formFile);
                }
            }
            ViewData["Failed"] = failed.ToString();
            ViewData["Successfull"] = successfull.ToString();
            ViewData["Employees"] = dbContext.Employees.ToList();
            ViewData["Count"] = "True";
            return View("Index");
        }

        private void Upload(ref int failed, ref int successfull, IFormFile formFile)
        {
            using (var stream = formFile.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    try
                    {
                        string line = reader.ReadLine();
                        ChangeName(ref line);
                        var values = line.Split(',');
                        Employee employee = new()
                        {
                            PayrollNumbers = values[0],
                            Name = values[1],
                            DateOfBirth = values[2],
                            TelephoneNumber = int.Parse(values[3]),
                            Mobile = int.Parse(values[4]),
                            Address = values[5],
                            Address2 = values[6],
                            PostCode = values[7],
                            Email = values[8],
                            StartDate = values[9]
                        };

                        dbContext.Employees.Add(employee);
                        dbContext.SaveChanges();
                        successfull += 1;
                    }
                    catch (Exception)
                    {
                        failed += 1;
                    }
                }
            }
        }

        [Route("Sort")]
        public IActionResult Sort(string sortBy)
        {
            List<Employee> employees = dbContext.Employees.ToList();
            switch (sortBy)
            {
                case "PayrollNumbers":
                    employees = (from item in employees
                                orderby item.PayrollNumbers ascending
                                select item).ToList();
                    break;
                case "Name":
                    employees = (from item in employees
                     orderby item.Name ascending
                     select item).ToList();
                    break;
                case "DateOfBirth":
                    employees = (from item in employees
                     orderby item.DateOfBirth ascending
                     select item).ToList();
                    break;
                case "Telephone":
                    employees = (from item in employees
                     orderby item.TelephoneNumber ascending
                     select item).ToList();
                    break;
                case "Mobile":
                    employees = (from item in employees
                     orderby item.Mobile ascending
                     select item).ToList();
                    break;
                case "Address":
                    employees = (from item in employees
                     orderby item.Address ascending
                     select item).ToList();
                    break;
                case "Address2":
                    employees = (from item in employees
                     orderby item.Address2 ascending
                     select item).ToList();
                    break;
                case "PostCode":
                    employees = (from item in employees
                     orderby item.PostCode ascending
                     select item).ToList();
                    break;
                case "Email":
                    employees = (from item in employees
                     orderby item.Email ascending
                     select item).ToList();
                    break;
                case "StartDate":
                    employees = (from item in employees
                     orderby item.StartDate ascending
                     select item).ToList();
                    break;

            }

            ViewData["Employees"] = employees;
            return View("Index");
        }

        public IActionResult Search(string CustomerName)
        {
            CustomerName = CustomerName.ToUpperInvariant();
            List<Employee> employees = new();
            dbContext.Employees.ForEachAsync(x =>
            {
                if (x.PayrollNumbers.ToUpperInvariant() == CustomerName)
                    employees.Add(x);
                if (x.Name.ToUpperInvariant() == CustomerName)
                    employees.Add(x);
                if (x.DateOfBirth.ToUpperInvariant() == CustomerName)
                    employees.Add(x);
                if (x.TelephoneNumber.ToString() == CustomerName)
                    employees.Add(x);
                if (x.Mobile.ToString() == CustomerName)
                    employees.Add(x);
                if (x.Address.ToUpperInvariant() == CustomerName)
                    employees.Add(x);
                if (x.Address2.ToUpperInvariant() == CustomerName)
                    employees.Add(x);
                if (x.Email.ToUpperInvariant() == CustomerName)
                    employees.Add(x);
                if (x.PostCode.ToUpperInvariant() == CustomerName)
                    employees.Add(x);
                if (x.StartDate.ToUpperInvariant() == CustomerName)
                    employees.Add(x);
            });

            ViewData["Employees"] = employees;
            return View("Index");
        }

        public JsonResult AutoComplete(string prefix)
        {
            HashSet<string> auto = new();
            prefix = prefix.ToUpperInvariant();

            Task.WaitAll(dbContext.Employees.ForEachAsync(x =>
            {
                if (x.PayrollNumbers.ToUpperInvariant().StartsWith(prefix))
                    auto.Add(x.PayrollNumbers);
                if (x.Name.ToUpperInvariant().StartsWith(prefix))
                    auto.Add(x.Name);
                if (x.DateOfBirth.ToUpperInvariant().StartsWith(prefix))
                    auto.Add(x.DateOfBirth);
                if (x.TelephoneNumber.ToString().StartsWith(prefix))
                    auto.Add(x.TelephoneNumber.ToString());
                if (x.Mobile.ToString().StartsWith(prefix))
                    auto.Add(x.Mobile.ToString());
                if (x.Address.ToUpperInvariant().StartsWith(prefix))
                    auto.Add(x.Address);
                if (x.Address2.ToUpperInvariant().StartsWith(prefix))
                    auto.Add(x.Address2);
                if (x.Email.ToUpperInvariant().StartsWith(prefix))
                    auto.Add(x.Email);
                if (x.PostCode.ToUpperInvariant().StartsWith(prefix))
                    auto.Add(x.PostCode);
                if (x.StartDate.ToUpperInvariant().StartsWith(prefix))
                    auto.Add(x.StartDate);
            }));

            return Json(auto);
        }

        private static void ChangeName(ref string line)
        {
            int count = 0;
            for(int i = 0; i < line.Length; i++)
            {
                if (line[i] == ',')
                {
                    count++;
                    if (count == 2)
                    {
                        line = line.Remove(i, 1);
                    }
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
