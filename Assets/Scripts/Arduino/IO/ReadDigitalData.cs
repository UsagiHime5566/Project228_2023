//Only Project setting API to .Net 4.X Can Use Serial Port
#if NET_4_6
using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class ReadDigitalData : MonoBehaviour {
	
	private SerialPort arduinoPort;
	private string arduinoData = null;
	public bool ArduinoPortState = false;

	public Action<string> OnRecieveData;

	Thread recThread;
	Action passToMainThread;

	void Start(){
		string[] ports2 = SerialPort.GetPortNames(); 
		foreach (var item in ports2)
		{
			Debug.Log(item);
		}
	}

	public bool DvInit (string comName)
	{
		arduinoPort = new SerialPort( comName, 9600 );
		
		if( arduinoPort.IsOpen == false )
		{
			arduinoPort.Open();

			if(recThread != null)
				recThread.Abort();

			recThread = new Thread (RecieveThread);
			recThread.Start ();

			Debug.Log( "Open port sucessful!!" );
		}
		else
		{
			Debug.Log( "Port already opened!!" );
			return false;
		}

		ArduinoPortState = true;
		return true;
	}

	public void CloseArduino(){
		if(recThread != null)
			recThread.Abort();

		if(arduinoPort == null)
			return;
			
		arduinoPort.Close();
		ArduinoPortState = false;
	}

	void RecieveThread(){
		while (true) {
			if(arduinoPort == null){
				Thread.Sleep (10);
				continue;
			}

			if (arduinoPort.IsOpen) {
				try {
					arduinoData = arduinoPort.ReadLine ();
					Debug.Log(" >> Read arduino data : " + arduinoData );
					if(!string.IsNullOrEmpty(arduinoData)){
						passToMainThread += () => {
							OnRecieveData?.Invoke(arduinoData);
						};
					}
				}
				catch {}
			}
			else
			{
				ArduinoPortState = false;
			}

			Thread.Sleep (10);
		}
	}
	
	void Update ()
	{
		if(passToMainThread != null){
			passToMainThread.Invoke();
			passToMainThread = null;
		}
	}
}
#endif