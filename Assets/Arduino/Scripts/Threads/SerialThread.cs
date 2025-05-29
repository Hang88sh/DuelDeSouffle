
public class SerialThread : SerialThreadLines
{
    public SerialThread(string portName, int baudRate, int delayBeforeReconnecting, int maxUnreadMessages)
                        : base(portName, baudRate, delayBeforeReconnecting, maxUnreadMessages){}
}
