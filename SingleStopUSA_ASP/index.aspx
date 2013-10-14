<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SingleStopUSA_ASP.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Single Stop USA Help</title>

    <script src="js/main.js"></script>
    <script type="text/javascript"  src="js/3p/json2.js"></script>
    <script type="text/javascript"  src="js/3p/jquery-1.9.1.min.js"></script>
    <script type="text/javascript"  src="js/3p/jquery.soap.js"></script>
    <script type="text/javascript"  src="js/3p/XrmSvcToolkit.js"></script>
    <script type="text/javascript"  src="js/3p/XrvSvcToolkit.Samples.createRecord.js  "></script>

<script>
        function jquerysoap()
        { 
            whoami(document.getElementById('headerID').value);
           // callSOAPWS(document.getElementById('headerID').value);
           // CreateEntity(document.getElementById('headerID').value, 'contact');
       }
</script>

<script>
    function jqueryajax() {
        //  whoami(document.getElementById('headerID').value);
        callSOAPWS(document.getElementById('headerID').value);
        // CreateEntity(document.getElementById('headerID').value, 'contact');
    }
</script>


<script>
    function oldCreate() {
         CreateEntity(document.getElementById('headerID').value, 'contact');
    }
</script>

<script>
function callSOAPWS(header)
{

    var soapServiceURL = 'https://singlestopusa.api.crm.dynamics.com/XRMServices/2011/Organization.svc?WSDL/';
    var soapMessage =
  '<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\"> ' +
            //  header +
              '<s:Body> ' +
              '  <Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"> ' +
              '  <request i:type="b:WhoAmIRequest" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\"> ' +
              '   <a:Parameters xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\" /> ' +
              '   <a:RequestId i:nil="true" />' +
              '   <a:RequestName>WhoAmI</a:RequestName>' +
              '  </request> ' +
              ' </Execute>' +
              '</s:Body> ' +
             '</s:Envelope>';



  alert("Check SOAP: [" + soapMessage + "]");

  jQuery.ajax({
          url: soapServiceURL,
          type: "POST",
          dataType: "xml",
          data: soapMessage,
          contentType: "application/soap+xml; charset=\"UTF-8\"",
         // headers: {
         //     SOAPAction: 'http://schemas.microsoft.com/crm/2007/WebServices/Execute',
         //     contentType: "application/soap+xml; charset=UTF-8"
         // },
          beforeSend: function (xhr) {
              xhr.setRequestHeader('SOAPAction', 'http://schemas.microsoft.com/crm/2007/WebServices/Execute');

          },
          //processData: false,   // what is it for? may be should be true when using 'complete:' ?
          //timeout: 5000,

          // below I first try to have only 'complete:' then I tried to have 'success:' + 'error:', then the 3. Nothing seems to be ok. I do not find which one i should use.
          //complete: myCallback,

          success: function( response ){
              //document.getElementById('debug').innerHTML = document.getElementById('debug').innerHTML + '\n' + 'success!' + '\n';
              alert("success!!!");
              return true;
          },

          error: function(XMLHttpRequest,textStatus, errorThrown){
             // document.getElementById('debug').innerHTML = document.getElementById('debug').innerHTML + '\n' + 'error : ' + textStatus + '\n';
              alert("error : ");
          }

  });

  alert('if we reach this line, is it a fail?!');
  return false;
}


</script>


  <script>
function whoami(header) {
 
   // alert(header);

    $.soap({
        url: 'https://singlestopusa.api.crm.dynamics.com/XRMServices/2011/Organization.svc?WSDL/',
        method: 'Execute',
        soap12: true,
        headers: {
            SOAPAction: 'http://schemas.microsoft.com/crm/2007/WebServices/Execute',
            contentType: "application/soap+xml; charset=\"UTF-8\""
        },

        params: '<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\"> ' +
             // header +
              '<s:Body> ' +
              '  <Execute xmlns="\http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"> ' +
              '  <request i:type="b:WhoAmIRequest" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\" xmlns:b=\"http://schemas.microsoft.com/crm/2011/Contracts\"> ' +
              '   <a:Parameters xmlns:c=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\" /> ' +
              '   <a:RequestId i:nil="true" />' +
              '   <a:RequestName>WhoAmI</a:RequestName>' +
              '  </request> ' +
              ' </Execute>' +
              '</s:Body> ' +
             '</s:Envelope>',

        params: header ,



        request: function (SOAPRequest) {
            //alert(SOAPRequest.toString());
        },
        success: function (soapResponse) {
            // do stuff with soapResponse
            // if you want to have the response as JSON use soapResponse.toJSON();
            // or soapResponse.toString() to get XML string
            // or soapResponse.toXML() to get XML DOM
            alert(soapResponse.toString());
        },
        error: function (SOAPResponse) {
            // show error
            alert(SOAPResponse.toString());
        }
    });
}
    </script>

</head>
<body>
    
    
    
    <form id="contactForm" runat="server" action="javascript:CreateContactNew()">
    <div>
    <p>

                First Name: <input type="text" size="65" name="firstname"/>
            </p>
            <p>
                Last Name: <input type="text" size="65" name="lastname"/>
            </p>
            <p>
                Student ID: <input type="text" size="65" name="studentid"/>
            </p>

            <p>Preferred Method of Communication</p>
            <input type="radio" name="prefcontact" value="prefphone"/>Phone<br />
            <input type="radio" name="prefcontact" value="prefemail"/>Email

            <p>
                Phone Number: <input type="text" size="65" name="phone"/>
            </p>
            <p>
                E-mail Address: <input type="text" size="65" name="email"/>
            </p>
            <p>
                School ID: <input type="text" size="65" name="schoolid"/>
            </p>
            <p>
                Campus ID: <input type="text" size="65" name="campusid"/>
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
                <textarea cols="55" name="description">  </textarea>
            </p>
            <p>
               <%-- <input type = "hidden" id ="header" runat="server" value ='@{SingleStopUSA_ASP.authenticate.getAuthHeader();}'/>--%>
               <%-- <input type = "hidden" id ="header" runat="server" value =\'' <% Response.Write(SingleStopUSA_ASP.authenticate.getAuthHeader()); %>  '\'/>--%>
           <%--     <input type="submit"  value="Submit to Support" name="submit"/>--%>

            </p>
  <%--  <asp:TextBox id="headerID" runat ="server" Visible="true" />
         <asp:HiddenField id="headerId2" runat="server" value='<%=SingleStopUSA_ASP.authenticate.getAuthHeader());%>'/>
    </div>--%>
            
    </form>
 <%--  <%Response.Write(SingleStopUSA_ASP.authenticate.getAuthHeader());%>--%>

   <%--  <%=SingleStopUSA_ASP.authenticate.getAuthHeader();%>--%>
    <button name="sendSDK" onclick=<%SingleStopUSA_ASP.connection.createCase();%> type="submit"> Send via SDK</button>
    <button name="send" onclick="jquerysoap()" type="submit"> Jquery Soap plugin</button>
    <button name="sendDemo" onclick="jqueryajax()" type="submit">Jquery Ajax</button>
    <button name="sendOld" onclick="oldCreate()" type="submit"> Old Function</button>
    <button name="sendXRM" onclick="onLoadCreateDemo()" type="submit">XRM Function</button>

</body>
</html>
