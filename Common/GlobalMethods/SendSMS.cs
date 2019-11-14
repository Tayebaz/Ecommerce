using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace Common.GlobalMethods
{
    public static class SendSMS
    {
        public static void Send(string mobile, string msg)
        {
            var URL = "http://sms.bulkssms.com/submitsms.jsp?";
            string user = "SHADEMO1";
            string key = "c02c8e5780XX";
            try
            {
                URL = URL + "user=" + user + "&key=" + key + "&mobile=" + mobile + "&message=" + msg + "&senderid=INFOSM&accusage=1";
                if ((!(string.IsNullOrEmpty(mobile) && string.IsNullOrEmpty(msg))))
                {
                    System.IO.StreamReader respBody = null;
                    System.Net.HttpWebRequest fr = null;
                    string str = "";

                    fr = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);

                    if ((fr.GetResponse().ContentLength > 0))
                    {
                        respBody =
                              new System.IO.StreamReader(fr.GetResponse().GetResponseStream());
                        while (!respBody.EndOfStream)
                        {
                            str = str + respBody.ReadLine();
                        }
                        respBody.Close();
                    }
                }

            }
            catch (Exception ex)
            {

                //Console.WriteLine("StartSending0", ex.Message);
            }

        }

    }
}
