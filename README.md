![Continuous Integration](https://github.com/cmiles74/WebApi/actions/workflows/ci.yml/badge.svg)

# Nervestaple.WebAPI

This library leverages the [Nervestaple.WebService][0] project and 
[Microsoft's WebAPI library][2] to make it way easy to build out RESTful web 
services. It's available on [NuGet.org][3], you may follow the direction on
that page to add it to your project.

* [Nervestaple.WebApi NuGet Package][3]

```cs
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ToDoItemController : ToDoReadWriteController<ToDoItem, long>, IToDoItemController
{
    public ToDoItemController(IToDoItemService service) : base(service) {

    }
    
    [HttpGet]
    [SwaggerResponse(200, "Returning an page of item resources", 
        typeof(HalResource<IEnumerable<HalResource<ToDoItem>>>))]
    public IActionResult Get([FromQuery] SimplePageParameters parameters) {
        return base.handleGet(parameters);
    }

    [HttpGet("{id}")]
    [SwaggerResponse(200, "Returning the item with the matching unique identifier",
        typeof(HalResource<ToDoItem>))]
    public IActionResult GetById(int id) {
        return base.handleGetById(id);
    }

    [HttpPost("query")]
    [SwaggerResponse(200, "Returning a set of matching item instances",
        typeof(HalResource<IEnumerable<HalResource<ToDoItem>>>))]
    public IActionResult Query([FromBody] ToDoItemParameters parameters) {
        return handleQuery(parameters);
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] ToDoItemEdit model) {
        return base.handleCreate(model);
    }
    
    [HttpPatch("{id}")]
    public IActionResult Update(long id, [FromBody] JsonPatchDocument<AbstractEntity> model) {
        return base.handleUpdate(id, model);
    }
}
```

Note that while the example above has Swagger annotations, we haven't included
Swagger in this package. If you would like to use it, all you need to do is add
the packages to your project. `;-)`

```
<PackageReference Include="Swashbuckle.AspnetCore" Version="6.0.7" />
<PackageReference Include="Swashbuckle.AspnetCore.Annotations" Version="6.0.7" />
<PackageReference Include="Swashbuckle.AspnetCore.Swagger" Version="6.0.7" />
<PackageReference Include="Nervestaple.WebApi" Version="0.8.0" />
```

The controller above provides a RESTful API end point for adding and updating 
To Do items, including the ability to update them with [JsonPatchDocument][1]
objects.

You may use our startup helper classes to make it easier to get Swagger and
Swashbuckle configured for your application.

```cs
public class Startup {

  ...

  // configure MVC...
  MvcStartupHelper.ConfigureServices(
    services, 
    new ApiVersion(1, 0));  // default API version
  
  // configure your swagger options...
  services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
  
  // configure Swagger
  SwaggerStartupHelper.ConfigureServices(
    services, 
    Path.Combine(AppContext.BaseDirectory, "YOUR_PROJECT_NAME.xml"));
}
```

In the code above we configure the Swagger options with a custom class, 
`ConfigureSwaggerOptions`. Your options class should look something like the
example in `src\Helpers\SampleConfigureSwaggerOptions.cs`. 

At this time both Swagger, Swashbuckle and the supporting tooling require that 
you version your API, there's no getting around this requirement. If you really
don't want to version your API we suggest you use "v0" to indicate this.

Also note that you need to provide a path to your generated XML documentation
(`msbuild` will put this together for you); Swashbuckle will scrape this file
 to provide better API documentation. If you aren't using XML comments... 
 Well, you better start!
 
The last piece of the puzzle is to call out to the helper class in the 
`Configure` method of your `Startup.cs` file.

```cs
public void Configure(
            IConfiguration configuration,
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IApiVersionDescriptionProvider provider,
            YourDbContext dbContext) {
  ...

  SwaggerStartupHelper.ConfigureApplication(app, provider, path);
}
```

In order to support authentication, we build on the services and models 
provided by the Nervestaple.WebService project and provide an 
`AbstractAuthenticationController` that will pick up an `AccountService` from
your applications services and provided JWT bearer token authentication. It's 
as easy to use as...

```cs
public class AuthenticationController : AbstractAuthenticationController
{
    public AuthenticationController(IAccountService service, SecurityConfiguration securityConfiguration)
        : base(service, securityConfiguration)
    {
        
    }
}
```

There's also a canned requirement and handler to ensure that authenticated 
people are associated with a required set of roles, letting you setup your own
authorization policies...

```cs
services.AddAuthorization(options => {
    options.AddPolicy("Everyone", policy => {
        policy.Requirements.Add(new HasRoleRequirement("All Users"));
    });
});
```

That you can use to annotate your API methods, like so...

```cs
[Authorize(Policy = "Everyone")]
[HttpPost]
public IActionResult Create([FromBody] ToDoContextEdit model) {
    // you're code here!
}
```

We provide a helper that you may call at startup to setup JWT bearer 
authentication for your application.

```cs
JwtStartupHelper.ConfigureJwtBearerAuthentication(Configuration, services);
```

And there's a small bit of glue code to ensure that Swashbuckle correctly 
handles these annotations for Swagger.

```cs
services.AddSwaggerGen(s => {
     // your swashbuckle configuration...
     JwtStartupHelper.ConfigureSwaggerGenForJwt(s);
}); 
```

This library is a work in progress, please feel free to fork and send me pull
requests! `:-D`


## Documentation

This project uses [Doxygen](http://www.doxygen.nl/) for documentation. Doxygen 
will collect inline comments from your code, along with any accompanying README 
files, and create a documentation website for the project. If you do not have 
Doxygen installed, you can download it from their website and place it on your 
path. To run Doxygen...

    $ cd src
    $ doxygen

The documentation will be written to the `doc/html` folder at the root of the 
project, you can read this documentation with your web browser.

## Other Notes

Project Icon made by [Freepik](https://www.freepik.com/) from 
[Flaticon](https://www.flaticon.com/) under the 
[Creative Commons license](http://creativecommons.org/licenses/by/3.0/).

----

[0]: https://github.com/cmiles74/WebService
[1]: https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch
[2]: https://docs.microsoft.com/en-us/aspnet/core/web-api/
[3]: https://www.nuget.org/packages/Nervestaple.WebApi/