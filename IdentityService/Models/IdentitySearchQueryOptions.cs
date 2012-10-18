using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityService.Models
{
    public class IdentitySearchQueryOptions
    {
        private string _searchString = "postit";
        public string SearchString { 
            get{ return _searchString; }
            set { _searchString = value; }
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