﻿using System;
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
            connection.createCase();
        }
    }
}