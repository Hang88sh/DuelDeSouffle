using UnityEngine;
using System;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Threading;

public abstract class AbstractSerialThread
{
    // Parameters passed from SerialController, used for connecting to the serial device
    private string portName;
    private int baudRate;
    private int delayBeforeReconnecting;
    private int maxUnreadMessages;

    // Object from the .Net framework used to communicate with serial devices.
    private SerialPort serialPort;

    // Amount of milliseconds alloted to a single read or connect
    private const int readTimeout = 100;

    // Amount of milliseconds alloted to a single write.
    private const int writeTimeout = 100;

    // Internal synchronized queues used to send and receive messages from the serial device.
    private Queue inputQueue, outputQueue;

    // Indicates when this thread should stop executing.
    private bool stopRequested = false;

    private bool enqueueStatusMessages = false;


    #region Unity Methods
    // Constructs the thread object.
    public AbstractSerialThread(string portName, int baudRate, int delayBeforeReconnecting, int maxUnreadMessages, bool enqueueStatusMessages)
    {
        this.portName = portName;
        this.baudRate = baudRate;
        this.delayBeforeReconnecting = delayBeforeReconnecting;
        this.maxUnreadMessages = maxUnreadMessages;
        this.enqueueStatusMessages = enqueueStatusMessages;

        inputQueue = Queue.Synchronized(new Queue());
        outputQueue = Queue.Synchronized(new Queue());
    }

    // Invoked to stop the thread
    public void RequestStop()
    {
        lock (this)
        {
            stopRequested = true;
        }
    }

    // return the next available message
    public object ReadMessage()
    {
        if (inputQueue.Count == 0)
            return null;

        return inputQueue.Dequeue();
    }

    // Schedules a message to be sent to the serial device.
    public void SendMessage(object message)
    {
        outputQueue.Enqueue(message);
    }
    #endregion


    #region Serial Controller Thread
    // loop of attempting connection to the serial device. Can be stopped by invoking 'RequestStop'.
    public void RunForever()
    {
        try
        {
            while (!IsStopRequested())
            {
                try
                {
                    AttemptConnection();
                    while (!IsStopRequested())
                    {
                        RunOnce();
                    }
                }
                catch (Exception ioe)
                {
                    // A disconnection happened, or there was a problem reading/writing to the device.
                    Debug.LogWarning("Exception: " + ioe.Message + " StackTrace: " + ioe.StackTrace);
                    if (enqueueStatusMessages)
                    {
                        inputQueue.Enqueue(SerialController.SERIAL_DEVICE_DISCONNECTED);
                    }

                    CloseDevice();
                    Thread.Sleep(delayBeforeReconnecting);
                }
            }

            // Before closing the COM port, give the opportunity for all messages from the output queue to reach the other endpoint.
            while (outputQueue.Count != 0)
            {
                SendToWire(outputQueue.Dequeue(), serialPort);
            }

            CloseDevice();
        }
        catch (Exception e)
        {
            Debug.LogError("Unknown exception: " + e.Message + " " + e.StackTrace);
        }
    }

    private void AttemptConnection()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = readTimeout;
        serialPort.WriteTimeout = writeTimeout;
        serialPort.Open();

        if (enqueueStatusMessages)
            inputQueue.Enqueue(SerialController.SERIAL_DEVICE_CONNECTED);
    }

    private void CloseDevice()
    {
        if (serialPort == null)
        {
            return;
        }

        try
        {
            serialPort.Close();
        }
        catch (IOException)
        {
        }

        serialPort = null;
    }

    private bool IsStopRequested()
    {
        lock (this)
        {
            return stopRequested;
        }
    }

    // Attempt to read/write to the serial device. If there are more lines in the queue than we may have
    // at a given time, then the newly read lines will be discarded. This is a protection mechanism when
    // the port is faster than the Unity progeram. If not, we may run out of memory if the queue really fills.
    private void RunOnce()
    {
        try
        {
            // Send a message.
            if (outputQueue.Count != 0)
            {
                SendToWire(outputQueue.Dequeue(), serialPort);
            }

            // Read a message.
            object inputMessage = ReadFromWire(serialPort);
            if (inputMessage != null)
            {
                if (inputQueue.Count < maxUnreadMessages)
                {
                    inputQueue.Enqueue(inputMessage);
                }
                else
                {
                    Debug.LogWarning("Queue is full. Dropping message: " + inputMessage);
                }
            }
        }
        catch (TimeoutException)
        {
        }
    }

    // Sends a message through the serialPort.
    protected abstract void SendToWire(object message, SerialPort serialPort);

    // Reads and returns a message from the serial port.
    protected abstract object ReadFromWire(SerialPort serialPort);
    #endregion
}
