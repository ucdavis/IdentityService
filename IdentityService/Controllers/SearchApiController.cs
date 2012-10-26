using Dapper;
using IdentityService.Core.Domain;
using IdentityService.Models;
using IdentityService.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;

namespace IdentityService.Controllers
{
    public class SearchApiController : ApiController
    {
        private readonly IRepositoryWithTypedId<People, string> _peopleRepository;
        private readonly IDbService _dbService;
        
        public System.Linq.Expressions.Expression<Func<People, bool>> PeopleSearchExpression { get; set; }

        public SearchApiController()
        {
            _peopleRepository = SmartServiceLocator<IRepositoryWithTypedId<People, string>>.GetService();
            _dbService = SmartServiceLocator<IDbService>.GetService();

        }

         // This doesn't currently get called, so it has to be done in the default c'tor.
        public SearchApiController(IRepositoryWithTypedId<People, string> peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        private static bool ValidateTopQueryOption(TopQueryOption top)
        {
            if (top != null && top.RawValue != null)
            {
                int topValue = Int32.Parse(top.RawValue, NumberStyles.None);
                return topValue < 10;
            }
            return true;
        }

        // Odata support
        // POST api/SearchApi/?$orderby=<fieldname>&$skip=<#>&top=<#>&$filter=<fieldname>%20eq%20'<fieldname>'
        public IQueryable<People> Post(ODataQueryOptions queryOptions)
        {
            // Validate the top parameter
            if (!ValidateTopQueryOption(queryOptions.Top))
            {
                HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid value for $top query parameter");
                throw new HttpResponseException(response);
            }
            return queryOptions.ApplyTo(_peopleRepository.Queryable) as IQueryable<People>;
        }

        //Replaced by multi parameter query string get.  See method below this one.
        //// GET api/SearchApi/{KerberosId}
        //public HttpResponseMessage Get(string id)
        //{
        //    // Need to return a list because some peole have multiple rows because of some one-to-many relationships
        //    // such as StudentId, PPS Id, etc.
        //    IList<People> people = new List<People>();

        //    people = _peopleRepository.Queryable.Where(p => p.KerberosId.Equals(id)).OrderBy(p => p.Id).ToList();


        //    if (people.Count == 0)
        //    {
        //        return new HttpResponseMessage(HttpStatusCode.NotFound);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, people);
        //}

        // GET api/SearchApi/{KerberosId (as SearchString)}, since defaullt searchMethod is "GetBy(Kerberos)Id"
        //// GET api/SearchApi/?SearchString=searchString {&SearchMethod=searchMethod(GetById)}{,&NumItemsPerPage=numItemsPerPage(5)}{,&PageNumber=pageNumber(1)}]
        public HttpResponseMessage Get(string searchString, string searchMethod = "GetById", int numItemsPerPage = 5, int pageNumber = 1)
        {
            // Need to return a list because some peole have multiple rows because of some one-to-many relationships
            // such as StudentId, PPS Id, etc.
            IList<People> people = new List<People>();

            if (string.IsNullOrEmpty(searchString))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            else if (searchMethod.Equals("GetById"))
            {
                people = _peopleRepository.Queryable.Where(p => p.KerberosId.Equals(searchString)).OrderBy(p => p.Id).ToList();

                if (people.Count == 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, people);
            }
            else
            {
                var queryOptions = new IdentitySearchQueryOptions
                                       {
                                           SearchString = searchString,
                                           SearchMethod = searchMethod,
                                           NumItemsPerPage = numItemsPerPage,
                                           PageNumber = pageNumber
                                       };

                return Get(queryOptions);
            }  
        }

        /// <summary>
        /// Given a JSON IdentitySearchQueryOptions sent in the request body,
        /// return the matching results.
        /// Note: Must provide "Content-type:application/json" in request headers
        /// in order for binding to queryOptions to occur.
        /// Add similar JSON to request body:
        /// {"SearchString":"postit", "SearchMethod":"Search", "NumItemsPerPage":"5", "PageNumber":"1"}
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>Matching results as JSON</returns>
        public HttpResponseMessage Get(IdentitySearchQueryOptions queryOptions)
        {
            IQueryable<dynamic> people;

            //Console.SetOut(new DebugTextWriter());
            if (queryOptions == null)
            {
                //Means json IdentitySearchQueryOptions were not provided in body of request.
                //queryOptions = new IdentitySearchQueryOptions();
                
                var retval = Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorMessage(){Error = "No JSON Query Options provided!"});
                
                return retval;
            }

            using (var conn = _dbService.GetConnection())
            {
                people = conn.Query<dynamic>("EXEC usp_IAMSearch @SearchString = @searchString, " +
                                             "@SearchMethod = @searchMethod, " +
                                             "@NumItemsPerPage = @numItemsPerPage, " +
                                             "@PageNumber = @pageNumber", 
                    new { 
                        searchString = (string)queryOptions.SearchString, 
                        searchMethod = (string)(String.IsNullOrEmpty(queryOptions.SearchMethod) ? "Search" : queryOptions.SearchMethod),
                        numItemsPerPage = (int)(queryOptions.NumItemsPerPage == 0 ? 5 : queryOptions.NumItemsPerPage),
                        pageNumber = (int)(queryOptions.PageNumber == 0 ? 1 : queryOptions.PageNumber)}
                    ).AsQueryable();
            }

            if (!people.Any())
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, people);
        }
    }

    public class ErrorMessage
    {
        public string Error { get; set; }
    }
}
