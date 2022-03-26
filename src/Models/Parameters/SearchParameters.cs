using Nervestaple.EntityFrameworkCore.Models.Criteria;
using Nervestaple.EntityFrameworkCore.Models.Entities;
using Nervestaple.EntityFrameworkCore.Models.Parameters;
using Nervestaple.WebApi.Models.converters;
using Newtonsoft.Json;

namespace Nervestaple.WebApi.Models.Parameters
{
    /// <summary>
    /// Provides a base class that all search parameter classes may extend.
    /// </summary>
    public abstract class WebApiSearchParameters<T, K> : SearchParameters<T, K>
        where T : IEntity<K>
        where K : struct
    {

        /// <summary>
        /// A set of page parameters indicating how the results of the query
        /// should be returned. This includes the size of each page, which 
        /// page and how the instances should be sorted.
        /// </summary>
        /// <returns>page parameters</returns>
        [QueryIgnore]
        [JsonConverter(typeof(PageParameterJsonConverter))]
        public override PageParameters PageParameters { get; set; } = new PageParameters();

        /// <summary>
        /// The actual search parameters used to build the query.
        /// </summary>
        [JsonIgnore]
        public abstract override ISearchCriteria<T, K> SearchCriteria { get; }
    }
}