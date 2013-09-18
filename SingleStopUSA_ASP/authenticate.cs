﻿using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Web;

namespace SingleStopUSA_ASP
{
    public class authenticate
    {

        // USE AT YOUR OWN RISK. 
        // PLEASE BE AWARE THAT ANY INFORMATION YOU MAY FIND HERE MAY BE INACCURATE, MISLEADING, DANGEROUS, ADDICTIVE, UNETHICAL OR ILLEGAL (as-is from Wikipedia!)

        // This example retrives data from CRM Online using pure SOAP calls only and no additional assemblies to illustrate the underlying SOAP interactions.
        // It is useful if you're planning to interact with CRM Online web services from a non-.NET environment.
        // The soap messages were based on Fiddler (http://www.fiddler2.com/) traffic capture of sample code from the CRM 2011 SDK (http://msdn.microsoft.com/en-us/library/gg309408.aspx).

        // This may look like a lot of code because it's completely done using raw SOAP calls and no C# wrappers or proxies. 
        // For optimal source code, please use the .NET assemblies that ship with CRM SDK or a wrapper around these methods in the programming language of your choice.
  
        
        public static string getAuthHeader()
        {

            string Username = "JPMorgan2@singlestopusa.org";
            string Password = "JPMCSingle$top";
            string CRMUrl = "https://singlestopusa.api.crm.dynamics.com/XRMServices/2011/Organization.svc";

            string crmSoapRequestHeader = "";

            #region Step 0: Get URN address and STS Enpoint dynamically from WSDL

            string WSDL = GetMethod(CRMUrl + "?wsdl");
            string WSDLImportURL = GetValueFromXML(WSDL, @"//*[local-name()='import' and namespace-uri()='http://schemas.xmlsoap.org/wsdl/']/@location");
            string WSDKImport = GetMethod(WSDLImportURL);
            string URNAddress = GetValueFromXML(WSDKImport, @"//*[local-name()='AuthenticationPolicy' and namespace-uri()='http://schemas.microsoft.com/xrm/2011/Contracts/Services']/*[local-name()='SecureTokenService' and namespace-uri()='http://schemas.microsoft.com/xrm/2011/Contracts/Services']//*[local-name()='AppliesTo' and namespace-uri()='http://schemas.microsoft.com/xrm/2011/Contracts/Services']/text()");
            string STSEnpoint = GetValueFromXML(WSDKImport, @"//*[local-name()='Issuer' and namespace-uri()='http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702']/*[local-name()='Address' and namespace-uri()='http://www.w3.org/2005/08/addressing']/text()");

            #endregion

            #region Step 1: Determine which authentication method (LiveID or OCP) and authenticate to get tokens and key.
            string keyIdentifier = null;
            string securityToken0 = null;
            string securityToken1 = null;
            if ((STSEnpoint != null) && (STSEnpoint.StartsWith("https://login.live.com")))
            {
                #region WLID Authenciation
                #region Step A: Get Windows Live Device Credentials

                // Please note that this step uses a modified version of DeviceIdManager class from the one that ships with the SDK. 
                // The modifications have been made to remove any writes to local disk which unfortunately removes the caching of device id as well. 

                // Generates random credentials (Username, Password & Application ID) for the device 
                // Sends the credentials to WLID and gets a PUID back
                DeviceIdManager.DeviceCredentials deviceCredentials = DeviceIdManager.RegisterDevice();

                #endregion

                #region Step B: Register Device Credentials and get binaryDAToken
                string deviceCredentialsSoapTemplate = @"<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope""
                xmlns:a=""http://www.w3.org/2005/08/addressing""
                xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">

                    <s:Header>
                    <a:Action s:mustUnderstand=""1"">
                    http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</a:Action>
                    <a:MessageID>
                    urn:uuid:{4:MessageID}</a:MessageID>
                    <a:ReplyTo>
                        <a:Address>
                        http://www.w3.org/2005/08/addressing/anonymous</a:Address>
                    </a:ReplyTo>
                    <VsDebuggerCausalityData xmlns=""http://schemas.microsoft.com/vstudio/diagnostics/servicemodelsink"">
                    uIDPoy9Ez+P/wJdOhoN2XNauvYcAAAAAK0Y6fOjvMEqbgs9ivCmFPaZlxcAnCJ1GiX+Rpi09nSYACQAA</VsDebuggerCausalityData>
                    <a:To s:mustUnderstand=""1"">
                    {4:STSEndpoint}</a:To>
                    <o:Security s:mustUnderstand=""1""
                    xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                        <u:Timestamp u:Id=""_0"">
                        <u:Created>{0:timeCreated}Z</u:Created>
                        <u:Expires>{1:timeExpires}Z</u:Expires>
                        </u:Timestamp>
                        <o:UsernameToken u:Id=""devicesoftware"">
                        <o:Username>{2:deviceUserName}</o:Username>
                        <o:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">{3:devicePassword}</o:Password>
                        </o:UsernameToken>
                    </o:Security>
                    </s:Header>
                    <s:Body>
                    <t:RequestSecurityToken xmlns:t=""http://schemas.xmlsoap.org/ws/2005/02/trust"">
                        <wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"">
                        <a:EndpointReference>
                            <a:Address>http://passport.net/tb</a:Address>
                        </a:EndpointReference>
                        </wsp:AppliesTo>
                        <t:RequestType>
                        http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType>
                    </t:RequestSecurityToken>
                    </s:Body>
                </s:Envelope>
                ";

                DateTime binaryDARequestCreatedTime = DateTime.Now.ToUniversalTime();
                string binaryDATokenXML = GetSOAPResponse(STSEnpoint, string.Format(deviceCredentialsSoapTemplate
                    , binaryDARequestCreatedTime.ToString("s") + "." + binaryDARequestCreatedTime.Millisecond
                    , binaryDARequestCreatedTime.AddMinutes(5.0).ToString("s") + "." + binaryDARequestCreatedTime.Millisecond
                    , "11" + deviceCredentials.DeviceName, deviceCredentials.Password, Guid.NewGuid().ToString(), STSEnpoint));
                string binaryDAToken = GetValueFromXML(binaryDATokenXML, @"//*[local-name()='CipherValue']/text()");

                #endregion

                #region Step C: Get Security Token by sending WLID username, password and device binaryDAToken

                string securityTokenSoapTemplate = @"
                <s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope""
                xmlns:a=""http://www.w3.org/2005/08/addressing""
                xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <s:Header>
                    <a:Action s:mustUnderstand=""1"">
                    http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</a:Action>
                    <a:MessageID>
                    urn:uuid:{5:MessageID}</a:MessageID>
                    <a:ReplyTo>
                      <a:Address>
                      http://www.w3.org/2005/08/addressing/anonymous</a:Address>
                    </a:ReplyTo>
                    <VsDebuggerCausalityData xmlns=""http://schemas.microsoft.com/vstudio/diagnostics/servicemodelsink"">
                    uIDPozBEz+P/wJdOhoN2XNauvYcAAAAAK0Y6fOjvMEqbgs9ivCmFPaZlxcAnCJ1GiX+Rpi09nSYACQAA</VsDebuggerCausalityData>
                    <a:To s:mustUnderstand=""1"">
                    {7:STSEndpoint}</a:To>
                    <o:Security s:mustUnderstand=""1""
                    xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                      <u:Timestamp u:Id=""_0"">
                        <u:Created>{0:timeCreated}Z</u:Created>
                        <u:Expires>{1:timeExpires}Z</u:Expires>
                      </u:Timestamp>
                      <o:UsernameToken u:Id=""user"">
                        <o:Username>{2:UserName}</o:Username>
                        <o:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">
                        {3:Password}</o:Password>
                      </o:UsernameToken>
                      <wsse:BinarySecurityToken ValueType=""urn:liveid:device""
                      xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                        <EncryptedData Id=""BinaryDAToken0""
                        Type=""http://www.w3.org/2001/04/xmlenc#Element""
                        xmlns=""http://www.w3.org/2001/04/xmlenc#"">
                          <EncryptionMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#tripledes-cbc"">
                          </EncryptionMethod>
                          <ds:KeyInfo xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">
                            <ds:KeyName>http://Passport.NET/STS</ds:KeyName>
                          </ds:KeyInfo>
                          <CipherData>
                            <CipherValue>
                                {4:BinaryDAToken}
                            </CipherValue>
                          </CipherData>
                        </EncryptedData>
                      </wsse:BinarySecurityToken>
                    </o:Security>
                  </s:Header>
                  <s:Body>
                    <t:RequestSecurityToken xmlns:t=""http://schemas.xmlsoap.org/ws/2005/02/trust"">
                      <wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"">
                        <a:EndpointReference>
                          <a:Address>{6:URNAddress}</a:Address>
                        </a:EndpointReference>
                      </wsp:AppliesTo>
                      <wsp:PolicyReference URI=""MBI_FED_SSL""
                      xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"" />
                      <t:RequestType>
                      http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType>
                    </t:RequestSecurityToken>
                  </s:Body>
                </s:Envelope>
                ";
                DateTime securityTokenRequestCreatedTime = DateTime.Now.ToUniversalTime();
                string securityTokenXML = GetSOAPResponse(STSEnpoint, string.Format(securityTokenSoapTemplate
                    , securityTokenRequestCreatedTime.ToString("s") + "." + securityTokenRequestCreatedTime.Millisecond
                    , securityTokenRequestCreatedTime.AddMinutes(5.0).ToString("s") + "." + securityTokenRequestCreatedTime.Millisecond
                    , Username, Password, binaryDAToken
                    , Guid.NewGuid().ToString(), URNAddress, STSEnpoint));
                securityToken0 = GetValueFromXML(securityTokenXML, @"//*[local-name()='CipherValue']/text()");
                securityToken1 = GetValueFromXML(securityTokenXML, @"//*[local-name()='CipherValue']/text()", 1);
                keyIdentifier = GetValueFromXML(securityTokenXML, @"//*[local-name()='KeyIdentifier']/text()");

                #endregion
                #endregion
            }
            else
            {
                #region OCP Authentication
                #region Step A: Get Security Token by sending OCP username, password

                string securityTokenSoapTemplate = @"
                <s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope""
	                xmlns:a=""http://www.w3.org/2005/08/addressing""
	                xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
	                <s:Header>
		                <a:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue
		                </a:Action>
		                <a:MessageID>urn:uuid:{4:MessageID}
		                </a:MessageID>
		                <a:ReplyTo>
			                <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
		                </a:ReplyTo>
		                <VsDebuggerCausalityData
			                xmlns=""http://schemas.microsoft.com/vstudio/diagnostics/servicemodelsink"">uIDPo4TBVw9fIMZFmc7ZFxBXIcYAAAAAbd1LF/fnfUOzaja8sGev0GKsBdINtR5Jt13WPsZ9dPgACQAA
		                </VsDebuggerCausalityData>
		                <a:To s:mustUnderstand=""1"">{6:STSEndpoint}
		                </a:To>
		                <o:Security s:mustUnderstand=""1""
			                xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
			                <u:Timestamp u:Id=""_0"">
				                <u:Created>{0:timeCreated}Z</u:Created>
				                <u:Expires>{1:timeExpires}Z</u:Expires>
			                </u:Timestamp>
			                <o:UsernameToken u:Id=""uuid-14bed392-2320-44ae-859d-fa4ec83df57a-1"">
				                <o:Username>{2:UserName}</o:Username>
				                <o:Password
					                Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">{3:Password}</o:Password>
			                </o:UsernameToken>
		                </o:Security>
	                </s:Header>
	                <s:Body>
		                <t:RequestSecurityToken xmlns:t=""http://schemas.xmlsoap.org/ws/2005/02/trust"">
			                <wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"">
				                <a:EndpointReference>
					                <a:Address>{5:URNAddress}</a:Address>
				                </a:EndpointReference>
			                </wsp:AppliesTo>
			                <t:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue
			                </t:RequestType>
		                </t:RequestSecurityToken>
	                </s:Body>
                </s:Envelope>
                ";

                DateTime securityTokenRequestCreatedTime = DateTime.Now.ToUniversalTime();
                string securityTokenXML = GetSOAPResponse(STSEnpoint, string.Format(securityTokenSoapTemplate
                    , securityTokenRequestCreatedTime.ToString("s") + "." + securityTokenRequestCreatedTime.Millisecond
                    , securityTokenRequestCreatedTime.AddMinutes(5.0).ToString("s") + "." + securityTokenRequestCreatedTime.Millisecond
                    , Username, Password
                    , Guid.NewGuid().ToString(), URNAddress, STSEnpoint));
                securityToken0 = GetValueFromXML(securityTokenXML, @"//*[local-name()='CipherValue']/text()");
                securityToken1 = GetValueFromXML(securityTokenXML, @"//*[local-name()='CipherValue']/text()", 1);
                keyIdentifier = GetValueFromXML(securityTokenXML, @"//*[local-name()='KeyIdentifier']/text()");

                #endregion
                #endregion
            }
            #endregion

            #region Step 2: Get / Set CRM Data by sending FetchXML Query
            crmSoapRequestHeader = @"
                <s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope""
                xmlns:a=""http://www.w3.org/2005/08/addressing""
                xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <s:Header>
                    <a:Action s:mustUnderstand=""1"">
                    http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Create</a:Action>
                    <a:MessageID>
                    urn:uuid:" + Guid.NewGuid().ToString() + @"</a:MessageID>
                    <a:ReplyTo>
                      <a:Address>
                      http://www.w3.org/2005/08/addressing/anonymous</a:Address>
                    </a:ReplyTo>
                    <VsDebuggerCausalityData xmlns=""http://schemas.microsoft.com/vstudio/diagnostics/servicemodelsink"">
                    uIDPozJEz+P/wJdOhoN2XNauvYcAAAAAK0Y6fOjvMEqbgs9ivCmFPaZlxcAnCJ1GiX+Rpi09nSYACQAA</VsDebuggerCausalityData>
                    <a:To s:mustUnderstand=""1""> "
                    + CRMUrl + @"</a:To>
                    <o:Security s:mustUnderstand=""1""
                    xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                      <u:Timestamp u:Id=""_0"">
                        <u:Created> " + DateTime.Now.ToString("s") + "." + DateTime.Now.Millisecond + @"Z</u:Created>
                        <u:Expires>" + DateTime.Now.AddMinutes(5.0).ToString("s") + "." + DateTime.Now.Millisecond + @"Z</u:Expires>
                      </u:Timestamp>
                      <EncryptedData Id=""Assertion0""
                      Type=""http://www.w3.org/2001/04/xmlenc#Element""
                      xmlns=""http://www.w3.org/2001/04/xmlenc#"">
                        <EncryptionMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#tripledes-cbc"">
                        </EncryptionMethod>
                        <ds:KeyInfo xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">
                          <EncryptedKey>
                            <EncryptionMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"">
                            </EncryptionMethod>
                            <ds:KeyInfo Id=""keyinfo"">
                              <wsse:SecurityTokenReference xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">

                                <wsse:KeyIdentifier EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary""
                                ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509SubjectKeyIdentifier"">"
                                + keyIdentifier + @"</wsse:KeyIdentifier>
                              </wsse:SecurityTokenReference>
                            </ds:KeyInfo>
                            <CipherData>
                              <CipherValue> " + securityToken0 + @"</CipherValue>
                            </CipherData>
                          </EncryptedKey>
                        </ds:KeyInfo>
                        <CipherData>
                          <CipherValue> "
                         + securityToken1 + @"</CipherValue>
                        </CipherData>
                      </EncryptedData>
                    </o:Security>
                  </s:Header>
                  ";

            //Return the header with the authentication tokens to complete the request
            
            return crmSoapRequestHeader;
            #endregion
        }

