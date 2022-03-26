using System;

namespace Nervestaple.WebApi.Models.Entities {

    /// <summary>
    /// Provides an attribute that indicates the annotated field should be
    /// used to create a link to the target resources.
    /// </summary> 
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class LinkedResource : Attribute {

        /// <summary>
        /// Name of the linked resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The linked resource.
        /// </summary>
        public string Resource { get; set; }
        
        /// <summary>
        /// Creates a new linked resource.
        /// <param name="name">Name of the linked resource</param>
        /// <param name="resource">Linked resource</param>
        /// </summary>
        public LinkedResource(string name, string resource) {
            Name = name;
            Resource = resource;
        }
    }
}