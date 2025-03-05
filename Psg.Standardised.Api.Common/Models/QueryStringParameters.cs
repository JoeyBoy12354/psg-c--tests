using Microsoft.AspNetCore.Http;
using Psg.Standardised.Api.Common.Attributes;
using Psg.Standardised.Api.Common.Exceptions;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Web;

namespace Psg.Standardised.Api.Common.Models
{
    public abstract class QueryStringParameters
    {
        [Newtonsoft.Json.JsonProperty("fields", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Fields { get; set; }

        [Newtonsoft.Json.JsonProperty("filter", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Filter { get; set; }

        [Newtonsoft.Json.JsonProperty("filterGroup", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string FilterGroup { get; set; }

        [Newtonsoft.Json.JsonProperty("orderBy", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string OrderBy { get; set; }

        [Newtonsoft.Json.JsonProperty("orderField", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string OrderField { get; set; }

        [Newtonsoft.Json.JsonProperty("order", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Order { get; set; }

        [Newtonsoft.Json.JsonProperty("pageNumber", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int PageNumber { get; set; }

        private int _pageSize = 20;
        [Newtonsoft.Json.JsonProperty("pageSize", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
                if (_pageSize > 10000)
                {
                    _pageSize = 10000;
                }
                else if (_pageSize < 0)
                {
                    _pageSize = 20;
                }
            }
        }

        [Newtonsoft.Json.JsonProperty("performCount", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool PerformCount { get; set; } = true;

        [Newtonsoft.Json.JsonProperty("distinct", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Distinct { get; set; } = false;

        public void ValidateGetParameters(string queryParams)
        {
            // Get the public properties.
            PropertyInfo[] propInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            NameValueCollection qscoll = HttpUtility.ParseQueryString(queryParams);
            foreach (string key in qscoll.AllKeys)
            {
                if (!propInfos.Any(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new PsgException($"Unrecognised query string parameter '{key}'.", null, StatusCodes.Status400BadRequest);
                }
            }

            if (PageNumber < 1)
            {
                PageNumber = 1;
            }
        }
    }
}
