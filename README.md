# Commerce-API-DeleteCustomer-DotNet
Sample code for Cloud Solution Provider partners who are using the limited Integration Sandbox

After you compile this with Visual Studio you can use it to list your test customers in the CSP Integration Sandbox and to selectively delete them or to delete them all. Note: the delete operaton will not function in a CSP Production account so you will not lose any customer data.

First, you have to add your details for your CSP Integration Sandbox account to the App.Config file. This includes your AppID, your Client Secret, your MicrosoftID, and your DefaultDomain.

Second, you need to compile the sample in Visual Studio 2015 to create the command line tool.

Third, you run the commandline tool from a command prompt. By default the tool is called Microsot.Partner.CSP.IntSandbox.Customer.Sample.exe

##Operations

The tool can export a list of the customers in the integration sandbox to a CSV file, it can read the same CSV text file and delete any customers listed, and it can be asked to delete all customers. This is useful for text accounts in the integration sandbox where you need to remain under the customer limit for the sandbox.

##Here are the command line options:

/operation:{Export|Delete|DeleteAll}  Operation to perform (short form /op)

/file:<string>                        File to write (export) or read (DeleteSelected) (short form /f)

/confirm[+|-]                         Suppress confirmation prompt on delete operations; Overwrite file on Export (short form /c)

@<file>                               Read response file for more options
