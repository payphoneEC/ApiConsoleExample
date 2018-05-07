using ApiConnectionExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ApiConnectionExample
{
    /// <summary>
    /// Custom exception
    /// </summary>
    public class PayPhoneWebException : Exception
    {
        /// <summary>
        /// constructor por defecto
        /// </summary>
        public PayPhoneWebException()
        : base()
        { }

        /// <summary>
        /// Constructor que recibe el status code
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <param name="errors"></param>
        public PayPhoneWebException(string message, string statusCode, List<ErrorResponseModel> errors)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorList = errors;
        }

        /// <summary>
        /// Constructor que recibe un mensaje
        /// </summary>
        /// <param name="message"></param>
        public PayPhoneWebException(string message)
        : base(message)
        { }

        /// <summary>
        /// Constructor con formato de mensaje
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public PayPhoneWebException(string format, params object[] args)
        : base(string.Format(format, args))
        { }

        /// <summary>
        /// Constructor con inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PayPhoneWebException(string message, Exception innerException)
        : base(message, innerException)
        { }

        /// <summary>
        /// Constructor con inner exception y formato de mensaje
        /// </summary>
        /// <param name="format"></param>
        /// <param name="innerException"></param>
        /// <param name="args"></param>
        public PayPhoneWebException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PayPhoneWebException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        { }
        /// <summary>
        /// 
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ErrorResponseModel> ErrorList { get; set; }
    }

    
}
