using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityService.Models
{
    public class IdentitySearchQueryOptions
    {
        public IDictionary<string, string> SearchMethods = new Dictionary<string, string>(); 
        private string _searchString = "";
        public string SearchString { 
            get{ return _searchString; }
            set { _searchString = value; }
        }

        public IdentitySearchQueryOptions()
        {
            SearchMethods.Add(new KeyValuePair<string, string>("GetById", "Search by Kerberos Id"));
            SearchMethods.Add(new KeyValuePair<string, string>("GetByEmail", "Search by Email Address"));
            SearchMethods.Add(new KeyValuePair<string, string>("GetUnique", "Search by Kerberos or Email"));
            SearchMethods.Add(new KeyValuePair<string, string>("Search", "'LIKE' based Search of Kerberos or Email or Display Name"));
        }

        private string _searchMethod = "GetById";
        public string SearchMethod
        {
            get { return _searchMethod; }
            set { _searchMethod = value; }
        }

        private int _numItemsPerPage = 5;
        public int NumItemsPerPage
        {
            get { return _numItemsPerPage; }
            set { _numItemsPerPage = value; }
        }

        private int _pageNumber = 1;
        public int PageNumber
        {
            get { return _pageNumber; }
            set { _pageNumber = value; }
        }
    }
}