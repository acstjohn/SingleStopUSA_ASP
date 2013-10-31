<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="support.aspx.cs" Inherits="SingleStopUSA_ASP.index" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Single Stop USA Help</title>

    <script src="js/main.js"></script>
    <script type="text/javascript"  src="js/3p/json2.js"></script>
    <script type="text/javascript"  src="js/3p/jquery-1.9.1.min.js"></script>


</head>
<body>
    <h2>
                <asp:Image ID="Image1" runat="server" Height="150px" ImageUrl="~/img/logo-small-2-o.png" Width="155px" />
                <strong>&nbsp;Single Stop Support</strong></h2>
            <p>&nbsp;</p>


    <%
        if (IsPostBack)
        {
            contactForm.Visible = false;
            Response.Write("Thank you for reaching out to us " + firstname.Text + ". One of our representatives will reach out to you shortly");
        }
       %>

    <form id="contactForm" runat="server" method="post" >
    <div>
            
            <p>First Name: <asp:TextBox width="200" ID="firstname" runat="server"/></p>
            <p>Last Name: <asp:TextBox width="200" ID="lastname" runat="server"/></p>
            <p>Student ID: <asp:TextBox width="200" ID="studentid" runat="server"/></p>

            <p>Mobile Number: <asp:TextBox width="200" ID="mobilephone" runat="server"/></p>
            <p>Home Number: <asp:TextBox width="200" ID="homephone" runat="server"/></p>
            
            <p>E-mail Address: <asp:TextBox width="200" ID="email" runat="server"/></p>
            <p>School ID: <asp:TextBox width="200" ID="schoolid" runat="server"/></p>

            <p>
                Preferred Day of Contact: <select name="preferredday">
                <option>&nbsp;</option>
                <option value="sunday">Sunday</option>
                <option value="monday">Monday</option>
                <option value="tuesday">Tuesday</option>
                <option value="wednesday">Wednesday</option>
                <option value="thursday">Thursday</option>
                <option value="friday">Friday</option>
                <option value="saturday">Saturday</option>
            </select>
            </p>

            <p>Reason for Contacting Us:
                <asp:DropDownList ID="type" runat="server">
                    <asp:ListItem Text="Question" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Problem" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Request" Value="3"></asp:ListItem>
                </asp:DropDownList>
            </p>    
        
            <p>Preferred Contact Time:
                <asp:DropDownList ID="preferredContactTime" runat="server">
                    <asp:ListItem Text="Morning" Value="morning"></asp:ListItem>
                    <asp:ListItem Text="Lunchtime" Value="morning"></asp:ListItem>
                    <asp:ListItem Text="Afternoon" Value="morning"></asp:ListItem>
                    <asp:ListItem Text="Evening" Value="morning"></asp:ListItem>
                </asp:DropDownList>
            </p>
       
            <p>Preferred Method of Communication:
                <asp:DropDownList ID="preferredContactMethod" runat="server">
                    <asp:ListItem Text="Email" Value="email"></asp:ListItem>
                    <asp:ListItem Text="Call my mobile" Value="mobile"></asp:ListItem>
                    <asp:ListItem Text="Call my home phone" Value="home"></asp:ListItem>
                </asp:DropDownList>
            </p>
            
            <p>Subject: <br />
                <asp:ListBox ID="subject" runat="server">
                    <asp:ListItem value="Help with forms">Help with forms</asp:ListItem>
                    <asp:ListItem value="Help with taxes">Help with taxes</asp:ListItem>
                    <asp:ListItem value="Question about qualifications">Question about qualifications</asp:ListItem>
                    <asp:ListItem value="Other">Other</asp:ListItem>
                </asp:ListBox>
            </p>
            
            <p>Description: <br />
                <asp:TextBox width="300" height= "150" ID="description"  runat="server"/></p>
    

    </div>
     <asp:Button ID="callCRM" runat="server" OnClick="callCRM_Click" text="Submit Request"/>     
    </form>

    

</body>
</html>
