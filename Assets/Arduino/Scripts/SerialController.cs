using UnityEngine;
using System.Threading;
using System.IO.Ports;

public class SerialController : MonoBehaviour
{
    public string portName = "COM1";
    public int baudRate = 9600;
    public GameObject messageListener;
    public int reconnectionDelay = 1000;
    public int maxUnreadMessages = 1;

    public const string SERIAL_DEVICE_CONNECTED = "-Connected-";
    public const string SERIAL_DEVICE_DISCONNECTED = "-Disconnected-";

    private Thread thread;
    private SerialThreadLines serialThread;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        // Adjust port name based on the operating system
#if UNITY_STANDALONE_WIN
        // Use Windows COM ports
        portName = "COM1"; // Default for Windows, adjust as necessary
#elif UNITY_STANDALONE_LINUX
        // Use Linux serial ports
        portName = "/dev/ttyUSB0"; // Default for Linux, adjust as necessary
#endif

        // Perform Serial initialization for PC
        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            Debug.Log("Checking port " + port);
            try
            {
                SerialPort serialPort = new SerialPort(port, 9600);
                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;
                serialPort.DtrEnable = true;
                serialPort.RtsEnable = true;

                serialPort.Open();
                string response = serialPort.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    Debug.Log("Arduino connected on port " + serialPort.PortName + ", ID: " + response.Trim());
                    if (response == "0")
                    {
                        portName = serialPort.PortName;
                    }
                    else
                    {
                        Debug.LogWarning("Wrong Arduino connected on port " + port);
                    }
                }
                else
                {
                    Debug.LogWarning("No Arduino connected on port " + port);
                }
                serialPort.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }

    void OnEnable()
    {       
        // Serial Communication for PC
        serialThread = new SerialThreadLines(portName, baudRate, reconnectionDelay, maxUnreadMessages);
        thread = new Thread(new ThreadStart(serialThread.RunForever));
        thread.Start();
    }

    void OnDisable()
    {
        // Stop Serial Communication
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }

        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }

    bool begin = false;

    void Update()
    {
      /*  if (!begin)
        {
            Init();
            begin = true;
        }*/

        if (messageListener == null)
        {
            return;
        }

        string message = null;

        // Read Serial message
        message = (string)serialThread.ReadMessage();

        if (message == null)
        {
            return;
        }

        if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
        {
            messageListener.SendMessage("OnConnectionEvent", true);
        }
        else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
        {
            messageListener.SendMessage("OnConnectionEvent", false);
        }
        else
        {
            messageListener.SendMessage("OnMessageArrived", message);
        }
    }

    public string ReadSerialMessage()
    {
        return (string)serialThread.ReadMessage();
    }

    public void SendSerialMessage(string message)
    {
        serialThread.SendMessage(message);
    }
}