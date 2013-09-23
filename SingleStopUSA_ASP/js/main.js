
/*function CreateEntity(entity)
{
    var sampleContact1 =
    {
        FirstName: "Joe",
        LastName: 'Foo',
        GenderCode: { Value: 2 },
        FamilyStatusCode: { Value: 1 },
        CreditLimit: { Value: "3000.0000" },
        BirthDate: new Date(1955, 1, 20),
        DoNotEMail: false,
        DoNotPhone: true
    };

    var contactId1 = createContactSync2(sampleContact1);

    alert("Contact was created with an ID: " + contactId1);
}*/




function CreateEntity2() {

    var createContact = new XrmServiceToolkit.Soap.BusinessEntity("contact");
    createContact.attributes["firstname"] = "Diane";
    createContact.attributes["lastname"] = "Morgan";
    createContact.attributes["middlename"] = "<&>";   // Deliberate special characters to ensure that the toolkit can handle special characters correctly.
    createContact.attributes["gendercode"] = { value: 2, type: "OptionSetValue" };
    createContact.attributes["familystatuscode"] = { value: 1, type: "OptionSetValue" }; // Picklist : Single - 1
    createContact.attributes["creditlimit"] = { value: 2, type: "Money" };
    createContact.attributes["birthdate"] = birthDate;
    createContact.attributes["donotemail"] = true;
    createContact.attributes["donotphone"] = false;
    createContact.attributes["parentcustomerid"] = { id: accountId, logicalName: "account", type: "EntityReference" };

    contactId = XrmServiceToolkit.Soap.Create(createContact);

   /* ok(guidExpr.test(contactId), "Creating a contact should returned the new record's ID in GUID format. ");     */

}

//$(document).ready(function () {
//    $('#createContact').click(function () {
//        CreateEntity(header,'contact');
//    });
//    $('#createCase').click(function () {
//        CreateEntity(header,'incident');
//    })
//});
function CreateEntity(header, entity) {
    var formArray;
    if (entity == 'contact') {
        formArray = $("#contactForm").serializeArray();
    }
    else {
        formArray = $("#caseForm").serializeArray();
    }

    var len = formArray.length;
    var formObj = {};

    for (i = 0; i < len; i++) {
        formObj[formArray[i].name] = formArray[i].value;
    }
    //parse the form for data
    var fields;
    if (entity == 'contact') {
        fields = [new CRMField('firstname', formObj['firstname']), new CRMField('lastname', formObj['lastname']), new CRMField('emailaddress1', formObj['email']), new CRMField('telephone1', formObj['phone'])];
    }
    else {
        fields = [new CRMField('title', formObj['title'])];
    }
    return CreateRecord(header, entity, fields);
}


//entityName can be contact, case etc.
function CreateRecord(header, entityName, fields) {
    try {
        var resultArray = new Array();
        var attributesList = '';

        for (i = 0; i < fields.length; i++) {
            if (fields[i].Value != null)
                attributesList += "<" + fields[i].SchemaName + ">" + fields[i].Value + "</" + fields[i].SchemaName + ">";
        }


        var xml = "<Create xmlns='http://schemas.microsoft.com/crm/2007/WebServices'><entity xsi:type='" + entityName + "'>" + attributesList + "</entity></Create>";
        var resultXml = CallCrmService(header,xml, 'Create');


        if (resultXml) {
            var newid = resultXml.selectSingleNode('//CreateResult').nodeTypedValue;
            return newid;
        }
    }
    catch (err) {
    }

    return null;
}

// Call Crm Service
function CallCrmService(header, soapBody, method) {
    try {
        var xmlHttpRequest = new ActiveXObject("Msxml2.XMLHTTP");
        //xmlHttpRequest.Open("POST", 'https://singlestopusa.crm.dynamics.com/mscrmservices/2007/CrmService.asmx', false); //synchronous
        xmlHttpRequest.Open("POST", 'https://singlestopusa.api.crm.dynamics.com/XRMServices/2011/Organization.svc/', false); //synchronous
        xmlHttpRequest.setRequestHeader("SOAPAction", 'http://schemas.microsoft.com/crm/2007/WebServices/' + method); //Fetch,Execute,Create
        xmlHttpRequest.setRequestHeader("Content-Type", "application/soap+xml; charset=UTF-8");
        var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
        //"<soap:Envelope xmlns:soap=\'http://schemas.xmlsoap.org/soap/envelope/\' xmlns:xsi=\'http://www.w3.org/2001/XMLSchema-instance\' xmlns:xsd=\'http://www.w3.org/2001/XMLSchema\'>" +
        "<soap:Envelope xmlns:soap=\'http://www.w3.org/2003/05/soap-envelope\' xmlns:ser=\'http://schemas.microsoft.com/xrm/2011/Contracts/Services\' xmlns:con=\'http://schemas.microsoft.com/xrm/2011/Contracts\' xmlns:sys=\'http://schemas.datacontract.org/2004/07/System.Collections.Generic\'>" +
        header + "<soap:Body>" + soapBody + "</soap:Body></soap:Envelope>";
        xmlHttpRequest.setRequestHeader("Content-Length", xml.length);
        xmlHttpRequest.send(xml);


        var resultXml = xmlHttpRequest.responseXML;
        var errorCount = resultXml.selectNodes('//error').length;

        if (errorCount != 0) {
            var msg = resultXml.selectSingleNode('//description').nodeTypedValue;
            alert(msg);

            return null;
        }

        return resultXml;
    }
    catch (err) {
    }

    return null;
}

function getHeader() {
    //var header = "<soap:Header><CrmAuthenticationToken xmlns='http://schemas.microsoft.com/crm/2007/WebServices'><AuthenticationType xmlns='http://schemas.microsoft.com/crm/2007/CoreTypes'>0</AuthenticationType><OrganizationName xmlns='http://schemas.microsoft.com/crm/2007/CoreTypes'>Single Stop USA</OrganizationName><CallerId xmlns='http://schemas.microsoft.com/crm/2007/CoreTypes'>00000000-0000-0000-0000-000000000000</CallerId></CrmAuthenticationToken></soap:Header>";
    var header = ""
    return header;
}


// Make Struct
function MakeStruct(names) {
    try {
        var names = names.split(' ');
        var count = names.length;

        function constructor() {
            for (var i = 0; i < count; i++)
                this[names[i]] = arguments[i];
        }

        return constructor;
    }
    catch (err) {
        alert(err);
    }
}


// Global Structs
//var FilterBy = MakeStruct("SchemaName Operator Value");
//var ViewColumn = MakeStruct("SchemaName Width");
var CRMField = MakeStruct("SchemaName Value");
//var MetaformObject = MakeStruct("SchemaName DisplayName");