using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        private Guid _appointmentId;
        private Guid _task;
        private OrganizationService _orgService;
        
        #endregion Class Level Members

         /// <summary>
        /// The method first connects to the Organization service. Afterwards,
        /// we create the contact, incident, and note and associate them
        /// </summary>
        /// <param name="connectionString">Provides service connection information.</param>

        //public void Run(String connectionString, Incident i, Contact c, Annotation a)
        public void Run(String connectionString, EntityCollection entities)
        {
            try
            {
                // Establish a connection to the organization web service using CrmConnection.
                Microsoft.Xrm.Client.CrmConnection connection = CrmConnection.Parse (connectionString);
                
                // Obtain an organization service proxy.
                // The using statement assures that the service proxy will be properly disposed.
                using (_orgService = new OrganizationService(connection) )
                {


                    for (int i = 0; i < entities.Entities.Count; i++)
                    {
                        if (entities.Entities.ElementAt(i).LogicalName == "contact")
                        {

                           // Contact contact = (contact)Convert(entity);  // .ToEntity("contact");
                            Contact contact = (Contact)entities.Entities.ElementAt(i);
                            if (contact.EMailAddress1 != "")
                            {
                                //Lookup by email address to see if we already have the contact in our database, if we dont then we add it.                   
                                QueryByAttribute querybyattribute = new QueryByAttribute("contact"); //Create a query object
                                querybyattribute.ColumnSet = new ColumnSet(new string[] { "firstname", "lastname", "ownerid" }); //Add the column(s) you want returned
                                querybyattribute.Attributes.AddRange("emailaddress1"); // Add which column you are searching
                                querybyattribute.Values.AddRange(contact.EMailAddress1); //Add what you are searching for

                                //  Query passed to service proxy.
                                EntityCollection retrieved = _orgService.RetrieveMultiple(querybyattribute); //Make the search


                                //if the contact doesnt exist then we create it, otherwise we pull the most recent contact and add it to the case
                                if (retrieved.Entities.Count == 0)
                                {
                                    _contactId = _orgService.Create(contact);
                                }
                                else
                                {
                                    //Return the last contact id
                                    _contactId = retrieved.Entities.ElementAt(retrieved.Entities.Count - 1).Id;
                                }
                            }
                            else
                            {
                                _contactId = _orgService.Create(contact);
                            }
                            

                        }
                        else if (entities.Entities.ElementAt(i).LogicalName == "incident")
                        {
                            //Create the incident (case)
                            Incident incident = (Incident)entities.Entities.ElementAt(i);
                            {
                                // Add any additional case fields here
                                incident.CaseOriginCode = new OptionSetValue(3);
                                // Assign the new or existing contact to the case
                                incident.CustomerId = new EntityReference(Contact.EntityLogicalName, _contactId);
                            };

                            _incidentId = _orgService.Create(incident);
                        }
                        else if (entities.Entities.ElementAt(i).LogicalName == "annotation")
                        {
                            //Create a note which we use to log the students initial question
                            Annotation note = (Annotation)entities.Entities.ElementAt(i);
                            {
                                // Add any additional fields for the note
                                // Assign the note tot the case
                                note.ObjectId = new EntityReference(Incident.EntityLogicalName, _incidentId);
                            };

                            _noteId = _orgService.Create(note);
                        }
                        else if (entities.Entities.ElementAt(i).LogicalName == "appointment")
                        {
                            //Create and appointment on the case
                            Appointment appointment = (Appointment)entities.Entities.ElementAt(i);
                            // Assign the appointment to the case
                            appointment.RegardingObjectId = new EntityReference(Incident.EntityLogicalName, _incidentId);
                            _appointmentId = _orgService.Create(appointment);
                        }
                        else if (entities.Entities.ElementAt(i).LogicalName == "task")
                        {
                            //Create and task on the case
                            Task task = (Task)entities.Entities.ElementAt(i);
                            // Assign the appointment to the case
                            task.RegardingObjectId = new EntityReference(Incident.EntityLogicalName, _incidentId);
                            _task = _orgService.Create(task);
                        }

                        
                    }

            
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
        /// Gets web service connection information from the web.config file.
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
        /// This is the method to be called from the asp page. It's expected that the entities will be built directly on the asp page and the process will take care of 
        /// default configuration so you can add/remove entity attributes from the asp page and not have to touch this code. 
        /// </summary>
        static public void createCase(EntityCollection entities)
        {
            try
            {
                //An incident can only have one contact and we are only building one incident at a time so we validate that each has a count one 1 before proceeding
                int incidentCount = 0;
                int contactCount = 0;
                for (int i = 0; i < entities.Entities.Count; i++)
                {
                    if (entities.Entities.ElementAt(i).LogicalName == "incident")
                    {
                        incidentCount ++;
                    }
                    else if (entities.Entities.ElementAt(i).LogicalName == "contact")
                    {
                        contactCount++;
                    }
                }

                if (incidentCount==1 || contactCount ==1)
                {
                    // Obtain connection configuration information for the Microsoft Dynamics
                    // CRM organization web service.
                    String connectionString = GetServiceConfiguration();

                    if (connectionString != null)
                    {
                        //Make the connection and then run the process
                        connection app = new connection();
                        app.Run(connectionString, entities);
                    }  
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