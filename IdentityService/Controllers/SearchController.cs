using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Dapper;
using IdentityService.Core.Domain;
using IdentityService.Models;
using IdentityService.Services;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;

namespace IdentityService.Controllers
{
    public class SearchController : Controller
    {
        private readonly IRepositoryWithTypedId<People, string> _peopleRepository;
        private readonly IDbService _dbService;
        
        public System.Linq.Expressions.Expression<Func<People, bool>> PeopleSearchExpression { get; set; }

        public SearchController(IRepositoryWithTypedId<People, string> peopleRepository, IDbService dbService)
        {
            _peopleRepository = peopleRepository;
            _dbService = dbService;
        }
        //
        // GET: /Search/

        public ActionResult Index()
        {
            var model = new IdentitySearchQueryOptions();
            return View(model);
        }

        //
        // GET: /Search/Details/5
        //public ActionResult Details(int id)
        public ActionResult Details(IdentitySearchQueryOptions queryOptions)
        {
            IEnumerable<People> people;

            //Console.SetOut(new DebugTextWriter());
            if (queryOptions == null)
            {
                return RedirectToAction("Index");
            }

            using (var conn = _dbService.GetConnection())
            {
                people = conn.Query<People>("EXEC usp_IAMSearch @SearchString = @searchString, " +
                                             "@SearchMethod = @searchMethod, " +
                                             "@NumItemsPerPage = @numItemsPerPage, " +
                                             "@PageNumber = @pageNumber",
                    new
                    {
                        searchString = (string)queryOptions.SearchString,
                        searchMethod = (string)(String.IsNullOrEmpty(queryOptions.SearchMethod) ? "Search" : queryOptions.SearchMethod),
                        numItemsPerPage = (int)(queryOptions.NumItemsPerPage == 0 ? 5 : queryOptions.NumItemsPerPage),
                        pageNumber = (int)(queryOptions.PageNumber == 0 ? 1 : queryOptions.PageNumber)
                    }
                    ).AsQueryable();
            }

            if (!people.Any())
            {
                return RedirectToAction("Index");
            }

            return View(people);
        }

        //
        // GET: /Search/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Search/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Search/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Search/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Search/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Search/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
