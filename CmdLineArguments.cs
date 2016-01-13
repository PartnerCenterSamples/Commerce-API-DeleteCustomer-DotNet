/********************************************************
*                                                        *
*   Copyright (C) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

using System;

namespace Microsoft.Partner.CSP.IntSandbox.Customer.Sample
{
	class CmdLineArguments
	{
		[Argument(ArgumentType.Required, ShortName = "op", HelpText = "Operation to perform")]
		public Operation operation;

		[Argument(ArgumentType.AtMostOnce, HelpText = "File to write (export) or read (DeleteSelected)")]
		public string file;

		[Argument(ArgumentType.AtMostOnce, HelpText = "Suppress confirmation prompt on delete operations; Overwrite file on Export")]
		public bool confirm;

		internal bool Validate()
		{
			if (operation == Operation.Export || operation == Operation.Delete)
			{
				if (String.IsNullOrEmpty(file))
				{
					Console.WriteLine("File is required for Export or Delete");
					return false;
				}
			}

			if (operation == Operation.Export)
			{
				if (System.IO.File.Exists(file))
				{
					if (confirm == false)
					{
						Console.WriteLine("File specified already exists");
						return false;
					}
				}
			}

			if (operation == Operation.Delete && !String.IsNullOrEmpty(file))
			{
				if (!System.IO.File.Exists(file))
				{
					Console.WriteLine("File specified does not exist");
					return false;
				}
			}

			return true;
		}
	}

	enum Operation
	{
		Export,
		Delete,
		DeleteAll
	}
}
