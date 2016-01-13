/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.IntSandbox.Customer.Sample
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using ResponseObjects;
	using System;
	using System.IO;
	using System.Net;

	public static class Reseller
	{
		/// <summary>
		/// Get the latest sales agent token given the AD Authorization Token
		/// </summary>
		/// <param name="adAuthorizationToken">AD Authorization Token</param>
		/// <returns>Latest sales agent token</returns>
		public static AuthorizationToken GetSA_Token(AuthorizationToken adAuthorizationToken)
		{
			CSPTokenResponse saToken = GetSA_Token(adAuthorizationToken.AccessToken);
			AuthorizationToken saAuthorizationToken = new AuthorizationToken(saToken.access_token, Convert.ToInt64(saToken.expires_in));
			return saAuthorizationToken;
		}

		/// <summary>
		/// Given the ad token this method retrieves the sales agent token for accessing any of the partner apis
		/// </summary>
		/// <param name="adToken">this is the access_token we get from AD</param>
		/// <returns>the sales agent token object which contains access_token, expiration duration</returns>
		private static CSPTokenResponse GetSA_Token(string adToken)
		{
			CSPTokenResponse result = null;

			var request = (HttpWebRequest)WebRequest.Create("https://api.cp.microsoft.com/my-org/tokens");

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + adToken);
			string content = "grant_type=client_credentials";

			using (var writer = new StreamWriter(request.GetRequestStream()))
			{
				writer.Write(content);
			}

			try
			{
				//Utilities.PrintWebRequest(request, content);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					result = JsonConvert.DeserializeObject<CSPTokenResponse>(responseContent);
					//Utilities.PrintWebResponse((HttpWebResponse)response, "<snipped>");
				}
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return result;
		}

		/// <summary>
		/// Get the latest AD token given the reseller domain and client credentials
		/// </summary>
		/// <param name="domain">domain of the reseller</param>
		/// <param name="clientId">clientID of the application</param>
		/// <param name="clientSecret">client secret of the application, also refered to as key</param>
		/// <returns>Latest AD Authorization token</returns>
		public static AuthorizationToken GetAD_Token(string domain, string clientId, string clientSecret)
		{
			AzureTokenResponse adToken = GetADToken(domain, clientId, clientSecret);
			AuthorizationToken adAuthorizationToken = new AuthorizationToken(adToken.access_token, Convert.ToInt64(adToken.expires_in));
			return adAuthorizationToken;
		}

		/// <summary>
		/// Given the reseller domain, clientid and clientsecret of the app, this method helps to retrieve the AD token
		/// </summary>
		/// <param name="resellerDomain">domain of the reseller including .onmicrosoft.com</param>
		/// <param name="clientId">AppId from the azure portal registered for this app</param>
		/// <param name="clientSecret">Secret from the azure portal registered for this app</param>
		/// <returns>this is the authentication token object that contains access_token, expiration time, can be used to get the authorization token from a resource</returns>
		private static AzureTokenResponse GetADToken(string resellerDomain, string clientId, string clientSecret)
		{
			AzureTokenResponse result = null;

			var request = WebRequest.Create(string.Format("https://login.microsoftonline.com/{0}/oauth2/token", resellerDomain));

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			string content = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&resource={2}", clientId, WebUtility.UrlEncode(clientSecret), WebUtility.UrlEncode("https://graph.windows.net"));

			using (var writer = new StreamWriter(request.GetRequestStream()))
			{
				writer.Write(content);
			}

			try
			{
				//Utilities.PrintWebRequest((HttpWebRequest)request, content);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					//Utilities.PrintWebResponse((HttpWebResponse)response, "<snipped>");
					result = JsonConvert.DeserializeObject<AzureTokenResponse>(responseContent);
				}

			}
			catch (WebException webException)
			{
				if (webException.Response != null)
				{
					using (var reader = new StreamReader(webException.Response.GetResponseStream()))
					{
						var responseContent = reader.ReadToEnd();
						Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// This method returns the CID of the reseller given the Microsoft ID and the sales agent token
		/// </summary>
		/// <param name="microsoftId">Microsoft ID of the reseller</param>
		/// <param name="sa_Token">sales agent token of the reseller</param>
		/// <returns>Reseller cid that is required to use the partner apis</returns>
		public static string GetCid(string resellerMicrosoftId, AuthorizationToken sa_Token)
		{
			var request = (HttpWebRequest)WebRequest.Create(string.Format("https://api.cp.microsoft.com/customers/get-by-identity?provider=AAD&type=tenant&tid={0}", resellerMicrosoftId));

			request.Method = "GET";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token.AccessToken);

			try
			{
				//Utilities.PrintWebRequest(request, string.Empty);

				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					//Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
					JObject crestResponse = JObject.Parse(responseContent);
					JToken idResponse = crestResponse["id"];
					return idResponse.ToString();
				}
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return string.Empty;
		}
	}
}