        public static string GetMethod(string url)
        {

            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            request.Timeout = 180000;

            string soapXMLResponse = string.Empty;
            using (WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                    soapXMLResponse = reader.ReadToEnd();
                response.Close();
            }

            return soapXMLResponse;
        }

        public static string GetSOAPResponse(string url, string soapEnvelope)
        {

            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/soap+xml; charset=UTF-8";

            request.Timeout = 580000;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(soapEnvelope);
            using (Stream str = request.GetRequestStream())
            {
                str.Write(bytes, 0, bytes.Length);
                str.Close();
            }

            string soapXMLResponse = string.Empty;
            using (WebResponse response = request.GetResponse())
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                    soapXMLResponse = reader.ReadToEnd();
                response.Close();
            }

            return soapXMLResponse;
        }

        public static string GetValueFromXML(string inputXML, string xPathQuery)
        {
            return GetValueFromXML(inputXML, xPathQuery, 0);
        }
        public static string GetValueFromXML(string inputXML, string xPathQuery, int index)
        {
            return GetValueFromXML(inputXML, xPathQuery, index, null);
        }
        public static string GetValueFromXML(string inputXML, string xPathQuery, int index, string[,] namespaces)
        {
            XmlDocument document = new XmlDocument();
            XmlNodeList nodes;
            document.LoadXml(inputXML);

            if (namespaces != null)
            {
                XmlNamespaceManager nsManager = new XmlNamespaceManager(document.NameTable);
                for (int i = 0; i < namespaces.Length / namespaces.Rank; i++)
                {
                    nsManager.AddNamespace(namespaces[i, 0], namespaces[i, 1]);
                }
                nodes = document.SelectNodes(xPathQuery, nsManager);
            }
            else
                nodes = document.SelectNodes(xPathQuery);

            if (nodes != null && nodes.Count > 0 && nodes[index] != null)
                return nodes[index].Value;
            else
                return string.Empty;
        }
    }
}