using System.Text.RegularExpressions;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

public class HostHelper
{
    [DllImport ("__Internal")]
    private static extern string getIPv6 (string host);

    public struct Address
    {
        public string host;
        public AddressFamily family;
    }

    private static string GetIPAddress(string host)
    {
#if UNITY_EDITOR
        return host + "&&ipv4";
#elif UNITY_IOS
        return getIPv6(host);
#else
        return host + "&&ipv4";
#endif
    }

    public static Address ConvertIPAddress(string host)
    {
        Address address = new Address();
        address.family = AddressFamily.InterNetwork;
        address.host = host;
        try 
        {
            string rawHost = GetIPAddress(host);
            if (!string.IsNullOrEmpty (rawHost)) 
            {
                string[] tmp = System.Text.RegularExpressions.Regex.Split (rawHost, "&&");
                if (tmp != null && tmp.Length >= 2 && tmp[1] == "ipv6") 
                {
                    address.host = tmp[0];
                    address.family = AddressFamily.InterNetworkV6;
                }
            }
        } 
        catch (Exception e) 
        {
            Debug.LogErrorFormat ("Convert Address Error: {0}", e.Message);
        }
        Debug.Log("Convert Address Host:" + address.host);
        return address;
    }
}