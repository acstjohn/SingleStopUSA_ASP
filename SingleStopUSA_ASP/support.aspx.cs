using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//Use the CRM objects
using Microsoft.Xrm.Sdk;

namespace SingleStopUSA_ASP
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // This was an oler method that would return an authentication header so we could send requests via javascript. The header function works but the additional requests proved difficult and we stopped this approach.
            // headerID.Text = SingleStopUSA_ASP.authenticate.getAuthHeader();
        }

        protected void callCRM_Click(object sender, EventArgs e)
        {
            // This method runs when the form submit button is clicked and will start to build each crm object 
            // based on the form inputs then pass the crm entity collection to our code for processing
            
            DateTime currentDate = DateTime.Now;
            
            // sample - create the case crm object
            Contact contact = new Contact
            {
                FirstName = firstname.Text,
                LastName = lastname.Text,
                EMailAddress1 = email.Text,
                Telephone1 = homephone.Text,
                MobilePhone = mobilephone.Text
            };

            // sample - create the case (incident) crm object
            Incident incident = new Incident {
                Title = "Online Support Request: " + firstname.Text + " " + lastname.Text,
                CaseTypeCode = new OptionSetValue(Convert.ToInt32(type.SelectedItem.Value)),
                FollowupBy = currentDate.AddDays(7)
            };
              
            // sample - add a note to the case
            Annotation note = new Annotation
            {
                //This is the Note Subject we want to use
                Subject = "Question Description",
                NoteText = description.Text
            };

            // sample - add another note to the case
            Annotation note2 = new Annotation
            {
                //This is the Note Subject we want to use
                Subject = "Good contact times",
                NoteText = description.Text
            };

            // sample - add a task to the case
            Task task = new Task
            {
                 Subject = "Reach out to " + firstname.Text + " " + lastname.Text,
                 Description = "Via " + preferredContactMethod.SelectedItem.Text + "\n\n" + description.Text
            };

            // sample - add an appointment to the case
            Appointment appointment = new Appointment
            {
                ScheduledStart = currentDate,
                ScheduledEnd = currentDate.AddDays(2),
                ScheduledDurationMinutes = 60,
                Subject = "Contact " + firstname.Text + " " + lastname.Text + " via " + preferredContactMethod.SelectedValue,
                Description = subject.SelectedItem.Text
            };

            // Create the collection of entities that we will loop through, create, and then associate later. 
            // We can only have one incident but we can accept multiple contacts or notes.
            EntityCollection entities = new EntityCollection();
                entities.Entities.Add(contact);
                entities.Entities.Add(incident);
                entities.Entities.Add(note);
                entities.Entities.Add(note2);
                entities.Entities.Add(task);
                entities.Entities.Add(note2);
                entities.Entities.Add(appointment);
        
            // Send over the collection for processing
            CRMService.createCase(entities);

        }
    }
}