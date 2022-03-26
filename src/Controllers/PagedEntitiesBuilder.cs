using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nervestaple.EntityFrameworkCore.Models.Entities;
using Nervestaple.EntityFrameworkCore.Models.Parameters;
using Nervestaple.WebApi.Models.converters;
using Nervestaple.WebApi.Models.Resources;

namespace Nervestaple.WebApi.Controllers {

    /// <summary>
    /// Returns a PagedEntities of enumerable instances wrapped in HalResource
    /// instances for the provides page parameters.
    /// </summary>
    public class PagedEntitiesBuilder<E, K> 
        where E: IEntity<K>
        where K: struct {

        /// <summary>
        /// the resource to wrap (most likely containing an enumerable)
        /// </summary>
        public HalResource<IEnumerable<HalResource<E>>> Resource { get; }

        /// <summary>
        /// Creates a new instance.
        /// <param name="url">UrlHelper used when generating reference links</param>
        /// <param name="pageParameters">Parameters for paging result lists</param>
        /// <param name="selfLink">Link to this resource, use to build paging links</param>
        /// <param name="pagedResource">The resource to page</param>
        /// </summary>
        public PagedEntitiesBuilder(
            IUrlHelper url,
            IPageParameters pageParameters, 
            string selfLink,
            PagedEntities<E> pagedResource) 
        {

            // generate our url parameters
            var urlParams = "?size=" + pageParameters.Size;
            string sortParam = null;
            bool sortDescParam = false;
            if (pageParameters.GetSort() != null && pageParameters.GetSort().Count() > 0) {
                sortParam = pageParameters.GetSort().Last().Field;
                sortDescParam = pageParameters.GetSort().Last().Desc;
                foreach (SortParameter parameter in pageParameters.GetSort()) {
                    urlParams += "&Sort=" + parameter.Field + "&Desc=" + parameter.Desc;
                }
            }

            // generate links
            var self = selfLink + urlParams;
            var previous = selfLink + urlParams  + "&page=" + (pageParameters.Page - 1);
            var next = selfLink + urlParams + "&page=" 
                + (pageParameters.Page + 1 < pagedResource.Pages ? pageParameters.Page + 1 : pagedResource.Pages);
            var first = selfLink + urlParams + "&page=" + 0;
            var last = selfLink + urlParams + "&page=" + pagedResource.Pages;

            Resource = new HalResource<IEnumerable<HalResource<E>>>(

                // dictionary of links
                new Dictionary<string, string> {
                    { "self", self },
                    { "previous", pageParameters.Page == 0 ? null : previous },
                    { "next", pageParameters.Page < pagedResource.Pages ? next : null },
                    { "first", first },
                    { "last", last }
                }, 

                // dictionary of metadata
                new Dictionary<string, object> {
                    { "count", pagedResource.Count },
                    { "pages", pagedResource.Pages },
                    { "page", pagedResource.PageParameters.Page },
                    { "size", pagedResource.PageParameters.Size },
                    { "sort", sortParam },
                    { "desc", sortDescParam }
                },

                // wrap the resource enumerable in a HalResource
                pagedResource.Resource.Select(r => HalResourceEntityConverter<E>.GetResource(url, r)));
        }
    }

    /// <summary>
    /// Returns a PagedEntities of enumerable anonymous instances wrapped in 
    /// HalResource instances for the provides page parameters.
    /// </summary>
    public class PagedAnonymousBuilder {

        /// <summary>
        /// the resource to wrap (most likely containing an enumerable)
        /// </summary>
        public HalResource<IEnumerable<HalResource<object>>> Resource { get; }

        /// <summary>
        /// Creates a new instance.
        /// <param name="url">UrlHelper used when generating reference links</param>
        /// <param name="pageParameters">Parameters for paging result lists</param>
        /// <param name="selfLink">Link to this resource, use to build paging links</param>
        /// <param name="pagedResource">The resource to page</param>
        /// </summary>
        public PagedAnonymousBuilder(
            IUrlHelper url,
            IPageParameters pageParameters, 
            string selfLink,
            PagedEntities<object> pagedResource) 
        {

            // generate our url parameters
            var urlParams = "?size=" + pageParameters.Size;
            string sortParam = null;
            bool sortDescParam = false;
            if (pageParameters.GetSort() != null && pageParameters.GetSort().Count() > 0) {
                sortParam = pageParameters.GetSort().Last().Field;
                sortDescParam = pageParameters.GetSort().Last().Desc;
                foreach (SortParameter parameter in pageParameters.GetSort()) {
                    urlParams += "&Sort=" + parameter.Field + "&Desc=" + parameter.Desc;
                }
            }

            // generate links
            var self = selfLink + urlParams;
            var previous = selfLink + urlParams  + "&page=" + (pageParameters.Page - 1);
            var next = selfLink + urlParams + "&page=" 
                + (pageParameters.Page + 1 < pagedResource.Pages ? pageParameters.Page + 1 : pagedResource.Pages);
            var first = selfLink + urlParams + "&page=" + 0;
            var last = selfLink + urlParams + "&page=" + pagedResource.Pages;

            Resource = new HalResource<IEnumerable<HalResource<object>>>(

                // dictionary of links
                new Dictionary<string, string> {
                    { "self", self },
                    { "previous", pageParameters.Page == 0 ? null : previous },
                    { "next", pageParameters.Page < pagedResource.Pages ? next : null },
                    { "first", first },
                    { "last", last }
                }, 

                // dictionary of metadata
                new Dictionary<string, object> {
                    { "count", pagedResource.Count },
                    { "pages", pagedResource.Pages },
                    { "page", pagedResource.PageParameters.Page },
                    { "size", pagedResource.PageParameters.Size },
                    { "sort", sortParam },
                    { "desc", sortDescParam }
                },

                // wrap the resource enumerable in a HalResource
                pagedResource.Resource.Select(r => HalResourceAnonymousConverter.GetResource(url, r)));
        }
    }
}