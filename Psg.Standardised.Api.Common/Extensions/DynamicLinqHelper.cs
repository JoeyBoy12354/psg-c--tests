using System.Linq.Dynamic.Core;
using System.Text;

namespace Psg.Standardised.Api.Common.Extensions
{
    public static class LeftJoinExtension
    {
        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys. 
        /// Left part will be always return.
        /// </summary>
        /// <param name="inner">The sequence to join.</param>
        /// <param name="leftKey">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKey">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="leftSelectedProperties">List of properties to return from the first sequence.</param>
        /// <param name="innerSelectedProperties">List of properties to return from the second sequence.</param>
        /// <returns>An System.Linq.IQueryable obtained by performing a left join on two sequences.</returns>
        public static IQueryable LeftJoin(this IQueryable query,
                                          IQueryable inner,
                                          string leftKey,
                                          string innerKey,
                                          List<(string field, string fieldAs)> leftSelectedProperties,
                                          List<(string field, string fieldAs)> innerSelectedProperties)
        {
            if (leftSelectedProperties is null)
                throw new ArgumentNullException("leftSelectedProperties","leftSelectedProperties cannot be null");

            if (innerSelectedProperties is null)
                throw new ArgumentNullException("innerSelectedProperties","innerSelectedProperties cannot be null");

            query = query.GroupJoin(ParsingConfig.DefaultEFCore21,
                                    inner,
                                    leftKey,
                                    innerKey,
                                    GroupJoinSelector);

            string selectedProperties = GenerateSelectedPropertiesString(leftSelectedProperties,
                                                                         innerSelectedProperties);

            query = query.SelectMany(SelectManyCollectionSelector,
                                     selectedProperties,
                                     SelectManyOuter,
                                     SelectManyInner);

            return query;
        }

        /// <summary>
        /// Turn the two property sequences into a string formatted for select many
        /// </summary>
        /// <param name="outerSelectedProperties">List of properties to return from the first sequence.</param>
        /// <param name="innerSelectedProperties">List of properties to return from the second sequence.</param>
        /// <returns>String formatted for select many</returns>
        private static string GenerateSelectedPropertiesString(List<(string field, string fieldAs)> outerSelectedProperties,
                                                               List<(string field, string fieldAs)> innerSelectedProperties)
        {
            if (outerSelectedProperties.Count == 0 && innerSelectedProperties.Count == 0)
                throw new ArgumentException("outerSelectedProperties or innerSelectedProperties must contains a value");

            StringBuilder sb = new StringBuilder();

            sb.Append("new { ");
            outerSelectedProperties.ForEach(p => sb.Append($"{SelectManyOuterSelection}.{p.field} as {p.fieldAs},"));
            innerSelectedProperties.ForEach(p => sb.Append($"{SelectManyInnerSelection} == null ? null : {SelectManyInnerSelection}.{p.field} as {p.fieldAs},"));
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            return sb.ToString();
        }

        private static string SelectManyOuter { get => "source"; }
        private static string SelectManyInner { get => "detail"; }

        private static string SelectManyOuterSelection { get => $"{SelectManyOuter}.{GroupJoinOuterPrefix}"; }
        private static string SelectManyInnerSelection { get => SelectManyInner; }

        private static string GroupJoinOuterPrefix { get => "A"; }
        private static string GroupJoinInnerPrefix { get => "B"; }

        private static string GroupJoinSelector { get => $"new (outer AS {GroupJoinOuterPrefix}, inner AS {GroupJoinInnerPrefix})"; }
        private static string SelectManyCollectionSelector { get => $"{GroupJoinInnerPrefix}.DefaultIfEmpty()"; }
    }
}