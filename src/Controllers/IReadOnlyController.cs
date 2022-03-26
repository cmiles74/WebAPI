using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nervestaple.EntityFrameworkCore.Models.Entities;
using Nervestaple.EntityFrameworkCore.Models.Parameters;

namespace Nervestaple.WebApi.Controllers {

    /// <summary>
    /// Provides an interface that all read only services must implement.
    /// </summary>
    public interface IReadOnlyController<T, K> 
        where T: IEntity<K> 
        where K: struct {
        /// <summary>
        /// Returns a page of instances that match the provided page 
        /// parameters.
        /// </summary>
        /// <param name="parameters">page parameters, including size of page, 
        /// requested page and sort information</param>
        /// <returns>a page of instances</returns>
        IActionResult HandleGet([FromQuery] SimplePageParameters parameters);
        
        /// <summary>
        /// Returns a page of instances that match the provided page 
        /// parameters.
        /// </summary>
        /// <param name="parameters">page parameters, including size of page, 
        /// requested page and sort information</param>
        /// <returns>a page of instances</returns>
        Task<IActionResult> HandleGetAsync([FromQuery] SimplePageParameters parameters);

        /// <summary>
        /// Returns the instance with the matching unique identifier or a 404 if
        /// no match is found.
        /// </summary>
        /// <param name="id">unique identifier</param>
        /// <returns>matching instance or a 404 if not found</returns>
        IActionResult HandleGetById(K id);
        
        /// <summary>
        /// Returns the instance with the matching unique identifier or a 404 if
        /// no match is found.
        /// </summary>
        /// <param name="id">unique identifier</param>
        /// <returns>matching instance or a 404 if not found</returns>
        Task<IActionResult> HandleGetByIdAsync(K id);

        /// <summary>
        /// Provides a default implementation of the query method. For 
        /// uniformity, this should be presented as a POST method under
        /// the URL "query". 
        /// </summary>
        /// <param name="parameters">search parameters</param>
        /// <returns>a page of matching instances</returns>
        IActionResult HandleQuery(ISearchParameters<T, K> parameters);
        
        /// <summary>
        /// Provides a default implementation of the query method. For 
        /// uniformity, this should be presented as a POST method under
        /// the URL "query". 
        /// </summary>
        /// <param name="parameters">search parameters</param>
        /// <returns>a page of matching instances</returns>
        Task<IActionResult> HandleQueryAsync(ISearchParameters<T, K> parameters);
    }
}