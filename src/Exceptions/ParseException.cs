using System;
using System.Runtime.Serialization;

namespace Nervestaple.WebApi.Exceptions {

    /// <summary>
    /// Provides an exception that indicates the supplied data could not be correctly parsed.
    /// </summary>
    public class ParseException : Exception {
     
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ParseException() { }
     
        /// <summary>
        /// Creates a new instance.
        /// <param name="message">Message for this exception</param>
        /// </summary>
        public ParseException(string message) : base(message) { }
     
        /// <summary>
        /// Creates a new instance.
        /// <param name="message">Message for this exception</param>
        /// <param name="inner">The exception being wrapped</param>
        /// </summary>
        public ParseException(string message, Exception inner) : base(message, inner) { }
     
        /// <summary>
        /// Creates a new Instance.
        /// <param name="info">Serialization definition</param>
        /// <param name="context">Describes the stream being serialized</param>
        /// </summary>
        protected ParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}