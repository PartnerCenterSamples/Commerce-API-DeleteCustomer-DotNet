/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using Microsoft.Partner.CSP.IntSandbox.Customer.Sample.ResponseObjects;
using System;
using System.IO;

namespace Microsoft.Partner.CSP.IntSandbox.Customer.Sample
{
	public class Orchestrator
	{
		static AuthorizationToken adAuthorizationToken;
		static AuthorizationToken saAuthorizationToken;
		static string resellerCid;

		/// <summary>
		/// Common routine to retrieve tokens for reseller
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		private static void GetTokens(string defaultDomain, string appId, string key, string resellerMicrosoftId)
		{
			// Get Active Directory token first
			adAuthorizationToken = Reseller.GetAD_Token(defaultDomain, appId, key);

			// Using the ADToken get the sales agent token
			saAuthorizationToken = Reseller.GetSA_Token(adAuthorizationToken);

			// Get the Reseller Cid, you can cache this value
			resellerCid = Reseller.GetCid(resellerMicrosoftId, saAuthorizationToken);
		}

		/// <summary>
		/// Exports all customers for the reseller to the specified file.
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="customerMicrosoftId">Microsoft Id of the customer</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		public static void ExportCustomers(string filename, string defaultDomain, string appId, string key, string resellerMicrosoftId)
		{
			GetTokens(defaultDomain, appId, key, resellerMicrosoftId);

			var adCustomers = Customer.GetAllCustomers(resellerMicrosoftId, adAuthorizationToken.AccessToken);

			using (StreamWriter outputFile = new StreamWriter(filename))
			using (var writer = new CsvHelper.CsvWriter(outputFile))
			{
				writer.WriteField("displayName");
				writer.WriteField("defaultDomainName");
				writer.WriteField("cspCustomerCid");
				writer.WriteField("customerContextId");
				writer.NextRecord();

				foreach (AzureADContract adCustomer in adCustomers)
				{
					adCustomer.cspCustomerCid = Customer.GetCustomerCid(adCustomer.customerContextId.ToString(), resellerMicrosoftId,
							saAuthorizationToken.AccessToken);

					writer.WriteField(adCustomer.displayName);
					writer.WriteField(adCustomer.defaultDomainName);
					writer.WriteField(adCustomer.cspCustomerCid);
					writer.WriteField(adCustomer.customerContextId);
					writer.NextRecord();
				}
			}
		}

		/// <summary>
		/// Delete customer from file. File must contain the CustomerCid from CSP
		/// </summary>
		/// <param name="filename">Path of csv file contain customers to delete</param>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		internal static void DeleteCustomers(string filename, string defaultDomain, string appId, string key, string resellerMicrosoftId)
		{
			GetTokens(defaultDomain, appId, key, resellerMicrosoftId);

			using (var reader = new CsvHelper.CsvReader(new StreamReader(filename)))
			{
                while (reader.Read())
                {
                    string displayName = reader.GetField<string>("displayName");
                    string cspCustomerCid = reader.GetField<string>("cspCustomerCid");
                    if (cspCustomerCid.Trim().Length == 0)
                        Console.WriteLine("Unable to delete {0} because CustomerCid is missing.", displayName);
                    else
                    { 
                        Console.WriteLine("Deleting {0}", displayName);
                        Customer.DeleteCustomer(resellerCid, cspCustomerCid, saAuthorizationToken.AccessToken);
                    }
				}

			}
		}

		/// <summary>
		/// Delete all customer from Integration Sandbox
		/// </summary>
		/// <param name="defaultDomain">default domain of the reseller</param>
		/// <param name="appId">appid that is registered for this application in Azure Active Directory (AAD)</param>
		/// <param name="key">Key for this application in Azure Active Directory</param>
		/// <param name="resellerMicrosoftId">Microsoft Id of the reseller</param>
		internal static void DeleteAllCustomers(string defaultDomain, string appId, string key, string resellerMicrosoftId)
		{
			GetTokens(defaultDomain, appId, key, resellerMicrosoftId);

			var adCustomers = Customer.GetAllCustomers(resellerMicrosoftId, adAuthorizationToken.AccessToken);

			foreach (AzureADContract adCustomer in adCustomers)
			{
				adCustomer.cspCustomerCid = Customer.GetCustomerCid(adCustomer.customerContextId.ToString(), resellerMicrosoftId,
						saAuthorizationToken.AccessToken);

				Console.WriteLine("Deleting {0}", adCustomer.displayName);
				Customer.DeleteCustomer(resellerCid, adCustomer.cspCustomerCid, saAuthorizationToken.AccessToken);
			}
		}

	}
}
