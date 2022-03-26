using Microsoft.AspNetCore.Mvc;

namespace Nervestaple.WebApi.Controllers {

    /// <summary>
    /// Provides a base controller all other Entity controllers may extend.
    /// </summary>
    [Route("[controller]")]
    public class EntityController : AbstractController {

    }
}