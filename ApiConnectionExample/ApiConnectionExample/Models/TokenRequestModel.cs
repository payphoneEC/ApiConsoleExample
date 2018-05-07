using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiConnectionExample.Models
{
    public class TokenRequestModel
    {
        /// <summary>
        /// clave de la aplicacion
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// Clave secreta de la aplicacion
        /// </summary>
        public string client_secret { get; set; }
        /// <summary>
        /// tipo de autenticacion 
        /// <example>client_credentials</example>
        /// </summary>
        public string grant_type { get; set; }
        /// <summary>
        /// Codigo unico de la compañía
        /// <example>RUC</example>
        /// </summary>
        public string company_code { get; set; }

        public override string ToString()
        {
            var props = typeof(TokenRequestModel).GetProperties();
            var result = "";
            var last = props[props.Length - 1];
            foreach (var propertyInfo in props)
            {
                if (!propertyInfo.Equals(last))
                {
                    result += propertyInfo.Name + "=" + propertyInfo.GetValue(this, null) + "&";
                }
                else
                {
                    result += propertyInfo.Name + "=" + propertyInfo.GetValue(this, null);
                }

            }

            return result;
        }
    }
}
