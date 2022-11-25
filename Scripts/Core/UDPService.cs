using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPService : MonobehaviourSingleton<UDPService>
{
    private UdpClient _socket;
    public bool IsConnect
    {
        get
        {
            return _socket?.Client?.Connected ?? false;
        }
    }

    private IPAddress _ip;
    private int _port;
    public string sendNumber;
    public string receiveNumber;
    private bool _isReceiPing;
    private DateTime _receiPingTime;
    private DateTime _checkTime;
    private ConcurrentQueue<int> _sendSize = new ConcurrentQueue<int>();
    private ConcurrentQueue<int> _receiveSize = new ConcurrentQueue<int>();
    public string send;
    public string receive;
    public const int SIO_UDP_CONNRESET = -1744830452;
    private readonly object _foo = new object();

    [SerializeField]
    private ConnectStatus _status;
    public ConnectStatus status
    {
        get { return _status; }
    }
    private IPEndPoint _server;
    private Thread _thread;

    public void Connect(IPAddress ip, int port)
    {
        if (_status != ConnectStatus.Connect)
        {

            _ip = ip;
            _port = port;
            Connect();
        }
    }

    private void Connect()
    {
        _server = new IPEndPoint(_ip, _port);
        _socket = new UdpClient(_ip.AddressFamily);
        //_socket.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
        // _socket.AllowNatTraversal(true);
#if UNITY_STANDALONE
        _socket.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);
#endif
        _status = ConnectStatus.Connect;
        if (_thread != null)
        {
            _thread.Abort();
        }
        _thread = new Thread(() => Receive());
        _thread.Start();
    }
    public IPEndPoint GetLocalEndPoint()
    {
        return _socket.Client.LocalEndPoint.GetLocalEndPoint();
    }

    public void Disconnect()
    {
        if (_thread != null)
        {
            _thread.Abort();
        }
#if UNITY_EDITOR
        //Disconnect();
        if (_socket != null)
        {
            _socket.Close();
            _socket = null;
        }
        _status = ConnectStatus.End;
#endif
    }
   
    private void FixedUpdate()
    {
      
    }
    public async void SendWithEP(byte[] data, IPEndPoint ep)
    {
        /// System.ObjectDisposedException: Cannot access a disposed object.
        try
        {
            await _socket?.SendAsync(data, data.Length, ep);

            _sendSize.Enqueue(data.Length);
        }
        catch //(Exception ex)
        {
            //Debug.LogError(ex.ToString());
        }
    }

    private async void Receive()
    {
        Debug.Log("Recive UDP");
        UdpReceiveResult res = default(UdpReceiveResult);
        while (status == ConnectStatus.Connect)
        {
            try
            {
                res = await _socket.ReceiveAsync();
            }
            catch
            {

                Debug.LogError("Error UDP");
                break;
            }
            finally
            {
                lock (_foo)
                {
                    if (status == ConnectStatus.Connect)
                        Progress(res.Buffer, res.RemoteEndPoint);
                }
            }
        }
        _status = ConnectStatus.Disconect;
    }
    private void Progress(byte[] data, IPEndPoint endPoint)
    {
        /// NullReferenceException: Object reference not set to an instance of an object
        if (data == null || data.Length < 1)
        {
            return;
        }
    }

    public void Disconnect(int index)
    {

    }

}
public enum ConnectStatus
{
    Waitting,
    Connect,
    Disconect,
    End,
}
public class AckMessage
{
    public long morder;
    public string id;
    public string room;
}