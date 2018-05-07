using ApiConnectionExample.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ApiConnectionExample.Extensions
{
    public static class ExceptionExtension
    {
        /// <summary>
        /// Se encarga de lanzar la excepcion correspondiente
        /// </summary>
        public static PayPhoneWebException ThrowPayPhoneException(this WebException webExc, JsonConversion json)
        {
            try
            {
                var postResult = new StreamReader(webExc.Response.GetResponseStream());
                var responseErrorForServer = postResult.ReadToEnd();
                var resultError = json.JsonToObject<ErrorResponseModel>(responseErrorForServer);
                var response = (HttpWebResponse)webExc.Response;
                postResult.Close();
                return new PayPhoneWebException(null, response.StatusCode.ToString(), resultError);
            }
            catch (Exception)
            {
                var response = (HttpWebResponse)webExc.Response;
                var listError = new List<ErrorModel>();
                var error = new ErrorResponseModel
                {
                    Message = webExc.Message
                };

                if (response != null)
                {
                    error.ErrorCode = response.StatusCode.ToString();
                    var err = new ErrorModel();
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Forbidden:
                            err.Message = "Url no disponible, por favor contacte con el soporte técnico.";
                            break;
                        default:
                            break;
                    }

                    error.Errors.Add(err);

                    return new PayPhoneWebException(null, response.StatusCode.ToString(), error);
                }

                return new PayPhoneWebException(null, "0", error);

            }
        }

        /// <summary>
        /// Lanza las excepciones generales
        /// </summary>
        /// <param name="exc"></param>
        public static PayPhoneWebException ThrowException(this Exception exc)
        {
            var listError = new List<ErrorResponseModel>();
            var error = new ErrorResponseModel
            {
                Message = exc.Message
            };
            listError.Add(error);
            return new PayPhoneWebException(null, "500", listError);
        }
    }
}
