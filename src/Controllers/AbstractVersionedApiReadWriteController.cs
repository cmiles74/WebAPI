using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Nervestaple.EntityFrameworkCore.Models.Entities;
using Nervestaple.WebApi.Models.converters;
using Nervestaple.WebService.Services;

namespace Nervestaple.WebApi.Controllers {
    /// <summary>
    /// Provides default implementation of a read-write controller methods that
    /// can also handle a versioned WebAPI
    ///
    /// NOTE: This class expects the versioned API to adhere to the format
    /// `/api/v{version}/{controller}/{method}`, varying from this format
    /// will require custom URL and route handling.
    /// </summary>
    public abstract class AbstractVersionedApiReadWriteController<T, K> : AbstractVersionedApiReadOnlyController<T, K>,
        IVersionedApiReadWriteController<T, K>
        where T : Entity<K>
        where K : struct {

        /// <summary>
        /// The backing service for this controller
        /// </summary>
        protected new readonly IWebReadWriteService<T, K> Service;

        /// <summary>
        /// Creates a new instance.
        /// <param name="service">Backing service for this controller</param>
        /// </summary>
        public AbstractVersionedApiReadWriteController(IWebReadWriteService<T, K> service) : base(service) {
            Service = service;
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult HandleCreate(EditModel<T, K> model) {
            return HandleCreateAsync(model).Result;
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> HandleCreateAsync(EditModel<T, K> model) {
            var entity = await Service.CreateAsync(model);
            
            if (entity == null) {
                return BadRequest();
            }
            
            return new ObjectResult(
                HalResourceEntityConverter<T>.GetResource(Url, entity));
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult HandleCreate(T instance) {
            return HandleCreateAsync(instance).Result;
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> HandleCreateAsync(T instance) {
            var entity = await Service.CreateAsync(instance);
            
            if (entity == null) {
                return BadRequest();
            }
            
            return new ObjectResult(
                HalResourceEntityConverter<T>.GetResource(Url, entity));
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult HandleDelete(K id) {
            return HandleDeleteAsync(id).Result;
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> HandleDeleteAsync(K id) {
            var entity = await Service.GetByIdAsync(id);
            if (entity == null) {
                return NotFound();
            }

            await Service.DeleteAsync(id);
            return StatusCode(204);
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult HandleUpdatePost(K id, EditModel<T, K> model) {
            return HandleUpdatePostAsync(id, model).Result;
        }
        
        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<IActionResult> HandleUpdatePostAsync(K id, EditModel<T, K> model) {
            var entity = await Service.UpdateAsync(id, model);
            
            if (entity == null) {
                return BadRequest();
            }
            
            return new ObjectResult(
                HalResourceEntityConverter<T>.GetResource(Url, entity));
        }

        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult HandleUpdate(K id, JsonPatchDocument<AbstractEntity> model) {
            return HandleUpdateAsync(id, model).Result;
        }
        
        /// <inheritdoc/>
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> HandleUpdateAsync(K id, JsonPatchDocument<AbstractEntity> model) {
            var entity = await Service.UpdateAsync(id, model);
            
            if (entity == null) {
                return BadRequest();
            }
            
            return new ObjectResult(
                HalResourceEntityConverter<T>.GetResource(Url, entity));
        }
    }
}