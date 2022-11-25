using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TCPService : MonobehaviourSingleton<TCPService>
{
    private TcpListener _listener;
    private TcpClient _serverConnect;
    public const int SIO_UDP_CONNRESET = -1744830452;
    private IPEndPoint _serverEndPoint;
    public ConnectStatus status { get; private set; }
    public string localAdress
    {
        get
        {
            return _listener.LocalEndpoint.GetLocalEndPoint().ToString();
        }
    }

 
    public IPEndPoint GetLocalEndPoint()
    {
        return _serverConnect.Client.LocalEndPoint.GetLocalEndPoint();
    }
    public async void ConnectToServer(Action onFinish = null,Action onFailed = null)
    {
        if (status != ConnectStatus.Connect || !_serverConnect.Connected)
        {
            try
            {
                status = ConnectStatus.Connect;
                if (_serverConnect == null)
                {
                    new Thread(() => Listen()).Start();
                }
                else
                {
                    _serverConnect.Close();
                }
                _serverConnect = new TcpClient(_serverEndPoint.AddressFamily);
                _serverConnect.SendBufferSize = 65535;
                _serverConnect.ReceiveBufferSize = 65535;
                await _serverConnect.ConnectAsync(_serverEndPoint.Address, _serverEndPoint.Port);
                new Thread(() => ConnectAndListenFromID(_serverConnect, "Server")).Start();
            }
            catch
            {
                onFailed?.Invoke();

                Debug.LogError("Can not connect TCP to Server");
                return;
            }
        }
        onFinish?.Invoke();
    }
    public void ConnectToServer(IPEndPoint iPEndPoint, Action onFinish = null,Action onFailed = null)
    {
        _serverEndPoint = iPEndPoint;
        ConnectToServer(onFinish, onFailed);
    }
    private void OnDisable()
    {
        status = ConnectStatus.End;
    }

    public string tcpAddress
    {
        get
        {
            if (_listener != null)
                return _listener.Server.LocalEndPoint.ToString().Split(':')[1];
            else
                return "";
        }
    }
    public async void Listen()
    {
        _listener = new TcpListener(IPAddress.Any, 0);
        _listener.Start();
        while (status == ConnectStatus.Connect)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync();
                if (client.Client != null && client.Client.RemoteEndPoint != null)
                {
                    IPEndPoint ep = client.Client.RemoteEndPoint as IPEndPoint;
                    if (ep != null)
                    {
                        Debug.LogWarningFormat("Accept TCP connect from TCP/IP {0}:{1}", ep.Address, ep.Port);
                    }
                }
                new Thread(() => ConnectAndListenFromID(client, "aa")).Start();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);

                break;
            }
        }
        _listener.Stop();
    }
    public void DisconnectServer()
    {
        _serverConnect.Close();
    }
    public void Disconnect()
    {
        status = ConnectStatus.End;
    }
    public void SendToServer(object data)
    {
        if (status == ConnectStatus.Connect && _serverConnect != null && _serverConnect.Connected)
        {
            try
            {
                _serverConnect.SendData(data);
            }
            catch
            {
                ConnectToServer();
            }
        }
    }

    private async void ConnectAndListenFromID(TcpClient tcpClient, string name)
    {
        using (var stream = tcpClient.GetStream())
        {
            var buffer = new byte[tcpClient.ReceiveBufferSize];
            var data = new List<byte>();

            while (tcpClient.Connected && status == ConnectStatus.Connect)
            {

                try
                {
                    while (stream.DataAvailable)
                    {
                        var length = await stream.ReadAsync(buffer, 0, buffer.Length);
                        data.AddRange(buffer.Take(length));
                        while (data.Contains(0x23))//# is the end character
                        {
                            var index = data.IndexOf(0x23);
                            var concat = data.Take(index).ToArray();
                            data.RemoveRange(0, index + 1);
                        }
                    }
                    await Task.Delay(10);
                }
                catch (Exception ex)
                {
                    tcpClient?.Close();
                    stream?.Dispose();
                    Debug.LogError(ex);
                    break;
                }
            }
            Debug.LogFormat("Disconnect from {0}", name);
        }
    }
}
