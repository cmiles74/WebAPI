using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Nervestaple.EntityFrameworkCore.Models.Entities;

namespace Nervestaple.WebApi.Controllers {
    /// <summary>
    /// Provides an interface that all read/write services must implement.
    /// </summary>
    public interface IReadWriteController<T, K> : IReadOnlyController<T, K>
        where T: Entity<K> 
        where K: struct 
    {
        /// <summary>
        /// Returns a new entity populated with the provided data.
        /// </summary>
        /// <param name="model">Data object used to populate the new Entity</param>
        IActionResult HandleCreate(EditModel<T, K> model);
        
        /// <summary>
        /// Returns a new entity populated with the provided data.
        /// </summary>
        /// <param name="model">Data object used to populate the new Entity</param>
        Task<IActionResult> HandleCreateAsync(EditModel<T, K> model);
        
        /// <summary>
        /// Creates the provided entity
        ///
        /// You may delegate to this method to create the new instance but you
        /// must provide the response yourself in order to provide
        /// comprehensible routing information.
        /// </summary>
        /// <param name="instance">Entity instance to create</param>
        /// <returns>the new instance</returns>
        IActionResult HandleCreate(T instance);
        
        /// <summary>
        /// Creates the provided entity
        ///
        /// You may delegate to this method to create the new instance but you
        /// must provide the response yourself in order to provide
        /// comprehensible routing information.
        /// </summary>
        /// <param name="instance">Entity instance to create</param>
        /// <returns>the new instance</returns>
        Task<IActionResult> HandleCreateAsync(T instance);
        
        /// <summary>
        /// Returns a current copy of the entity after updating the Entity with
        /// the matching unique identifier with the values provided in the model
        ///
        /// You may delegate to this method to create the new instance but you
        /// must provide the response yourself in order to provide
        /// comprehensible routing information.
        /// </summary>
        /// <param name="id">Unique ID of the Entity to update</param>
        /// <param name="model">Data object used to update the Entity</param>
        /// <returns>the updated instance</returns>
        IActionResult HandleUpdatePost(K id, EditModel<T, K> model);
        
        /// <summary>
        /// Returns a current copy of the entity after updating the Entity with
        /// the matching unique identifier with the values provided in the model
        ///
        /// You may delegate to this method to create the new instance but you
        /// must provide the response yourself in order to provide
        /// comprehensible routing information.
        /// </summary>
        /// <param name="id">Unique ID of the Entity to update</param>
        /// <param name="model">Data object used to update the Entity</param>
        /// <returns>the updated instance</returns>
        Task<IActionResult> HandleUpdatePostAsync(K id, EditModel<T, K> model);
        
        /// <summary>
        /// Returns a current copy of the entity after updating the Entity with
        /// the matching unique identifier with the values provided in the model
        ///
        /// You may delegate to this method to create the new instance but you
        /// must provide the response yourself in order to provide
        /// comprehensible routing information.
        /// </summary>
        /// <param name="id">Unique ID of the Entity to update</param>
        /// <param name="model">Data object used to update the Entity</param>
        /// <returns>the updated instance</returns>
        IActionResult HandleUpdate(K id, JsonPatchDocument<AbstractEntity> model);
        
        /// <summary>
        /// Returns a current copy of the entity after updating the Entity with
        /// the matching unique identifier with the values provided in the model
        ///
        /// You may delegate to this method to create the new instance but you
        /// must provide the response yourself in order to provide
        /// comprehensible routing information.
        /// </summary>
        /// <param name="id">Unique ID of the Entity to update</param>
        /// <param name="model">Data object used to update the Entity</param>
        /// <returns>the updated instance</returns>
        Task<IActionResult> HandleUpdateAsync(K id, JsonPatchDocument<AbstractEntity> model);

        /// <summary>
        /// Delete the entity with the matching unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the instance to delete</param>
        IActionResult HandleDelete(K id);
        
        /// <summary>
        /// Delete the entity with the matching unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the instance to delete</param>
        Task<IActionResult> HandleDeleteAsync(K id);
    }
}