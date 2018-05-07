using ApiConnectionExample.Extensions;
using ApiConnectionExample.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ApiConnectionExample
{
    public class Connection
    {
        private readonly JsonConversion _jsonConversion;

        public Connection()
        {
            _jsonConversion = new JsonConversion();
        }

        /// <summary>
        /// Realiza una llamada GET
        /// </summary>
        /// <param name="uri">Url a la cual se debe llamar</param>
        /// <param name="hotLang">Es el lenguage especificado para esta solicitud</param>
        /// <typeparam name="T">Tipo de dato que se desea obtener</typeparam>
        /// <returns>Un objeto de tipo T</returns>
        public T GetCall<T>(string uri, string hotLang)
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create($"{Configurations.ResourcePath}{uri}");

                //Esta seccion se emplea para soportar la version 2.1 de TSL
                //actualmente si no se coloca esta parate no es posible consumir los servicios de PayPhone
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                ServicePointManager.DefaultConnectionLimit = 9999;

                request.Method = WebRequestMethods.Http.Get;
                request.Headers.Add("Authorization", "Bearer " + Configurations.Token);
                request.Headers.Add("Accept-Language", hotLang);

                StreamReader postResult;

                var response = request.GetResponse();

                postResult = new StreamReader(response.GetResponseStream());

                var responseForServer = postResult.ReadToEnd();

                var resultObject = _jsonConversion.JsonToObject<T>(responseForServer);

                response.Close();
                postResult.Close();

                return resultObject;
            }
            catch (WebException e)
            {
                throw e.ThrowPayPhoneException(_jsonConversion);
            }
            catch (Exception e)
            {
                throw e.ThrowException();
            }
        }

        /// <summary>
        /// Realiza una llamada POST
        /// </summary>
        /// <param name="uri">Url del post</param>
        /// <param name="postData">Datos del post a enviar</param>
        /// <param name="hotLang">Es el lenguaje especificado para esta solicitud en ISO 639-1 ejemplo "es"</param>
        /// <typeparam name="T">Tipo de dato que se desea recibir de vuelta</typeparam>
        /// <returns>Retorna un objeto del tipo T</returns>
        public T PostCall<T>(string uri, object postData, string hotLang)
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create($"{Configurations.ResourcePath}{uri}");

                //Esta seccion se emplea para soportar la version 2.1 de TSL
                //actualmente si no se coloca esta parate no es posible consumir los servicios de PayPhone
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                ServicePointManager.DefaultConnectionLimit = 9999;

                var json = _jsonConversion.ObjectToJson(postData);
                var byteData = Encoding.UTF8.GetBytes(json);

                request.Method = WebRequestMethods.Http.Post;
                request.Headers.Add("Authorization", "Bearer " + Configurations.Token);
                request.Headers.Add("Accept-Language", hotLang);

                request.ContentType = "application/json";
                request.ContentLength = byteData.Length;

                var reqStream = request.GetRequestStream();
                reqStream.Write(byteData, 0, byteData.Length);
                reqStream.Close();

                StreamReader postResult;

                var response = (HttpWebResponse)request.GetResponse();
                postResult = new StreamReader(response.GetResponseStream());

                var responseForServer = postResult.ReadToEnd();

                var resultObject = _jsonConversion.JsonToObject<T>(responseForServer);
                response.Close();
                postResult.Close();

                return resultObject;
            }
            catch (WebException e)
            {
                throw e.ThrowPayPhoneException(_jsonConversion);
            }
            catch (Exception e) {
                throw e.ThrowException();
            }
        }

        /// <summary>
        /// Obtiene el token de autenticación necesario para la comunicación con PayPhone
        /// </summary>
        public TokenResponseModel GetToken(string companyCode = null)
        {
            try
            {
                var request = HttpWebRequest.Create(Configurations.ResourcePath + "/token");
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var tokenRequest = new TokenRequestModel()
                {
                    client_id = Configurations.ClientId,
                    client_secret = Configurations.KeySecret,
                    company_code = companyCode,
                    grant_type = "client_credentials"
                };

                var postData = tokenRequest.ToString();
                var byteData = Encoding.UTF8.GetBytes(postData);

                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/json";
                request.ContentLength = byteData.Length;

                var reqStream = request.GetRequestStream();
                reqStream.Write(byteData, 0, byteData.Length);
                reqStream.Close();

                StreamReader postResult;

                var response = (HttpWebResponse)request.GetResponse();
                postResult = new StreamReader(response.GetResponseStream());

                var responseForServer = postResult.ReadToEnd();

                var resultObject = _jsonConversion.JsonToObject<TokenResponseModel>(responseForServer);
                response.Close();
                postResult.Close();

                return resultObject;
            }
            catch (WebException e)
            {
                throw e.ThrowPayPhoneException(_jsonConversion);
            }
            catch (Exception e)
            {
                throw e.ThrowException();
            }
        }


    }
}
