using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiConnectionExample.Models
{
    /// <summary>
    /// Modelo para obtener el mensaje de error
    /// </summary>
    public class ErrorResponseModel
    {
        /// <summary>
        /// Mensaje de error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Codigo del error que corresponde
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Listado de errores
        /// </summary>
        public List<ErrorModel> Errors { get; set; }
    }

    /// <summary>
    /// Model error
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Current error message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Current error code
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// Current error description
        /// </summary>
        public string ErrorDescription { get; set; }
        /// <summary>
        /// List of all errores related to this error
        /// </summary>
        public string[] ErrorDescriptions { get; set; }
    }
}
