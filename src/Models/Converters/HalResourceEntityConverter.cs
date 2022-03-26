using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Nervestaple.EntityFrameworkCore.Models.Entities;
using Nervestaple.WebApi.Models.Entities;
using Nervestaple.WebApi.Models.Resources;

namespace Nervestaple.WebApi.Models.converters {

    /// <summary>
    /// Provides an object for converting entity instances into HAL resources.
    /// </summary>
    public class HalResourceEntityConverter<TEntity> where TEntity : IEntity {

        /// <summary>
        /// Returns a HalResource for the provided entity instance. The 
        /// UrlHelper is used when generating links.
        /// </summary>
        /// <param name="url">UrlHelper for generating links</param>
        /// <param name="entity">entity to convert</param>
        /// <returns>HAL resource instance</returns>
        public static HalResource<TEntity> GetResource(IUrlHelper url, TEntity entity) {
            return GetResource(
                url,
                new Dictionary<string, string>(),
                new Dictionary<string, object>(),
                entity
            );
        }

        /// <summary>
        /// Returns a HalResource for the provided entity instance. The 
        /// UrlHelper is used when generating links. The links and meta
        /// dictionaries will be added to the links and meta dictionaries
        /// of the returned HAL resources instance.
        /// </summary>
        /// <param name="url">UrlHelper for generating links</param>
        /// <param name="links">dictionary with extra links</param>
        /// <param name="meta">dictionary with extra meta information</param>
        /// <param name="entity">entity to convert</param>
        /// <returns>HAL resource instance</returns>
        public static HalResource<TEntity> GetResource(IUrlHelper url, IDictionary<string, string> links,
            IDictionary<string, object> meta, TEntity entity) {

            // prepare our entity by populating the embedded fields
            IDictionary<string, IEntity> embeddedEntities = new Dictionary<string, IEntity>();
            IDictionary<string, IEnumerable<IEntity>> embeddedEntityCollections = new Dictionary<string, IEnumerable<IEntity>>();
            GetEmbeddedEntities(entity, embeddedEntities, embeddedEntityCollections);
            
            if (links == null) {
                links = new Dictionary<string, string>();
            }
            if(meta == null) {
                meta = new Dictionary<string, object>();
            }

            // prepare our entity by populating the link and meta fields
            PopulateLinks(url, entity, links);
            entity.PopulateMeta(meta);

            // we're going to merge our two typed embedded dictionaries into one outgoing dictionary
            IDictionary<string, object> embeddedOut = new Dictionary<string, object>();

            foreach (KeyValuePair<string, IEntity> entry in embeddedEntities) {
                embeddedOut.Add(entry.Key, HalResourceEntityConverter<IEntity>.GetResource(url, entry.Value));
            }

            foreach (KeyValuePair<string, IEnumerable<IEntity>> entry in embeddedEntityCollections) {
                IList<HalResource<IEntity>> embeddedListOut = new List<HalResource<IEntity>>();
                embeddedOut.Add(entry.Key, embeddedListOut);
                foreach(IEntity entityThis in entry.Value) {
                    embeddedListOut.Add(HalResourceEntityConverter<IEntity>.GetResource(url, entityThis));
                }
            }

            // add a self link if we don't have one
            if (links.ContainsKey("self") == false) {
                links.Add("self", url.Action("Get", entity.GetType().Name) + "/" + entity.Id);
            }

            // add the resource type if it's missing
            if(meta.ContainsKey("type") == false) {
                meta.Add("type", entity.GetType().Name);
            }

            return new HalResource<TEntity>(
                links, 
                meta,
                embeddedOut,
                entity
            );
        }

        /// <summary>
        /// Populates the provided dictionaries with the embedded resources of
        /// the provided Entity instance. This method uses attributes on the 
        /// Entity class properties to decide what resources should be embedded.
        /// </summary>
        /// <param name="entity">entity to convert</param>
        /// <param name="embeddedEntities">dictionary of singular entities to populate></param>
        /// <param name="embeddedEntityCollections">dictionary of entity collections to populate</param>
        private static void GetEmbeddedEntities(
            TEntity entity,
            IDictionary<string, IEntity> embeddedEntities,
            IDictionary<string, IEnumerable<IEntity>> embeddedEntityCollections) {

            IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties().Where(
                p => p.GetValue(entity) != null
                    && p.GetCustomAttributes().FirstOrDefault(a => a.GetType().Equals(typeof(Embedded))) != null);
            
            foreach(PropertyInfo property in properties) {
                if(property.GetValue(entity) is IEnumerable<IEntity>) {
                    IEnumerable<IEntity> entityEmbedded = (IEnumerable<IEntity>) property.GetValue(entity);
                    IList<IEntity> entitiesOut = new List<IEntity>();

                    // we don't want to embed the entity we're converting
                    foreach(IEntity entityOut in entityEmbedded) {
                        if(!entityOut.GetType().Equals(entity.GetType()) && !entityOut.Id.Equals(entity.Id)) {
                            entitiesOut.Add(entityOut);
                        }
                    }

                    embeddedEntityCollections.Add(property.Name, entitiesOut);
                } else {
                    IEntity entityEmbedded = (IEntity) property.GetValue(entity);

                    // we don't want to embed the entity we're converting
                    if(!(entityEmbedded.GetType().Equals(entity.GetType()) && entityEmbedded.Id.Equals(entity.Id))) {
                        embeddedEntities.Add(property.Name, entityEmbedded);
                    }
                }
            }
        }

