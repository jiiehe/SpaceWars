using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// this file is used to connect to the server
/// </summary>
namespace NetworkController
{
    public static class NController
    {
        /// <summary>
        /// connect to the server, set up the connection
        /// </summary>
        /// <param name="callMe"></param>
        /// <param name="hostname"></param>
        /// <returns></returns>
      public static  Socket ConnectToServer(Action<SocketState> callMe, string hostname)
        {
            Socket socket;
            IPAddress ipaddress;
            MakeSocket(hostname, out socket, out ipaddress);
            SocketState state = new SocketState();
            state.theSocket = socket;
            state.callMe = callMe;
            socket.BeginConnect(ipaddress, 11000, ConnectedCallback, state);
            return socket;
        }
        // callback method used to connect to the server
       public static void ConnectedCallback(IAsyncResult stateAsArObject)
        {
            //connect to the server
            SocketState state = (SocketState)stateAsArObject.AsyncState;

            try
            {
                // Complete the connection.
                state.theSocket.EndConnect(stateAsArObject);
                state.callMe(state);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                return;
            }

           
        }
        /// <summary>
        /// receive the data and store the data to the messageBuffer
        /// </summary>
        /// <param name="state"></param>
      public static void GetData(SocketState state)
        {
            state.theSocket.BeginReceive(state.messageBuffer, 0, state.messageBuffer.Length, SocketFlags.None, ReceiveCallback, state);
        }
        /// <summary>
        /// callback to end receiving    
        /// </summary>
        /// <param name="stateAsArObject"></param>
       public static void ReceiveCallback(IAsyncResult stateAsArObject)
        {
            try
            {
                SocketState state = (SocketState)stateAsArObject.AsyncState;

                int bytesRead = state.theSocket.EndReceive(stateAsArObject);

                // If the socket is still open
                if (bytesRead > 0)
                {
                    string theMessage = Encoding.UTF8.GetString(state.messageBuffer, 0, bytesRead);
                    // Append the received data to the growable buffer.
                    // It may be an incomplete message, so we need to start building it up piece by piece
                    state.sb.Append(theMessage);
                    state.callMe(state);

                }
            }catch(Exception e)
            {

            }

          
        }
        /// <summary>
        /// send the data to the server 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
       public static void Send(Socket socket, String data)
        {
            //if (e.KeyCode == Keys.Enter)
            try
            {
                string message = data;
                // Append a newline, since that is our protocol's terminating character for a message.
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, socket);
            }
            catch(Exception e)
            {

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
      public static void SendCallback(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            // Nothing much to do here, just conclude the send operation so the socket is happy.
            s.EndSend(ar);
        }

        /// <summary>
        /// Creates a Socket object for the given host string
        /// </summary>
        /// <param name="hostName">The host name or IP address</param>
        /// <param name="socket">The created Socket</param>
        /// <param name="ipAddress">The created IPAddress</param>
        public static void MakeSocket(string hostName, out Socket socket, out IPAddress ipAddress)
        {
            ipAddress = IPAddress.None;
            socket = null;
            try
            {
                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo;

                // Determine if the server address is a URL or an IP
                try
                {
                    ipHostInfo = Dns.GetHostEntry(hostName);
                    bool foundIPV4 = false;
                    foreach (IPAddress addr in ipHostInfo.AddressList)
                        if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                        {
                            foundIPV4 = true;
                            ipAddress = addr;
                            break;
                        }
                    // Didn't find any IPV4 addresses
                    if (!foundIPV4)
                    {
                        System.Diagnostics.Debug.WriteLine("Invalid addres: " + hostName);
                        throw new ArgumentException("Invalid address");
                    }
                }
                catch (Exception)
                {
                    // see if host name is actually an ipaddress, i.e., 155.99.123.456
                    System.Diagnostics.Debug.WriteLine("using IP");
                    ipAddress = IPAddress.Parse(hostName);
                }

                // Create a TCP/IP socket.
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                // Disable Nagle's algorithm - can speed things up for tiny messages, 
                // such as for a game
                socket.NoDelay = true;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create socket. Error occured: " + e);
                throw new ArgumentException("Invalid address");
            }
        }
        /// <summary>
        /// wait for client joinig in 
        /// </summary>
        /// <param name="callMe"></param>
        public static void ServerAwaitClientLoop(Action<SocketState> callMe)
        {
            TcpListener lstn = new TcpListener(IPAddress.Any, 11000);//create TcpListener for accpecting the client
            lstn.Start();
            ConnectionState cs = new ConnectionState();
            cs.lstn = lstn;
            cs.callMe = callMe;
            lstn.BeginAcceptSocket(AcceptNewClient, cs);
        }

        public static void AcceptNewClient(IAsyncResult ar)
        {
            ConnectionState cs = (ConnectionState)ar.AsyncState;
            Socket socket = cs.lstn.EndAcceptSocket(ar);
            SocketState ss = new SocketState();
            ss.theSocket = socket;
            ss.callMe = cs.callMe;
            ss.callMe(ss);
            cs.lstn.BeginAcceptSocket(AcceptNewClient, cs);
        }
    }
    /// <summary>
    /// A SocketState represents all of the information needed
    /// to handle one connection.
    /// </summary>
    public class SocketState
    {
        // It would be better to make these private
        public Socket theSocket = null;
        public byte[] messageBuffer = new byte[4096];
        public int uid;
        public Action<SocketState> callMe;
        public StringBuilder sb = new StringBuilder();


       

    }

    public class ConnectionState
    {
        public TcpListener lstn;
        public Action<SocketState> callMe;
    }
    
}
