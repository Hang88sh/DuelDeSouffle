using UnityEngine;

public class TreatmentSerialTemplate : MonoBehaviour
{
    void OnMessageArrived(string msg)
    {
        if (float.Parse(msg) > 0)
        {
            if (float.Parse(msg) < 10)
            {
                // too weak
            }
            else if (float.Parse(msg) > 20)
            {
                // too strong
            }
            else
            {
                // correct
            }
        }
    }

    void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }
}
