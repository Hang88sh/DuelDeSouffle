using System.IO.Ports;

public class SerialThreadLines : AbstractSerialThread
{
    public SerialThreadLines(string portName, int baudRate, int delayBeforeReconnecting, int maxUnreadMessages)
                             : base(portName, baudRate, delayBeforeReconnecting, maxUnreadMessages, true){}

    protected override void SendToWire(object message, SerialPort serialPort)
    {
        serialPort.WriteLine((string) message);
    }

    protected override object ReadFromWire(SerialPort serialPort)
    {
        return serialPort.ReadLine();
    }
}
