/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.IntSandbox.Customer.Sample
{
	using ResponseObjects;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;

	public static class Customer
	{

		/// <summary>
		/// Given the Customer Microsoft ID, and the Reseller Microsoft ID, this method will retrieve the customer cid that can be used to perform transactions on behalf of the customer using the partner APIs
		/// </summary>
		/// <param name="customerMicrosoftId">Microsoft ID of the customer, this is expected to be available to the reseller</param>
		/// <param name="resellerMicrosoftId">Microsoft ID of the reseller</param>
		/// <param name="sa_Token">unexpired access token to call the partner apis</param>
		/// <returns>customer cid that can be used to perform transactions on behalf of the customer by the reseller</returns>
		public static string GetCustomerCid(string customerMicrosoftId, string resellerMicrosoftId, string sa_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(
									string.Format(
											"https://api.cp.microsoft.com/customers/get-by-identity?provider=AAD&type=external_group&tid={0}&etid={1}",
											customerMicrosoftId, resellerMicrosoftId));

			request.Method = "GET";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);

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
					//Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// This method is used to get customer info in the Microsoft reseller ecosystem by the reseller
		/// </summary>
		/// <param param name="customerCid">cid of the customer whose information we are trying to retrieve</param>
		/// <param name="customer_Token">unexpired access token to access the partner apis</param>
		/// <returns>the created customer information: customer cid, customer microsoft id</returns>
		public static dynamic GetCustomer(string customerCid, string customer_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(string.Format("https://api.cp.microsoft.com/customers/{0}", customerCid));

			request.Method = "GET";
			request.ContentType = "application/json";
			request.Accept = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + customer_Token);

			try
			{
				//Utilities.PrintWebRequest(request, string.Empty);
				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					//Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
					var customer = JsonConvert.DeserializeObject(responseContent);
					return customer;
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

		/// <summary>
		/// This method is used to get all the customers for the reseller
		/// </summary>
		/// <param name="resellerCid"cid of the reseller></param>
		/// <param name="sa_Token">sales agent token</param>
		/// <returns>list of customers</returns>
		public static IList<AzureADContract> GetAllCustomers(string resellerCid, string sa_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(string.Format("https://graph.windows.net/{0}/contracts?api-version=1.6",
									resellerCid));

			request.Method = "GET";
			request.ContentType = "application/json";
			request.Accept = "application/json";

			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);

			try
			{
				//Utilities.PrintWebRequest(request, string.Empty);
				var response = request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					//Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
					JObject jo = JObject.Parse(responseContent);
					IList<JToken> results = jo["value"].Children().ToList();
					IList<AzureADContract> contracts = new List<AzureADContract>();

					foreach (JToken result in results)
					{
						AzureADContract contract = JsonConvert.DeserializeObject<AzureADContract>(result.ToString());
						contracts.Add(contract);
					}
					return contracts;
					//var c = JsonConvert.DeserializeObject(responseContent);
					//return Json.Decode(responseContent);
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
			return null;
		}

		/// <summary>
		/// Delete a customer account from the integration sandbox
		/// </summary>
		/// <param name="resellerCid">Cid of reseller</param>
		/// <param param name="customerCid">cid of the customer whose information we are trying to retrieve</param>
		/// <param name="sa_Token">sales agent token</param>
		public static void DeleteCustomer(string resellerCid, string customerCid, string sa_Token)
		{
			var request =
					(HttpWebRequest)
							HttpWebRequest.Create(
									string.Format("https://api.cp.microsoft.com/{0}/customers/delete-tip-reseller-customer",
											resellerCid));

			request.Method = "POST";
			request.ContentType = "application/json";

			request.Headers.Add("api-version", "2015-03-31");
			request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
			request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
			request.Headers.Add("Authorization", "Bearer " + sa_Token);

			string content = JsonConvert.SerializeObject(new { customer_id = customerCid });
			using (var writer = new StreamWriter(request.GetRequestStream()))
			{
				writer.Write(content);
			}

			try
			{
				var response = request.GetResponse();
			}
			catch (WebException webException)
			{
				using (var reader = new StreamReader(webException.Response.GetResponseStream()))
				{
					var responseContent = reader.ReadToEnd();
					Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
				}
			}
		}
	}
}