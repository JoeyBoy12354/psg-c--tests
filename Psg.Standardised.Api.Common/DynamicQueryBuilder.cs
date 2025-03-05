using Microsoft.AspNetCore.Http;
using Psg.Standardised.Api.Common.Attributes;
using Psg.Standardised.Api.Common.Exceptions;
using Psg.Standardised.Api.Common.Models;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Psg.Standardised.Api.Common
{
    public class DynamicQueryBuilder<T> where T : class
    {
        public IQueryable<DynamicQueryEntity<T>> Query { get; private set; }
        
        public QueryStringParameters Parameters { get; private set; }

        public List<DynamicQueryField> IncludeFields { get; private set; }
        
        public Dictionary<string, int> OrderFields { get; private set; }

        private IQueryable _selectedQuery = null;
        private int _dynamicFieldCounter = 0;

        public DynamicQueryBuilder(IQueryable<T> query, QueryStringParameters parameters)
        {
            Query = query.Select(s => new DynamicQueryEntity<T>() { main = s });
            Parameters = parameters;
            BuildIncludeFields();
        }

        public void BuildIncludeFields()
        {
            var properties = Parameters.GetType().GetProperties()
                .Where(w => w.DeclaringType != typeof(QueryStringParameters))
                .Select(s => new { s.Name, Attr = s.GetCustomAttribute<QueryParameterAttribute>() })
                .Where(w => w.Attr?.ParameterType == QueryParameterType.MainEntity)
                .ToDictionary(k => k.Name, v => v.Attr);

            if (!string.IsNullOrWhiteSpace(Parameters.Fields))
            {
                IncludeFields = Parameters.Fields.ToLower()
                   .Replace(" ", "")
                   .Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Distinct()
                   .Select(s => new DynamicQueryField(s, properties.ContainsKey(s) ? properties[s].EntityPath : s, s))
                   .ToList();
            }
            else
            {
                IncludeFields = properties.Select(s => new DynamicQueryField(s.Key, s.Value.EntityPath, s.Key)).ToList();
            }
        }

        public void BuildOrderFields()
        {
            string GetFieldSource(string original)
            {
                var field = IncludeFields.FirstOrDefault(f => f.Original.Equals(original, StringComparison.OrdinalIgnoreCase));
                return field?.Source ?? original;
            }

            if (!string.IsNullOrEmpty(Parameters.OrderBy))
            {
                OrderFields = Parameters.OrderBy
                    .ToLower()
                    .Replace(" ", "")
                    .Split('~', StringSplitOptions.RemoveEmptyEntries)
                    .ToDictionary(k => GetFieldSource(k.Substring(0, k.IndexOf(','))), v => int.Parse(v.Substring(v.IndexOf(',') + 1)));
            }
            else
            {
                OrderFields = new();
                if (!string.IsNullOrEmpty(Parameters.OrderField))
                {
                    OrderFields.Add(GetFieldSource(Parameters.OrderField), Parameters.Order);
                }
            }
        }

        public DynamicQueryBuilder<T> ApplyFilter()
        {
            var filterProperties = Parameters.GetType().GetProperties()
                .Where(w => w.DeclaringType != typeof(QueryStringParameters))
                .Select(s => new { Param = s, Attr = s.GetCustomAttribute<QueryParameterAttribute>() })
                .Where(w => w.Attr != null && w.Attr.ParameterType != QueryParameterType.Control);

            //Build standard query
            foreach (var item in filterProperties)
            {
                string paramVal = item.Param.GetValue(Parameters) as string;
                if (paramVal == null)
                    continue;

                string modelPropName = default;
                Type modelPropType = item.Attr.EntityType;

                if (!string.IsNullOrEmpty(item.Attr.EntityPath))
                {
                    modelPropName = item.Attr.EntityPath;
                }
                else
                {
                    // as a fallback, we will allow filtering on dynamically added includefields as well.
                    var includeField = IncludeFields.FirstOrDefault(f => f.SourceAlias.Equals(item.Param.Name))
                        ?? throw new PsgException($"Auto filters cannot be applied to dynamic field '{item.Param.Name}' since it is not selected.", null, StatusCodes.Status400BadRequest);

                    modelPropName = includeField.Source;
                }

                if (paramVal.StartsWith("~"))
                {
                    if (paramVal.StartsWith("~nco~"))
                    {
                        string check_value = paramVal.Replace("~nco~", "");
                        Query = Query.Where($"!{modelPropName}.Contains(@0)", check_value);
                    }
                    else if (paramVal.StartsWith("~co~"))
                    {
                        string check_value = paramVal.Replace("~co~", "");
                        Query = Query.Where($"{modelPropName}.Contains(@0)", check_value);
                    }
                    else if (paramVal.StartsWith("~sw~"))
                    {
                        string check_value = paramVal.Replace("~sw~", "");
                        Query = Query.Where($"{modelPropName}.StartsWith(@0)", check_value);
                    }
                    else if (paramVal.StartsWith("~neq~"))
                    {
                        string check_value = paramVal.Replace("~neq~", "");
                        ApplyFilterCondition(modelPropType, modelPropName, "!=", check_value);
                    }
                    else if (paramVal.StartsWith("~eq~"))
                    {
                        var check_value = paramVal.Replace("~eq~", "");
                        ApplyFilterCondition(modelPropType, modelPropName, "==", check_value);
                    }
                    else if (paramVal.StartsWith("~gteq~"))
                    {
                        string check_value = paramVal.Replace("~gteq~", "");
                        ApplyFilterCondition(modelPropType, modelPropName, ">=", check_value);
                    }
                    else if (paramVal.StartsWith("~lteq~"))
                    {
                        string check_value = paramVal.Replace("~lteq~", "");
                        ApplyFilterCondition(modelPropType, modelPropName, "<=", check_value);
                    }
                    else if (paramVal.StartsWith("~gt~"))
                    {
                        string check_value = paramVal.Replace("~gt~", "");
                        ApplyFilterCondition(modelPropType, modelPropName, ">", check_value);
                    }
                    else if (paramVal.StartsWith("~lt~"))
                    {
                        string check_value = paramVal.Replace("~lt~", "");
                        ApplyFilterCondition(modelPropType, modelPropName, "<", check_value);
                    }
                    else if (paramVal.StartsWith("~nnu~"))
                    {
                        Query = Query.Where(modelPropName + " != NULL");
                    }
                    else if (paramVal.StartsWith("~nu~"))
                    {
                        Query = Query.Where(modelPropName + " == NULL");
                    }
                    else if (paramVal.StartsWith("~in~"))
                    {
                        string check_value = paramVal.Replace("~in~", "");
                        bool add_quote = false;
                        if (modelPropType == typeof(string))
                        {
                            add_quote = true;
                        }

                        List<string> check_value_arr = new List<string>();
                        foreach (String entry in check_value.Split(new char[] { '~' }))
                        {
                            if (add_quote)
                            {
                                check_value_arr.Add("\"" + entry + "\"");
                            }
                            else
                            {
                                check_value_arr.Add(entry);
                            }
                        }
                        Query = Query.Where(modelPropName + " IN (" + String.Join(',', check_value_arr) + ")");
                    }
                    else if (paramVal.StartsWith("~nin~"))
                    {
                        string check_value = paramVal.Replace("~nin~", "");
                        bool add_quote = false;
                        if (modelPropType == typeof(string))
                        {
                            add_quote = true;
                        }

                        List<string> check_value_arr = new List<string>();
                        foreach (String entry in check_value.Split(new char[] { '~' }))
                        {
                            if (add_quote)
                            {
                                check_value_arr.Add("\"" + entry + "\"");
                            }
                            else
                            {
                                check_value_arr.Add(entry);
                            }
                        }

                        Query = Query.Where("!(" + modelPropName + " IN (" + String.Join(',', check_value_arr) + "))");
                    }
                    else if (paramVal.StartsWith("~bw~"))
                    {
                        string check_value = paramVal.Replace("~bw~", "");
                        string[] check_value_arr = check_value.Split(new char[] { '~' }, 2);
                        DateTime dtFrom;
                        DateTime dtTo;

                        if (!DateTime.TryParse(check_value_arr[0], out dtFrom))
                        {
                            throw new PsgException("Unable to parse provided from date.", null, StatusCodes.Status400BadRequest);
                        }
                        if (!DateTime.TryParse(check_value_arr[1], out dtTo))
                        {
                            throw new PsgException("Unable to parse provided to date.", null, StatusCodes.Status400BadRequest);
                        }
                        Query = Query.Where(modelPropName + " >= \"" + dtFrom.ToString() + "\" AND " + modelPropName + " <= \"" + dtTo.ToString() + "\"");
                    }
                    else
                    {
                        throw new PsgException($"Unsupported special filter '{paramVal}'.", null, StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    ApplyFilterCondition(modelPropType, modelPropName, "==", paramVal);
                }
            }

            return this;
        }

        private void ApplyFilterCondition(Type modelPropType, string modelPropName, string filterOperator, string paramVal)
        {
            if (modelPropType == typeof(DateTime))
            {
                if (!DateTime.TryParse(paramVal, out var parsedParamVal))
                {
                    throw new PsgException("Unable to parse " + modelPropName + " provided.", null, StatusCodes.Status400BadRequest);
                }

                Query = Query.Where($"{modelPropName} {filterOperator} @0", parsedParamVal);
            }
            else if (modelPropType == typeof(DateOnly?))
            {
                if (!DateOnly.TryParse(paramVal, out var parsedParamVal))
                {
                    throw new PsgException("Unable to parse " + modelPropName + " provided.", null, StatusCodes.Status400BadRequest);
                }

                Query = Query.Where($"{modelPropName} {filterOperator} @0", parsedParamVal);
            }
            else if (modelPropType == typeof(int))
            {
                if (!int.TryParse(paramVal, out var parsedParamVal))
                {
                    throw new PsgException("Unable to parse " + modelPropName + " provided.", null, StatusCodes.Status400BadRequest);
                }

                Query = Query.Where($"{modelPropName} {filterOperator} @0", parsedParamVal);
            }
            else if (modelPropType == typeof(long))
            {
                if (!long.TryParse(paramVal, out var parsedParamVal))
                {
                    throw new PsgException("Unable to parse " + modelPropName + " provided.", null, StatusCodes.Status400BadRequest);
                }

                Query = Query.Where($"{modelPropName} {filterOperator} @0", parsedParamVal);
            }
            else if (modelPropType == typeof(bool))
            {
                if (paramVal.Length == 1)
                {
                    if (paramVal[0] == '0')
                    {
                        paramVal = bool.FalseString;
                    }
                    else if (paramVal[0] == '1')
                    {
                        paramVal = bool.TrueString;
                    }
                }

                if (!bool.TryParse(paramVal, out var parsedParamVal))
                {
                    throw new PsgException("Unable to parse " + modelPropName + " provided.", null, StatusCodes.Status400BadRequest);
                }

                Query = Query.Where($"{modelPropName} {filterOperator} @0", parsedParamVal);
            }
            else
            {
                Query = Query.Where($"{modelPropName} {filterOperator} @0", paramVal);
            }
        }

        public DynamicQueryBuilder<T> ApplyOrder()
        {
            BuildOrderFields(); // todo, we may need to do this earlier or at least allow ExtendFind to add custom entries somewhere which take preference over these. we also need to do the rest after ExtendFind in case rebase caused field source changes.

            if (!OrderFields.Any())
                return this;

            var firstOrder = OrderFields.First();
            var orderedSource = Query.OrderBy(firstOrder.Key + (firstOrder.Value == 0 ? " ASC" : " DESC"));
            foreach (var item in OrderFields.Skip(1))
            {
                orderedSource = orderedSource.ThenBy(item.Key + (item.Value == 0 ? " ASC" : " DESC"));
            }

            Query = orderedSource;

            return this;
        }

        public DynamicQueryBuilder<T> ApplySelect()
        {
            var selectList = IncludeFields.Select(s => $"{s.Source} as {s.SourceAlias}");
            _selectedQuery = Query.Select($"new ({string.Join(',', selectList)})");

            return this;
        }

        public DynamicQueryBuilder<T> ApplyDistinct()
        {
            if (Parameters.Distinct)
            {
                Query = Query.Distinct();
                _selectedQuery = _selectedQuery.Distinct();
            }

            return this;
        }

        public PagedList<TResult> ToPagedList<TResult>() where TResult : class
        {
            var items = _selectedQuery.Skip((Parameters.PageNumber - 1) * Parameters.PageSize).Take(Parameters.PageSize + 1);
            var fetchedItems = items.Cast<TResult>().ToList(); // execute query and store results.

            var totalCount = Parameters.PerformCount ? _selectedQuery.Count() : int.MinValue;
            int totalPages = totalCount == int.MinValue
                ? int.MinValue
                : (int)Math.Ceiling(totalCount / (double)Parameters.PageSize);
            bool hasPrevious = Parameters.PageNumber > 1;
            bool hasNext = fetchedItems.Count > Parameters.PageSize;

            var pagingData = new PagingData(totalCount, Parameters.PageNumber, Parameters.PageSize, totalPages, hasPrevious, hasNext);

            return new PagedList<TResult>(fetchedItems.Take(Parameters.PageSize), pagingData);
        }

        private string GetNextDynamicField()
        {
            _dynamicFieldCounter++;
            if (_dynamicFieldCounter > 50)
                throw new NotSupportedException("Dynamic field count cannot exceed 50 fields.");

            return $"dynamic_{_dynamicFieldCounter}";
        }

        /// <summary>
        /// Projects each item in <paramref name="source2"/> onto a new <see cref="IQueryable{T}"/> result (where T is <see cref="DynamicQueryEntity{T}"/>) using the predefined select string <see cref="DynamicQueryEntity.RebaseSelect"/> and parameters from <paramref name="newProperties"/>.
        /// </summary>
        /// <param name="source2">Should be of type IQueryable&lt;a&gt; where 'a' is an anonymous type containing a 'row' property (the original <see cref="DynamicQueryEntity{T}"/>) and one or more additional properties that can be merged into <see cref="DynamicQueryEntity{T}"/>.
        /// <para>Thus, 'a' is new { <see cref="DynamicQueryEntity{T}"/> row, T1 additionalProp1, T2 additionalProp2, etc. }.</para>
        /// <para>If no newProperties are required, e.g. only Where conditions have been added, then source2 should be of type IQueryable&lt;DynamicQueryEntity&lt;T&gt;&gt; instead of IQueryable&lt;a&gt;.</para></param>
        /// <param name="joinedProperties">A list of existing joins (or null to skip this behaviour) can be specified, which allows this function to skip entries in <paramref name="newProperties"/> where <see cref="DynamicQueryField.SourceAlias"/> is already in this list. On completion, delta entries are added to this list.</param>
        /// <param name="newProperties">A list of additional properties from <paramref name="source2"/> (based on <see cref="DynamicQueryField.Source"/>) that should be merged with matching properties in 'source2.row' to produce the resulting IQueryable&lt;DynamicQueryEntity&lt;T&gt;&gt;. Do not include the 'row' property in this list.
        /// <para>Note, a new dynamic value for <see cref="DynamicQueryField.Source"/> will be set during the rebase operation.</para></param>
        public void RebaseFrom(IQueryable source2, List<string> joinedProperties, params DynamicQueryField[] newProperties)
        {
            // source2 contains no additional properties, and potentially some other query modifiers e.g. where statements.
            if (source2.GetType() == Query.GetType())
            {
                if (newProperties.Length != 0)
                    throw new ArgumentException("newProperties cannot be specified for this type of source. Ensure source2 contains properties listed in newProperties.");

                Query = source2 as IQueryable<DynamicQueryEntity<T>>;
                return;
            }

            // source2 contains additional properties, possibly previously done, and potentially some other query modifiers.
            if (joinedProperties != null)
            {
                newProperties = newProperties.Where(w => !joinedProperties.Contains(w.SourceAlias)).ToArray();
                if (newProperties.Length != 0)
                    joinedProperties.AddRange(newProperties.Select(s => s.SourceAlias));
            }

            string selector = DynamicQueryEntity<T>.RebaseSelect;
            foreach (var item in newProperties)
            {
                string tempAlias = GetNextDynamicField();
                selector = selector.Replace($"it.row.{tempAlias} as {tempAlias}", $"it.{item.Source} as {tempAlias}");
                item.Source = tempAlias;
            }

            Query = source2.Select(typeof(DynamicQueryEntity<T>), selector) as IQueryable<DynamicQueryEntity<T>>;

            return;
        }
    }

    public class DynamicQueryEntity<T> where T : class
    {
        /// <summary>
        /// Default select containing all fields in this class, regardless of whether they are used as source or final output.
        /// This allows us to use LINQ to select all values from a parent record when creating a new child record.
        /// </summary>
        public static readonly string RebaseSelect;

        static DynamicQueryEntity()
        {
            var propertyNames = typeof(DynamicQueryEntity<T>)
                .GetProperties()
                .Select(s => $"it.row.{s.Name} as {s.Name}");

            RebaseSelect = $"new {{ {string.Join(", ", propertyNames)} }}";
        }

        public T main { get; internal set; }

        #region special fields that can't easily be obtained from a table directly (using a simple select string), e.g. calculated fields or fields from complex subqueries.
        public object dynamic_1 { get; internal set; }
        public object dynamic_2 { get; internal set; }
        public object dynamic_3 { get; internal set; }
        public object dynamic_4 { get; internal set; }
        public object dynamic_5 { get; internal set; }
        public object dynamic_6 { get; internal set; }
        public object dynamic_7 { get; internal set; }
        public object dynamic_8 { get; internal set; }
        public object dynamic_9 { get; internal set; }
        public object dynamic_10 { get; internal set; }
        public object dynamic_11 { get; internal set; }
        public object dynamic_12 { get; internal set; }
        public object dynamic_13 { get; internal set; }
        public object dynamic_14 { get; internal set; }
        public object dynamic_15 { get; internal set; }
        public object dynamic_16 { get; internal set; }
        public object dynamic_17 { get; internal set; }
        public object dynamic_18 { get; internal set; }
        public object dynamic_19 { get; internal set; }
        public object dynamic_20 { get; internal set; }
        public object dynamic_21 { get; internal set; }
        public object dynamic_22 { get; internal set; }
        public object dynamic_23 { get; internal set; }
        public object dynamic_24 { get; internal set; }
        public object dynamic_25 { get; internal set; }
        public object dynamic_26 { get; internal set; }
        public object dynamic_27 { get; internal set; }
        public object dynamic_28 { get; internal set; }
        public object dynamic_29 { get; internal set; }
        public object dynamic_30 { get; internal set; }
        public object dynamic_31 { get; internal set; }
        public object dynamic_32 { get; internal set; }
        public object dynamic_33 { get; internal set; }
        public object dynamic_34 { get; internal set; }
        public object dynamic_35 { get; internal set; }
        public object dynamic_36 { get; internal set; }
        public object dynamic_37 { get; internal set; }
        public object dynamic_38 { get; internal set; }
        public object dynamic_39 { get; internal set; }
        public object dynamic_40 { get; internal set; }
        public object dynamic_41 { get; internal set; }
        public object dynamic_42 { get; internal set; }
        public object dynamic_43 { get; internal set; }
        public object dynamic_44 { get; internal set; }
        public object dynamic_45 { get; internal set; }
        public object dynamic_46 { get; internal set; }
        public object dynamic_47 { get; internal set; }
        public object dynamic_48 { get; internal set; }
        public object dynamic_49 { get; internal set; }
        public object dynamic_50 { get; internal set; }
        #endregion
    }

    public class DynamicQueryField
    {
        public string Original { get; private set; }

        public string Source { get; set; } // name for DB query purposes or calculated field expression.

        public string SourceAlias { get; private set; } // field alias for DB query purposes and also result model.

        public DynamicQueryField(string original, string source, string sourceAlias)
        {
            Original = original;
            Source = source;
            SourceAlias = sourceAlias;
        }
    }
}
