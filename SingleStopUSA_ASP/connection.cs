using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


//<snippetSimplifiedConnection>
using System.Configuration;
using System.ServiceModel;

// These namespaces are found in the Microsoft.Crm.Sdk.Proxy.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Crm.Sdk.Messages;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

// These namespaces are found in the Microsoft.Xrm.Client.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;

namespace SingleStopUSA_ASP
{
    public class connection
    {
        #region Class Level Members

        private Guid _accountId;
        private Guid _contactId;
        private Guid _incidentId;
        private OrganizationService _orgService;
        
        #endregion Class Level Members

         /// <summary>
        /// The Run() method first connects to the Organization service. Afterwards,
        /// basic create, retrieve, update, and delete entity operations are performed.
        /// </summary>
        /// <param name="connectionString">Provides service connection information.</param>
        /// <param name="promptforDelete">When True, the user will be prompted to delete all
        /// created entities.</param>
        public void Run(String connectionString, Incident i, Contact c )
        {
            try
            {
                // Establish a connection to the organization web service using CrmConnection.
                Microsoft.Xrm.Client.CrmConnection connection = CrmConnection.Parse (connectionString);
                
                // Obtain an organization service proxy.
                // The using statement assures that the service proxy will be properly disposed.
                using (_orgService = new OrganizationService(connection) )
                {
                    //Create any entity records this sample requires.
                    CreateRequiredRecords();


                    // Instantiate an account object. Note the use of option set enumerations defined in OptionSets.cs.
                    // Refer to the Entity Metadata topic in the SDK documentation to determine which attributes must
                    // be set for each entity.
                  //  Account account = new Account { Name = "ASJ Enterprises" };
                  //  account.AccountCategoryCode = new OptionSetValue((int)AccountAccountCategoryCode.PreferredCustomer);
                  //  account.CustomerTypeCode = new OptionSetValue((int)AccountCustomerTypeCode.Investor);

                    // Create an account record named Fourth Coffee.
                 //   _accountId = _orgService.Create(account);

                    //ASJ
                    Contact contact = new Contact
                    {
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        EMailAddress1 = c.EMailAddress1
                    };

                    _contactId = _orgService.Create(contact);

                    Incident incident = new Incident {
                        Title = i.Title,
                        Description = i.Description,
                        CustomerId = new EntityReference(Contact.EntityLogicalName, _contactId)
                    };
                  
                    _incidentId = _orgService.Create(incident);



                    //Console.Write("{0} {1} created, ", account.LogicalName, account.Name);

                    // Retrieve the several attributes from the new account.
                 //   ColumnSet cols = new ColumnSet(
                //        new String[] { "name", "address1_postalcode", "lastusedincampaign" });

               //     Account retrievedAccount = (Account)_orgService.Retrieve("account", _accountId, cols);
                    //Console.Write("retrieved, ");

                    // Update the postal code attribute.
             //       retrievedAccount.Address1_PostalCode = "98052";

                    // The address 2 postal code was set accidentally, so set it to null.
          //          retrievedAccount.Address2_PostalCode = null;
//
                    // Shows use of a Money value.
          //          retrievedAccount.Revenue = new Money(5000000);

                    // Shows use of a Boolean value.
        //            retrievedAccount.CreditOnHold = false;

                    // Update the account record.
         //           _orgService.Update(retrievedAccount);
            
                }
            }

            // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
            {
                // You can handle an exception here or pass it back to the calling method.
                throw;
            }
        }

        #region Public Methods
        /// <summary>
        /// Creates any entity records this sample requires.
        /// </summary>
        public void CreateRequiredRecords()
        {
            // For this sample, all required entities are created in the Run() method.
        }

     

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets web service connection information from the app.config file.
        /// If there is more than one available, the user is prompted to select
        /// the desired connection configuration by name.
        /// </summary>
        /// <returns>A string containing web service connection configuration information.</returns>
        private static String GetServiceConfiguration()
        {
            // Get available connection strings from app.config.
            int count = ConfigurationManager.ConnectionStrings.Count;

            // Create a filter list of connection strings so that we have a list of valid
            // connection strings for Microsoft Dynamics CRM only.
            List<KeyValuePair<String, String>> filteredConnectionStrings = 
                new List<KeyValuePair<String, String>>();

            for (int a = 0; a < count; a++)
            {
                if (isValidConnectionString(ConfigurationManager.ConnectionStrings[a].ConnectionString))
                    filteredConnectionStrings.Add
                        (new KeyValuePair<string, string>
                            (ConfigurationManager.ConnectionStrings[a].Name,
                            ConfigurationManager.ConnectionStrings[a].ConnectionString));
            }

    
            if (filteredConnectionStrings.Count == 1)
            {
                return filteredConnectionStrings[0].Value;
            }

            return null;        
        }


        /// <summary>
        /// Verifies if a connection string is valid for Microsoft Dynamics CRM.
        /// </summary>
        /// <returns>True for a valid string, otherwise False.</returns>
        private static Boolean isValidConnectionString(String connectionString)
        {
            // At a minimum, a connection string must contain one of these arguments.
            if (connectionString.Contains("Url=") ||
                connectionString.Contains("Server=") ||
                connectionString.Contains("ServiceUri="))
                return true;

            return false;
        }
        
        #endregion Private Methods

        #region Main method

        /// <summary>
        /// Standard Main() method used by most SDK samples.
        /// </summary>
        /// <param name="args"></param>
        static public void createCase(Incident incident, Contact contact)
        {
            try
            {
                // Obtain connection configuration information for the Microsoft Dynamics
                // CRM organization web service.
                String connectionString = GetServiceConfiguration();

                if (connectionString != null)
                {
                    connection app = new connection();
                    app.Run(connectionString, incident, contact);
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }


        }
        
        static public void Main(string[] args)   
    {
            try
            {
                // Obtain connection configuration information for the Microsoft Dynamics
                // CRM organization web service.
                String connectionString = GetServiceConfiguration();

                if (connectionString != null)
                {
                  connection app = new connection();
                  //  app.Run(connectionString, true);
                }
            }

            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            
            // Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
            // SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            finally
            {
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
        #endregion Main method
    
    }
}