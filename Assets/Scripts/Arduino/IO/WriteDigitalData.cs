//Only Project setting API to .Net 4.X Can Use Serial Port
#if NET_4_6
using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class WriteDigitalData : MonoBehaviour {
	
	private SerialPort arduinoPort;
	public bool ArduinoPortState { get {
		if(arduinoPort == null)
			return false;
			
		return arduinoPort.IsOpen;
		} }

	//此處為傳送方COM, 但切記千萬不要在 arduino 方下 Serial.print / println 指令，這會造成Serial阻塞導致程式當機，因為此script並未讀取port上任何資料
	public bool DvInit (string comName)
	{
		arduinoPort = new SerialPort( comName, 9600 );
		
		if( arduinoPort.IsOpen == false )
		{
			arduinoPort.Open();
			arduinoPort.WriteTimeout = 100;
			Debug.Log( "Open port sucessful!!" );
		}
		else
		{
			Debug.Log( "Port already opened!!" );
			return false;
		}
		
		return true;
	}

	public void WriteData(string data){
		if(arduinoPort == null)
			return;

		if(!arduinoPort.IsOpen)
			return;

		arduinoPort.WriteLine(data);
	}

	public void CloseArduino(){
		if(arduinoPort == null)
			return;

		arduinoPort.Close();
	}
}
#endif