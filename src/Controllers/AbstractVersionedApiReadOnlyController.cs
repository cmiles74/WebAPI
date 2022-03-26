using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nervestaple.EntityFrameworkCore.Models.Entities;
using Nervestaple.EntityFrameworkCore.Models.Parameters;
using Nervestaple.WebApi.Models.converters;
using Nervestaple.WebService.Services;

namespace Nervestaple.WebApi.Controllers {
     /// <summary>
    /// Provides default implementation of a read-only controller methods.
    /// </summary>
    public abstract class AbstractVersionedApiReadOnlyController<T, K> : EntityController, IVersionedApiReadOnlyController<T, K> 
        where T: IEntity<K>
        where K: struct {

        /// <summary>
        /// The backing service for this controller
        /// </summary>
        protected readonly IReadOnlyService<T, K> Service;

        /// <summary>
        /// Creates a new instance.
        /// <param name="service">Backing service for this controller</param>
        /// </summary>
        public AbstractVersionedApiReadOnlyController(IReadOnlyService<T, K> service) {
            Service = service;
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleGet(SimplePageParameters parameters) {
            return HandleGetAsync(parameters).Result;
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> HandleGetAsync(SimplePageParameters parameters) {
            if(parameters == null) {
                Response.StatusCode = 400;
                return Content("The provided page parameters could not be parsed");
            }

            //return WrapExceptionHandler(() => {
            //}); 
            var pagedResource = await Service.GetAsync(parameters);
            return new ObjectResult(
                new PagedEntitiesBuilder<T, K>(
                        Url,
                        parameters, 
                        Url.Action("Get", new
                        {
                            controller = ControllerContext.ActionDescriptor.ControllerName
                        }),
                        pagedResource)
                    .Resource);
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleGetById(K id) {
            return HandleGetByIdAsync(id).Result;
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> HandleGetByIdAsync(K id) {
            /*  return WrapExceptionHandler(() => {
                 
             });*/
            var entity = await Service.GetByIdAsync(id);
            if (entity == null) {
                return NotFound();
            }

            return new ObjectResult(
                HalResourceEntityConverter<T>.GetResource(Url, entity));
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleQuery(ISearchParameters<T, K> parameters) {
            return HandleQueryAsync(parameters).Result;
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> HandleQueryAsync(ISearchParameters<T, K> parameters) {
            if(parameters == null) {
                Response.StatusCode = 400;
                return Content("The provided search parameters could not be parsed");
            }

            // return WrapExceptionHandler(() => {
            //     
            // });
                
            PagedEntities<T> pagedResource = null;
            
            pagedResource = await Service.QueryAsync(parameters.SearchCriteria, parameters.PageParameters);
            
            return new ObjectResult(
                new PagedEntitiesBuilder<T, K>(
                        Url,
                        parameters.PageParameters, 
                        Url.Action("Get", new
                        {
                            controller = ControllerContext.ActionDescriptor.ControllerName
                        }),
                        pagedResource)
                    .Resource);
        }

        private static T DefaultConvertMethod(IEntity entity) {
           return (T) entity;
        }
    }
}