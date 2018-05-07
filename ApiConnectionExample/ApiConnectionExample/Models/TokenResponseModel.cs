using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiConnectionExample.Models
{
    public class TokenResponseModel
    {
        /// <summary>
        /// Token de autenticación
        /// </summary>
        public string Access_Token { get; set; }

        /// <summary>
        /// token de actualización
        /// </summary>
        public string Refresh_Token { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string UserName { get; set; }
    }
}
