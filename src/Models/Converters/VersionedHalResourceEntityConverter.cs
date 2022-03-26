using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nervestaple.EntityFrameworkCore.Models.Entities;
using Nervestaple.WebApi.Models.Entities;
using Nervestaple.WebApi.Models.Resources;

namespace Nervestaple.WebApi.Models.converters {
    /// <summary>
    /// Provides an object for converting entity instances into HAL resources
    /// and will handle versioned API links.
    /// </summary>
    public class VersionedHalResourceEntityConverter<TEntity> where TEntity : IEntity {
        /// <summary>
        /// Returns a HalResource for the provided entity instance. The 
        /// UrlHelper is used when generating links.
        /// </summary>
        /// <param name="url">UrlHelper for generating links</param>
        /// <param name="apiVersion">API version data for this method</param> 
        /// <param name="entity">entity to convert</param>
        /// <returns>HAL resource instance</returns>
        public static HalResource<TEntity> GetResource(IUrlHelper url, ApiVersion apiVersion, TEntity entity) {
            return GetResource(
                url,
                apiVersion,
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
        /// <param name="apiVersion">API version data for this method</param>
        /// <param name="links">dictionary with extra links</param>
        /// <param name="meta">dictionary with extra meta information</param>
        /// <param name="entity">entity to convert</param>
        /// <returns>HAL resource instance</returns>
        public static HalResource<TEntity> GetResource(IUrlHelper url, ApiVersion apiVersion, IDictionary<string, string> links,
            IDictionary<string, object> meta, TEntity entity) {

            // prepare our entity by populating the embedded fields
            IDictionary<string, IEntity> embeddedEntities = new Dictionary<string, IEntity>();
            IDictionary<string, IEnumerable<IEntity>> embeddedEntityCollections =
                new Dictionary<string, IEnumerable<IEntity>>();
            GetEmbeddedEntities(entity, embeddedEntities, embeddedEntityCollections);

            if (links == null) {
                links = new Dictionary<string, string>();
            }

            if (meta == null) {
                meta = new Dictionary<string, object>();
            }

            // prepare our entity by populating the link and meta fields
            PopulateLinks(url, apiVersion, entity, links);
            entity.PopulateMeta(meta);

            // we're going to merge our two typed embedded dictionaries into one outgoing dictionary
            IDictionary<string, object> embeddedOut = new Dictionary<string, object>();

            foreach (KeyValuePair<string, IEntity> entry in embeddedEntities) {
                embeddedOut.Add(entry.Key, VersionedHalResourceEntityConverter<IEntity>.GetResource(url, apiVersion, entry.Value));
            }

            foreach (KeyValuePair<string, IEnumerable<IEntity>> entry in embeddedEntityCollections) {
                IList<HalResource<IEntity>> embeddedListOut = new List<HalResource<IEntity>>();
                embeddedOut.Add(entry.Key, embeddedListOut);
                foreach (IEntity entityThis in entry.Value) {
                    embeddedListOut.Add(VersionedHalResourceEntityConverter<IEntity>.GetResource(url, apiVersion, entityThis));
                }
            }

            // add a self link if we don't have one
            if (links.ContainsKey("self") == false) {
                
                // always use the "Get" action
                var action = "Get";
                
                // compute the URL for the route and our entity (short names should match controllers)
                var urlOut = url.Action(
                    action, 
                    entity.GetType().ShortDisplayName(), 
                    new { version = apiVersion.MajorVersion.ToString()});

                // if the URL ends with our action then strip it out (typically our URLs do not end with
                // the action name, i.e. .../Parent/Get, /Parent/GetById, etc.
                if (urlOut.EndsWith("/" + action, StringComparison.InvariantCultureIgnoreCase)) {
                    urlOut = urlOut.Substring(0, urlOut.LastIndexOf("/" + action, StringComparison.InvariantCultureIgnoreCase));
                }

                // don't add our entities ID to actions that typically do not accept the ID in the URL
                if (action.Equals("Get") || action.Equals("Query") 
                    || action.Equals("Create") || action.Equals("Update") || action.Equals("Delete")) {
                    urlOut += "/" + entity.Id;
                }
                
                links.Add("self", urlOut);
            }

            // add the resource type if it's missing
            if (meta.ContainsKey("type") == false) {
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

            foreach (PropertyInfo property in properties) {
                if (property.GetValue(entity) is IEnumerable<IEntity>) {
                    IEnumerable<IEntity> entityEmbedded = (IEnumerable<IEntity>) property.GetValue(entity);
                    IList<IEntity> entitiesOut = new List<IEntity>();

                    // we don't want to embed the entity we're converting
                    foreach (IEntity entityOut in entityEmbedded) {
                        if (!entityOut.GetType().Equals(entity.GetType()) && !entityOut.Id.Equals(entity.Id)) {
                            entitiesOut.Add(entityOut);
                        }
                    }

                    embeddedEntityCollections.Add(property.Name, entitiesOut);
                }
                else {
                    IEntity entityEmbedded = (IEntity) property.GetValue(entity);

                    // we don't want to embed the entity we're converting
                    if (!(entityEmbedded.GetType().Equals(entity.GetType()) && entityEmbedded.Id.Equals(entity.Id))) {
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
        /// <param name="apiVersion">API version data for this method</param>
        /// <param name="entity">entity to convert</param>
        /// <param name="links">dictionary of links to populate</param>
        private static void PopulateLinks(IUrlHelper url, ApiVersion apiVersion, TEntity entity, IDictionary<string, string> links) {
            var linkAttributes = entity.GetType().GetCustomAttributes()
                .Where(a => a.GetType().Equals(typeof(LinkedResource)));
            
            foreach (var linkAttribute in linkAttributes) {
                LinkedResource linkThis = (LinkedResource) linkAttribute;
                
                // calculate the URL to the linked resource's controller
                links.Add(linkThis.Name,
                    url.Action("Get",    // most controller's have a "Get" method that has no URL parameters
                        new {
                            controller = linkThis.Resource,    // controller's are typically named after their resource
                            version = apiVersion.MajorVersion.ToString()
                        }) 
                    + "/" + entity.GetType().Name + "/" + entity.Id);  // tack the id of the linked entity onto the end
            }
        }
    }
}