using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CreateTicket : MonoBehaviour
{
    static readonly string fdDomain = "yiplifreshdesk"; // your freshdesk domain
    static readonly string apiKey = "wLCOd8cgOBcXXNO4McNu";
    static readonly string apiPath = "/api/v2/tickets"; // API path
    private string ticketData = string.Empty;
    
    UnityWebRequest ticketWebPostRequest = null;

    WWWForm ticketDataForm = null;

    private void Start()
    {
        /*
        ticketDataForm = new WWWForm();

        byte[] byteArray = Encoding.UTF8.GetBytes(ticketData);

        ticketDataForm.AddField("data", ticketData);

        string authInfo = apiKey + ":X"; // It could be your username:password also.
        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

        ticketWebPostRequest = UnityWebRequest.Post("https://" + fdDomain + ".freshdesk.com" + apiPath, ticketDataForm);
        ticketWebPostRequest.SetRequestHeader("Authorization", "Basic " + authInfo);
        ticketWebPostRequest.SetRequestHeader("APIKey", apiKey);
        ticketWebPostRequest.SetRequestHeader("Content-Type", "application/json");

        StartCoroutine(SendWebRequest(ticketWebPostRequest));
        */

        //CoreStyle();
        SetTicketData();
    }

    public void SetTicketData()
    {
        //ticketData = "{\"status\": 2, \"priority\": 1, \"email\":\"test@test.com\",\"subject\":\"test\",\"description\":\"confirm whether received\"}";

        TicketStructure ts = new TicketStructure
        (
            "2",
            "1",
            "vismay@playyipli.com",
            "Test New",
            "This works as json"
        );

        Debug.LogError("Ticke data in json : " + JsonUtility.ToJson(ts));
    }

    private void CoreStyle()
    {
        /*
        string fdDomain = "yiplifreshdesk"; // your freshdesk domain
        string apiKey = "wLCOd8cgOBcXXNO4McNu";
        string apiPath = "/api/v2/tickets"; // API path
        string json = "{\"status\": 2, \"priority\": 1, \"email\":\"bhansali.saurabh20@gmail.com\",\"subject\":\"Vismay chalu\",\"description\":\"Vismay is not programmer. He is maha feku\"}";
        */
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + fdDomain + ".freshdesk.com" + apiPath);

        //HttpWebRequest class is used to Make a request to a Uniform Resource Identifier (URI).  
        request.ContentType = "application/json";

        // Set the ContentType property of the WebRequest. 
        request.Method = "POST";
        byte[] byteArray = Encoding.UTF8.GetBytes(ticketData);

        // Set the ContentLength property of the WebRequest. 
        request.ContentLength = byteArray.Length;
        string authInfo = apiKey + ":X"; // It could be your username:password also.
        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
        request.Headers["Authorization"] = "Basic " + authInfo;

        //Get the stream that holds request data by calling the GetRequestStream method. 
        Stream dataStream = request.GetRequestStream();

        // Write the data to the request stream. 
        dataStream.Write(byteArray, 0, byteArray.Length);

        // Close the Stream object. 
        dataStream.Close();

        try
        {
            Debug.LogError("Submitting Request");
            WebResponse response = request.GetResponse();

            // Get the stream containing content returned by the server.
            //Send the request to the server by calling GetResponse. 
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access. 
            StreamReader reader = new StreamReader(dataStream);

            // Read the content. 
            string Response = reader.ReadToEnd();

            //return status code
            Debug.LogErrorFormat("Status Code: {1} {0}", ((HttpWebResponse)response).StatusCode, (int)((HttpWebResponse)response).StatusCode);

            //return location header
            Debug.LogErrorFormat("Location: {0}", response.Headers["Location"]);

            //return the response 
            Debug.LogErrorFormat(Response);
        }
        catch (WebException ex)
        {
            Debug.LogErrorFormat("API Error: Your request is not successful. If you are not able to debug this error properly, mail us at support@freshdesk.com with the follwing X-Request-Id");
            Debug.LogErrorFormat("X-Request-Id: {0}", ex.Response.Headers["X-Request-Id"]);
            Debug.LogErrorFormat("Error Status Code : {1} {0}", ((HttpWebResponse)ex.Response).StatusCode, (int)((HttpWebResponse)ex.Response).StatusCode);
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                Debug.LogErrorFormat("Error Response: ");
                Debug.LogErrorFormat(reader.ReadToEnd());
            }
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("ERROR");
            Debug.LogErrorFormat(ex.Message);
        }
    }

    class TicketStructure
    {
        //ticketData = "{\"status\": 2, \"priority\": 1, \"email\":\"test@test.com\",\"subject\":\"test\",\"description\":\"confirm whether received\"}";

        string status = string.Empty;
        string priority = string.Empty;
        string email = string.Empty;
        string subject = string.Empty;
        string description = string.Empty;

        public TicketStructure(string status, string priority, string email, string subject, string description)
        {
            this.status = status;
            this.priority = priority;
            this.email = email;
            this.subject = subject;
            this.description = description;
        }
    }
}
