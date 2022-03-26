using System.Collections;
using System.Collections.Generic;

namespace Nervestaple.WebApi.Models.Resources {

    /// <summary>
    /// A Hypertext Application Language representation of the resource.
    /// </summary>
    public class HalResource<T> {

        /// <summary>
        /// dictionary of links for this resource
        /// </summary>
        public IDictionary<string, string> _links;

        /// <summary>
        /// dictionary of metadata about the resource
        /// </summary>
        public IDictionary<string, object> _meta;

        /// <summary>
        /// dictionary of embeded resources
        /// </summary>
        public IDictionary<string, object> _embedded;

        /// <summary>
        /// the resource itself
        /// </summary>
        public T Resource;

        /// <summary>
        /// Creates a new instance.
        /// <param name="selfLink">Link to this resource</param>
        /// <param name="resource">The resource being wrapped</param>
        /// </summary>
        public HalResource(string selfLink, T resource) : this(
            new Dictionary<string, string> {
                { "self", selfLink }
            }, new Dictionary<string, object> {

            }, resource
        ) {

        }

        /// <summary>
        /// Creates a new instance.
        /// <param name="links">A set of links for this resource</param>
        /// <param name="meta">A dictionary of metadata about this resource</param>
        /// <param name="resource">The resource being wrapped</param>
        /// </summary>
         public HalResource(IDictionary<string, string> links, IDictionary<string, object> meta, 
            T resource) : this(links, meta, new Dictionary<string, object>(), resource) {

            }

        /// <summary>
        /// Creates a new instance.
        /// <param name="links">A set of links for this resource</param>
        /// <param name="meta">A dictionary of metadata about this resource</param>
        /// <param name="embedded">A dictionary of resources embedded into this resource</param>
        /// <param name="resource">The resource being wrapped</param>
        /// </summary>
        public HalResource(IDictionary<string, string> links, IDictionary<string, object> meta, 
            IDictionary<string, object> embedded, T resource) {
            _links = links;
            _meta = meta;
            _embedded = embedded;
            Resource = resource;
        }

        /// <summary>
        /// Returns the type of wrapped object, indicating either the short
        /// name for the type or "Dictionary" or "Collection"
        /// </summary>
        private string GetType(object resource) {
            if(resource is DictionaryBase) {
                return "Dictionary";
            } else if(resource is IEnumerable) {
                return "Collection";
            } else {
                return resource.GetType().Name;
            }
        }
    }
}