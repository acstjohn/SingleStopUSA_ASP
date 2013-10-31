~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Single Stop USA / JPMorgan Force for Good Collaboration
Student Support Solution 2013

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Overview
	The goal of this solution is to provide a generic interface to the Microsoft Dynamics CRM that the Single Stop web developers could utilize to populate a support queue for the staff to action. This solution utilizes the SDK provided by Microsoft for authenticating and making the requests. Links for the references can be found in the references section below. There are two main code files in the solution:
		* authentication.cs - Authenticates a Microsoft 365 live id and provides back an authentication header that can then be used with the language of your choosing to send requests to the CRM.
		* CRMService - Accepts a collection of entities that the class will process, create within the CRM, and then associate back to the case (incident). This class takes care of all authentication with the CRM and assumes that the CRM objects will be passed to it. 
		
	Web.config is where the url and login details for the CRM are located. CRMService.cs will validate that the format of these details are correct, below you can find a sample of how this should look:
	
	<connectionStrings>
    <!-- Online using Office 365 -->
    <add name="Server=CRM Online, organization=singlestopusa, user=JPMorgan2"
        connectionString="Url=https://singlestopusa.crm.dynamics.com; Username=JPMorgan2@singlestopusa.org; Password=JPMCSingle$top;"/>
	</connectionStrings>
	
	The user that you put here will be tied to each of the records created within the CRM so I would suggest that you pick a name for the ID like 'Online Support' or 'Webserice' so you know its system generated. This will also make it easier to group the support requests into queues later.
		
References
	Microsoft Dynamics SDK
	http://www.microsoft.com/en-us/download/details.aspx?id=24004
		Used through out the application to authentication, query, create, and modify objects within the CRM.
	
	Windows Identity Foundation SDK
	http://www.microsoft.com/en-us/download/details.aspx?id=4451
		Used by the authentication service to create the security tokens.