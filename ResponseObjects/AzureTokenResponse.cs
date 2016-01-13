/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.Partner.CSP.IntSandbox.Customer.Sample.ResponseObjects
{
	class AzureTokenResponse
	{
		public string token_type;
		public string expires_in;
		public string expires_on;
		public string not_before;
		public string resource;
		public string access_token;
	}
}