        /// <summary>
        /// Populates the provided dictionary with link information. These
        /// links are derived from attributes on the Entity class.
        /// </summary>
        /// <param name="url">UrlHelper for generating links</param>
        /// <param name="entity">entity to convert</param>
        /// <param name="links">dictionary of links to populate</param>
        private static void PopulateLinks(IUrlHelper url, TEntity entity, IDictionary<string, string> links) {
            var path = url.ActionContext.HttpContext.Request.Path.Value;
            var root = path.Substring(0, path.LastIndexOf("/"));
            
            var linkAttributes = entity.GetType().GetCustomAttributes().Where(a => a.GetType().Equals(typeof(LinkedResource)));
            foreach(var linkAttribute in linkAttributes) {
                LinkedResource linkThis = (LinkedResource) linkAttribute;

                var urlThis = path + "/" + linkThis.Resource + "/" + entity.Id;
                if (path.EndsWith(entity.Id.ToString())) {
                    urlThis = root + "/" + linkThis.Resource + "/" + entity.Id;
                }

                //links.Add(linkThis.Name, urlThis);
                links.Add(linkThis.Name, url.Action("Get", linkThis.Resource) + "/" + entity.GetType().Name + "/" + entity.Id);
            }
        }
    }

    /// <summary>
    /// Provides an object for converting entity instances into HAL resources.
    /// </summary>
    public class HalResourceAnonymousConverter {

        /// <summary>
        /// Returns a HalResource for the provided entity instance. The 
        /// UrlHelper is used when generating links.
        /// </summary>
        /// <param name="url">UrlHelper for generating links</param>
        /// <param name="entity">entity to convert</param>
        /// <returns>HAL resource instance</returns>
        public static HalResource<object> GetResource(IUrlHelper url, object entity) {
            return GetResource(
                url,
                new Dictionary<string, string>(),
                new Dictionary<string, object>(),
                entity
            );
        }

        /// <summary>
        /// Returns a HalResource for the provided entity instance. The 
        /// UrlHelper is used when generating links. The links and meta
        /// dictionaries will be added to the links and meta dictionaries
        /// of the returned HAL resources instance.
        /// </summary>
        /// <param name="url">UrlHelper for generating links</param>
        /// <param name="links">dictionary with extra links</param>
        /// <param name="meta">dictionary with extra meta information</param>
        /// <param name="entity">entity to conver</param>
        /// <returns>HAL resource instance</returns>
        public static HalResource<object> GetResource(IUrlHelper url, IDictionary<string, string> links,
            IDictionary<string, object> meta, object entity) {

            // prepare our entity by populating the embedded fields
            IDictionary<string, IEntity> embeddedEntities = new Dictionary<string, IEntity>();
            IDictionary<string, IEnumerable<IEntity>> embeddedEntityCollections = new Dictionary<string, IEnumerable<IEntity>>();
            
            if (links == null) {
                links = new Dictionary<string, string>();
            }
            if(meta == null) {
                meta = new Dictionary<string, object>();
            }

            // we're going to merge our two typed embedded dictionaries into one outgoing dictionary
            IDictionary<string, object> embeddedOut = new Dictionary<string, object>();

            foreach (KeyValuePair<string, IEntity> entry in embeddedEntities) {
                embeddedOut.Add(entry.Key, HalResourceEntityConverter<IEntity>.GetResource(url, entry.Value));
            }

            foreach (KeyValuePair<string, IEnumerable<IEntity>> entry in embeddedEntityCollections) {
                IList<HalResource<IEntity>> embeddedListOut = new List<HalResource<IEntity>>();
                embeddedOut.Add(entry.Key, embeddedListOut);
                foreach(IEntity entityThis in entry.Value) {
                    embeddedListOut.Add(HalResourceEntityConverter<IEntity>.GetResource(url, entityThis));
                }
            }

            // add the resource type if it's missing
            if(meta.ContainsKey("type") == false) {
                meta.Add("type", "Anonymous");
            }

            return new HalResource<object>(
                links, 
                meta,
                embeddedOut,
                entity
            );
        }
    }
}