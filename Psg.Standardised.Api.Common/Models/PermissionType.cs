namespace Psg.Standardised.Api.Common.Models
{
    public static class PermissionType
    {
        public const string Add = "add";
        public const string Edit = "edit";
        public const string Delete = "delete";
        public const string Get = "get";
        public const string Find = "find";
        public const string Lookup = "lookup";
        public const string Use = "use";
        public const string Public = "public";
        public const string Mine = "mine";

        public static readonly string[] All = { Add, Edit, Delete, Get, Find, Lookup, Use, Public, Mine };
        public static readonly string[] Read = { Get, Find, Lookup };
    }
}
