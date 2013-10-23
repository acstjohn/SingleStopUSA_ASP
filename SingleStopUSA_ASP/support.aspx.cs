using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SingleStopUSA_ASP
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //headerID.Text = SingleStopUSA_ASP.authenticate.getAuthHeader();
        }

        protected void callCRM_Click(object sender, EventArgs e)
        {
            Contact contact = new Contact
            {
                FirstName = firstname.Text,
                LastName = lastname.Text,
                EMailAddress1 = email.Text,
                Telephone1 = homephone.Text,
                MobilePhone = mobilephone.Text
            };

            Incident incident = new Incident {
                Title = "New Case: " + firstname.Text + " " + lastname.Text,
                //reference the subject ids already created in the system
               // SubjectId = subject.SelectedIte,
               // Description = description.Text
            };

            

            Annotation note = new Annotation
            {
                NoteText = description.Text
            };

            connection.createCase(incident, contact, note);

        }
    }
}