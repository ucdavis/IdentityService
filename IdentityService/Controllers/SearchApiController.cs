using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Helpers;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;
using IdentityService.Core.Domain;

namespace IdentityService.Controllers
{
    public class SearchApiController : ApiController
    {
        private readonly IRepositoryWithTypedId<People, string> _peopleRepository;

        public System.Linq.Expressions.Expression<Func<People, bool>> PeopleSearchExpression { get; set; }

        public SearchApiController()
        {
            _peopleRepository = SmartServiceLocator<IRepositoryWithTypedId<People, string>>.GetService();
        }

         // This doesn't currently get called, so it has to be done in the default c'tor.
        public SearchApiController(IRepositoryWithTypedId<People, string> peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        // GET api/SearchApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/SearchApi/{KerberosId}
        public HttpResponseMessage Get(string id)
        {
            // Need to return a list because some peole have multiple rows because of some one-to-many relationships
            // such as StudentId, PPS Id, etc.
            IList<People> people = new List<People>();

            people = _peopleRepository.Queryable.Where(p => p.KerberosId.Equals(id)).OrderBy(p => p.Id).ToList();

            if (people.Count == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, people);
        }
    }
}
