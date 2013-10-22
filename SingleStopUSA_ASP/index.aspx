<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SingleStopUSA_ASP.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Single Stop USA Help</title>

    <script src="js/main.js"></script>
    <script type="text/javascript"  src="js/3p/json2.js"></script>
    <script type="text/javascript"  src="js/3p/jquery-1.9.1.min.js"></script>


</head>
<body>
    
    
    
    <form id="contactForm" runat="server" onsubmit="alert('Thank you for your feedback, we will reach out to you shortly.');">
    <div>
    <p>
                First Name: <asp:TextBox width="65" ID="firstname" runat="server"/>
            </p>
            <p>
                Last Name: <asp:TextBox width="65" ID="lastname" runat="server"/>
            </p>
            <p>
                Student ID: <asp:TextBox width="65" ID="studentid" runat="server"/>
            </p>

            <p>Preferred Method of Communication</p>
            <input type="radio" name="prefcontact" value="prefphone"/>Phone<br />
            <input type="radio" name="prefcontact" value="prefemail"/>Email

            <p>
                Phone Number: <asp:TextBox width="65" ID="phone" runat="server"/>
            </p>
            <p>
                E-mail Address: <asp:TextBox width="65" ID="email" runat="server"/>
            </p>
            <p>
                School ID: <asp:TextBox width="65" ID="schoolid" runat="server"/>
            </p>
            <p>
                Campus ID: <asp:TextBox width="65" ID="campusid" runat="server"/>
            </p>

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


            <p>
                Preferred Time of Contact: <select name="hour">
                <option>&nbsp;</option>
                <option value="1">1</option>
                <option value="2">2</option>
                <option value="3">3</option>
                <option value="4">4</option>
                <option value="5">5</option>
                <option value="6">6</option>
                <option value="7">7</option>
                <option value="8">8</option>
                <option value="9">9</option>
                <option value="10">10</option>
                <option value="11">11</option>
                <option value="12">12</option>
            </select> : <select name="minute">
                <option>&nbsp;</option>
                <option value="00">00</option>
                <option value="15">15</option>
                <option value="30">30</option>
                <option value="45">45</option>
            </select> <select name="ampm">
                <option>&nbsp;</option>
                <option value="am">am</option>
                <option value="pm">pm</option>
            </select>
            </p>

            <p>
                Subject: <select name="subject">
                <option value="whatever">whatever</option>
            </select>
            </p>
            <p>
                Description:
                <asp:TextBox width="55" height= "300=" ID="description"  runat="server"/>
            </p>
    

    </div>
     <asp:Button ID="callCRM" runat="server" OnClick="callCRM_Click" text="Send via SDK"/>     
    </form>




</body>
</html>
