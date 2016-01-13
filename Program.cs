/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System;
using System.Configuration;

namespace Microsoft.Partner.CSP.IntSandbox.Customer.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			CmdLineArguments parsedArgs = new CmdLineArguments();
			if (Parser.ParseArgumentsWithUsage(args, parsedArgs))
			{
				// the parser only ensures the switch names are specified correctly - still need to validate
				if (parsedArgs.Validate())
				{
					if (parsedArgs.operation == Operation.DeleteAll && parsedArgs.confirm == false)
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.Write("\n\n\nAre you sure you wish to delete all customers?");
						Console.Write("\n(Enter 'Y' to delete)?");
						string confirmPrompt = Console.ReadLine();

						parsedArgs.confirm = (confirmPrompt.ToLowerInvariant() == "Y".ToLowerInvariant());
						if (!parsedArgs.confirm)
						{
							return;
						}
					}
				}
				else
				{
					return;
				}

				// This is the Microsoft ID of the reseller
				// Please work with your Admin Agent to get it from https://partnercenter.microsoft.com/en-us/pc/AccountSettings/TenantProfile
				string microsoftId = ConfigurationManager.AppSettings["MicrosoftId"];

				// This is the default domain of the reseller
				// Please work with your Admin Agent to get it from https://partnercenter.microsoft.com/en-us/pc/AccountSettings/TenantProfile
				string defaultDomain = ConfigurationManager.AppSettings["DefaultDomain"];

				// This is the appid that is registered for this application in Azure Active Directory (AAD)
				// Please work with your Admin Agent to get it from  https://partnercenter.microsoft.com/en-us/pc/ApiIntegration/Overview 
				string appId = ConfigurationManager.AppSettings["AppId"];

				// This is the key for this application in Azure Active Directory
				// This is only available at the time your admin agent has created a new app at https://partnercenter.microsoft.com/en-us/pc/ApiIntegration/Overview
				// You could alternatively goto Azure Active Directory and generate a new key, and use that.
				string key = ConfigurationManager.AppSettings["Key"];

				// Prompts the user to edit the config parametres if its not already done.
				Utilities.ValidateConfiguration(microsoftId, defaultDomain, appId, key);


				try
				{
					switch (parsedArgs.operation)
					{
						case Operation.Export:
							Orchestrator.ExportCustomers(parsedArgs.file, defaultDomain, appId, key, microsoftId);
							break;

						case Operation.Delete:
							Orchestrator.DeleteCustomers(parsedArgs.file, defaultDomain, appId, key, microsoftId);
							break;

						case Operation.DeleteAll:
							Orchestrator.DeleteAllCustomers(defaultDomain, appId, key, microsoftId);
							break;
						default:
							break;
					}
				}
				catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("\n Make sure the app.config has all the right settings.  The defaults in the app.config won't work."
							+ "\n If the settings are correct, its possible you are hitting a service error.  Try again."
							+ "\n If the error persists, contact support");
				}
				catch (Exception ex)
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("\n An unrecoverable error occurred:");
					Console.WriteLine(ex.Message);
				}

				Console.ResetColor();
				Console.Write("\n\n\nHit enter to exit the app...");
				Console.ReadLine();
			}
		}
	}
}
