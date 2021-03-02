using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;

public class ReceiverPhoneDataUDP : MonoBehaviour
{
	//Thread to receive by UDP from the Phone 
	bool on = false;
	private Thread lThread;

	//UDP Receiver definitions
	private UdpClient client;
	private IPEndPoint remoteIpEndPoint;

	//Receiver port
	int port = 21800;

	Quaternion receivedRotation; 

    private void Start()
    {
		Listen();
	}

	public void Listen()
	{
		client = new UdpClient(port);
		remoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

		lThread = new Thread(new ThreadStart(ListenUDPThread));
		lThread.Name = "Receiver UDP listen thread";
		lThread.Start();
	}

	private void ListenUDPThread()
	{
		var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
		//Debug.Log("separator: "+culture.NumberFormat.NumberDecimalSeparator);
		on = true;

		Debug.Log("Receiver: listening on port " + port);

		while (on)
		{
			try
			{
				byte[] packet = client.Receive(ref remoteIpEndPoint);

				// read message
				if (packet != null && packet.Length > 0)
				{
					string message = ExtractString(packet, 0, packet.Length);
					string[] messageSplit = message.Split(new Char[] { ',' });

					receivedRotation = new Quaternion(float.Parse(messageSplit[0].ToString()),
													float.Parse(messageSplit[1].ToString()),
													float.Parse(messageSplit[2].ToString()),
													float.Parse(messageSplit[3].ToString()));

					//Debug.Log("Receiver: "+message);
				}
				if (packet != null && packet.Length == 0)
				{
					Debug.Log("Received packet is empty");
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}
		}
	}

	public void Close()
	{
		on = false;
		lThread.Abort();
		if (client != null) client.Close();
	}

	private string ExtractString(byte[] packet, int start, int length)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < packet.Length; i++) sb.Append((char)packet[i]);
		return sb.ToString();
	}

    private void Update()
    {
		transform.rotation = receivedRotation;
    }

    private void OnDestroy()
    {
		Close();
    }
}
