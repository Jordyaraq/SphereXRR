using System;
using System.Reflection;
using UnityEngine;

public class ArduinoMpuInputBridge : MonoBehaviour
{
    [Header("Serial Input")]
    public bool enableSerialInput = false;
    public string portName = "COM3";
    public int baudRate = 9600;
    public int readTimeout = 10;
    public float reconnectInterval = 2f;
    public float minDirectionInterval = 0.18f;

    [Header("Target")]
    public MovementSequenceManager target;

    [Header("Debug")]
    public string lastRawLine = "";
    public string lastDirection = "";
    public string status = "Serial disabled";

    private Type serialPortType;
    private object serialPort;
    private float nextReconnectTime;
    private float lastDirectionTime;

    void Start()
    {
        if (target == null)
        {
            target = FindAnyObjectByType<MovementSequenceManager>(FindObjectsInactive.Include);
        }
    }

    void Update()
    {
        if (!enableSerialInput)
        {
            if (status != "Serial disabled")
            {
                status = "Serial disabled";
            }
            return;
        }

        if (target == null)
        {
            target = FindAnyObjectByType<MovementSequenceManager>(FindObjectsInactive.Include);
        }

        if (!EnsureSerialOpen())
            return;

        ReadAvailableLines();
    }

    bool EnsureSerialOpen()
    {
        if (serialPort != null && IsSerialOpen())
            return true;

        if (Time.unscaledTime < nextReconnectTime)
            return false;

        nextReconnectTime = Time.unscaledTime + reconnectInterval;

        try
        {
            serialPortType = serialPortType ?? Type.GetType("System.IO.Ports.SerialPort, System.IO.Ports") ?? Type.GetType("System.IO.Ports.SerialPort");
            if (serialPortType == null)
            {
                status = "System.IO.Ports is not available in this Unity runtime";
                return false;
            }

            serialPort = Activator.CreateInstance(serialPortType, portName, baudRate);
            serialPortType.GetProperty("ReadTimeout")?.SetValue(serialPort, readTimeout);
            serialPortType.GetMethod("Open", BindingFlags.Instance | BindingFlags.Public)?.Invoke(serialPort, null);
            status = "Serial connected: " + portName;
            return true;
        }
        catch (Exception exception)
        {
            status = "Serial connect failed: " + exception.GetBaseException().Message;
            CloseSerial();
            return false;
        }
    }

    bool IsSerialOpen()
    {
        if (serialPort == null || serialPortType == null)
            return false;

        try
        {
            object value = serialPortType.GetProperty("IsOpen")?.GetValue(serialPort);
            return value is bool isOpen && isOpen;
        }
        catch
        {
            return false;
        }
    }

    void ReadAvailableLines()
    {
        int safety = 0;
        while (GetBytesToRead() > 0 && safety < 8)
        {
            safety++;

            string raw = ReadLine();
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            lastRawLine = raw.Trim();
            string direction = ParseDirection(lastRawLine);
            if (string.IsNullOrEmpty(direction))
                continue;

            if (Time.unscaledTime - lastDirectionTime < minDirectionInterval)
                continue;

            lastDirectionTime = Time.unscaledTime;
            lastDirection = direction;
            target?.SubmitExternalInput(direction);
        }
    }

    int GetBytesToRead()
    {
        if (serialPort == null || serialPortType == null)
            return 0;

        try
        {
            object value = serialPortType.GetProperty("BytesToRead")?.GetValue(serialPort);
            return value is int bytes ? bytes : 0;
        }
        catch
        {
            status = "Serial read failed";
            CloseSerial();
            return 0;
        }
    }

    string ReadLine()
    {
        try
        {
            object value = serialPortType.GetMethod("ReadLine", BindingFlags.Instance | BindingFlags.Public)?.Invoke(serialPort, null);
            return value as string;
        }
        catch
        {
            return "";
        }
    }

    public string ParseDirection(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "";

        string normalized = raw.Trim().ToUpperInvariant();

        if (normalized.Contains("UP") || normalized.Contains("ARRIBA") || normalized == "W")
            return "UP";

        if (normalized.Contains("DOWN") || normalized.Contains("ABAJO") || normalized == "S")
            return "DOWN";

        if (normalized.Contains("LEFT") || normalized.Contains("IZQUIERDA") || normalized == "A")
            return "LEFT";

        if (normalized.Contains("RIGHT") || normalized.Contains("DERECHA") || normalized == "D")
            return "RIGHT";

        return "";
    }

    void OnDisable()
    {
        CloseSerial();
    }

    void OnDestroy()
    {
        CloseSerial();
    }

    void CloseSerial()
    {
        if (serialPort == null || serialPortType == null)
            return;

        try
        {
            if (IsSerialOpen())
            {
                serialPortType.GetMethod("Close", BindingFlags.Instance | BindingFlags.Public)?.Invoke(serialPort, null);
            }
        }
        catch
        {
        }

        serialPort = null;
    }
}
