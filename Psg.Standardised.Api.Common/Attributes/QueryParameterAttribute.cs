namespace Psg.Standardised.Api.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QueryParameterAttribute : Attribute
    {
        public QueryParameterAttribute(QueryParameterType parameterType, Type entityType, string entityPath)
        {
            if (entityType == null && parameterType != QueryParameterType.Control)
                throw new ArgumentException($"entityType must be set for QueryParameterType {parameterType}.");

            if (string.IsNullOrEmpty(entityPath) && parameterType == QueryParameterType.MainEntity)
                throw new ArgumentException($"entityPath must be set for QueryParameterType {parameterType}.");

            ParameterType = parameterType;
            EntityType = entityType;
            EntityPath = entityPath;
        }

        public QueryParameterType ParameterType { get; }

        public Type EntityType { get; }

        public string EntityPath { get; }
    }

    public enum QueryParameterType
    {
        /// <summary>
        /// Does not map directly to an entity field that can be selected or filtered. Use this to control the query directly. (ExtendFind)
        /// Requires: none
        /// </summary>
        Control = 0,

        /// <summary>
        /// Maps directly to a main entity field that can be auto-selected, auto-filtered and auto-sorted.
        /// Requires: EntityType, EntityPath
        /// </summary>
        MainEntity = 1,

        /// <summary>
        /// Maps directly to a secondary or calculated entity field that can be manually-selected (ExtendFind), auto-filtered (only if selected, otherwise use ExtendFind) and auto-sorted (only if selected, otherwise use ExtendFind).
        /// Requires: EntityType
        /// </summary>
        OtherEntity = 2,
    }
}
