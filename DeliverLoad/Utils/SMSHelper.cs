using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;

namespace DeliverLoad.Utils
{
	public static class SMSHelper
	{
        public static string SendSMS(string mobile,string message)
        {
            //http://portal.supersolutions.pk/api/mt/SendSMS?=charles&password=charles&senderid=Charles &text=test
            //we creating the necessary URL string:
            string _URL = ConfigurationManager.AppSettings["SMS_URL"]; //where the SMS Gateway is running
            string _senderid = ConfigurationManager.AppSettings["SMS_SENDERID"];   // here assigning sender id
            string _user = ConfigurationManager.AppSettings["SMS_USERNAME"]; // API user name to send SMS
            string _pass = ConfigurationManager.AppSettings["SMS_PASSWORD"];     // API password to send SMS
            string _recipient = HttpUtility.UrlEncode(mobile);  // who will receive message
            string _messageText = HttpUtility.UrlEncode(message); // text message
                                                                  // Creating URL to send sms
            string _createURL = _URL +
                "?user=" + _user +
                "&password=" + _pass +
                "&senderid=" + _senderid +
                "&channel=Normal&DCS=0&flashsms=0" +
                "&number=" + _recipient+
                "&text=" + _messageText;
            try
            {
                HttpWebRequest _createRequest = (HttpWebRequest)WebRequest.Create(_createURL);
                HttpWebResponse myResp = (HttpWebResponse)_createRequest.GetResponse();
                System.IO.StreamReader _responseStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                string responseString = _responseStreamReader.ReadToEnd();
                _responseStreamReader.Close();
                myResp.Close();
                return responseString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}