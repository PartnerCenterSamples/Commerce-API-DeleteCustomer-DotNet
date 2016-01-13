/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System;

namespace Microsoft.Partner.CSP.IntSandbox.Customer.Sample.ResponseObjects
{
	public class AzureADContract
	{
		public Guid customerContextId { get; set; }
		public string defaultDomainName { get; set; }
		public DateTime? deletionTimestamp { get; set; }
		public string displayName { get; set; }
		public string objectType { get; set; }
		public string objectId { get; set; }
		public string cspCustomerCid { get; set; }
	}
}
