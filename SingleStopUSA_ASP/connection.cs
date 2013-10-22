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

        private Guid _contactId;
        private Guid _incidentId;
        private Guid _noteId;
        private OrganizationService _orgService;
        
        #endregion Class Level Members

         /// <summary>
        /// The Run() method first connects to the Organization service. Afterwards,
        /// basic create, retrieve, update, and delete entity operations are performed.
        /// </summary>
        /// <param name="connectionString">Provides service connection information.</param>
        /// <param name="promptforDelete">When True, the user will be prompted to delete all
        /// created entities.</param>
        public void Run(String connectionString, Incident i, Contact c, Annotation a)
        {
            try
            {
                // Establish a connection to the organization web service using CrmConnection.
                Microsoft.Xrm.Client.CrmConnection connection = CrmConnection.Parse (connectionString);
                
                // Obtain an organization service proxy.
                // The using statement assures that the service proxy will be properly disposed.
                using (_orgService = new OrganizationService(connection) )
                {


                    Contact contact = c;

                    //Lookup by email address to see if we already have the contact in our database, if we dont then we add it.                   
                    //QueryByAttribute querybyattribute = new QueryByAttribute("contact"); //Create a query object
                    //querybyattribute.ColumnSet = new ColumnSet("guid"); //Add the column(s) you want returned
                    //querybyattribute.Attributes.AddRange("emailaddress1"); // Add which column you are searching
                    //querybyattribute.Values.AddRange(c.EMailAddress1); //Add what you are searching fo

                    // Create a column set to define which attributes should be retrieved.
                    //ColumnSet attributes = new ColumnSet(new string[] { "name", "ownerid" });

                    //contact = _orgService.Retrieve(contact.LogicalName, _contactId, attributes);

                    ////  Query passed to service proxy.
                    ////EntityCollection retrieved = _orgService.RetrieveMultiple(querybyattribute); //Make the search

                    //foreach (var e in retrieved.Entities)
                    //{

                    //    if (e.Attributes.Contains("emailaddress1"))
                    //    {
                    //        // _contactId = new Guid(e.Attributes["Guid"].ToString);
                    //    }
                    //    else
                    //    {
                            
                    //        {
                    //            //Add any additional fields for the contact here

                    //        };

                    //        _contactId = _orgService.Create(contact);
                    //    }
                    //}

                    _contactId = _orgService.Create(contact);

                    //Create the incident (case)
                    Incident incident = i;
                    {
                        //Add any additional case fields here
                        i.CustomerId = new EntityReference(Contact.EntityLogicalName, _contactId);
                    };
                  
                    _incidentId = _orgService.Create(incident);

                    //Create a note which we use to log the students initial question
                    Annotation note = a; 
                    {
                        //Add any additional fields for the note
                        a.Subject = "Question Description";
                        a.ObjectId = new EntityReference(Incident.EntityLogicalName, _incidentId);
                    };
                      
                    _noteId = _orgService.Create(note);

                    //Create an appointment 
                    //var appointment = new Appointment
                    //{
                    //    ScheduledStart = DateTime.Now,
                    //    ScheduledEnd = DateTime.Now.Add(new TimeSpan(0, 30, 0)),
                    //    Subject = "Sample 30-minute Appointment",
                    //    RegardingObjectId = new EntityReference(Incident.EntityLogicalName,
                    //        _incidentId)
                    //};
                    //Guid _appointmentId = _serviceProxy.Create(appointment);
            
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
        static public void createCase(Incident incident, Contact contact, Annotation note)
        {
            try
            {
                // Obtain connection configuration information for the Microsoft Dynamics
                // CRM organization web service.
                String connectionString = GetServiceConfiguration();

                if (connectionString != null)
                {
                    connection app = new connection();
                    app.Run(connectionString, incident, contact, note);
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }


        }
        
        #endregion Main method
    
    }
}