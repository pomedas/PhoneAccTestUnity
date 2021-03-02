using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SendPhoneDataUDP : MonoBehaviour
{
    //UDP definitions
    public string ipAddress;
    int udpPort = 21800; 

    private UdpClient sock; //UDP
    private IPEndPoint iep; //UDP

    //Variable to check if we have made the connection
    bool connected = false;

    //The input field game object
    public InputField iPAddressInputField;

    //Text to show messages
    public Text msgText; 

    // Start is called before the first frame update
    void Start()
    {
        //Enables the use of the Gyroscope in the phone
        Input.gyro.enabled = true;

        //Initialize the defaul IP address
        iPAddressInputField.text = "192.168.86.165";
    }

    public void Connect()
    {
        if (!connected)
        {
            try
            {
                //Get the ip address from the input field
                ipAddress = iPAddressInputField.text;

                //Initialize socket objects
                sock = new UdpClient();
                iep = new IPEndPoint(IPAddress.Parse(ipAddress), udpPort);

                //
                connected = true;

                Debug.Log("Connected");
            }
            catch (Exception e) {
                //Show the error in the GUI
                msgText.text = e.Message.ToString();

                Debug.Log("Exception in Connect: " + e.Message.ToString());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            Send("Test");
        }
    }

    private void FixedUpdate()
    {
        if (connected)
        {
            Quaternion q = Input.gyro.attitude;
            transform.rotation = new Quaternion(q.x, q.y, -q.z, -q.w);
            string msg = (q.x) + "," + (q.y) + "," + (-q.z) + "," + (-q.w);
            Send(msg);
        }
    }

    public void Send(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);

        try
        {
            // Note: read UDP ports with "nc".e.g. nc - lu 127.0.0.1 27015
            sock.Send(data, data.Length, iep);
            //Debug.Log("->" + message);
        }
        catch (Exception e)
        {
            //Show the error in the GUI
            msgText.text = e.Message.ToString();
            Debug.Log("Exception send: " + e.Message.ToString());
        }

    }
}
