using System.Runtime.Serialization;

namespace UDP_Chat
{
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection.Metadata;
    using System.Runtime.Serialization.Json;
    public partial class Form1 : Form
    {
        UserName User;
        bool Connect;
        SynchronizationContext uiContext;
        public Form1()
        {
            InitializeComponent();
            User = new UserName();
            Connect = false;
            uiContext = SynchronizationContext.Current!;
            Form2 frm = new Form2(User);
            DialogResult result = frm.ShowDialog();
            WaitConnectClient();
            Task tsk = Send(new Message { message = null, user = User.name, Disconnect = false, Connect = true });
        }

        private async void WaitConnectClient()
        {
            await Task.Run(() =>
            {
                try
                {
                    IPEndPoint ipEndPoint = new(IPAddress.Any, 49154);
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPAddress ip = IPAddress.Parse("235.0.0.0");
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
                    socket.Bind(ipEndPoint);
                    while (true)
                    {
                        byte[] buf_bytes = new byte[1024];
                        EndPoint remote = new IPEndPoint(0xF70000, 100);
                        Connect = true;
                        int len = socket.ReceiveFrom(buf_bytes, ref remote);
                        byte[] bytes = new byte[len];
                        Array.Copy(buf_bytes, 0, bytes, 0, len);
                        MemoryStream stream = new MemoryStream(bytes);
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message));
                        Message? message = serializer.ReadObject(stream) as Message;
                        if (message != null)
                        {
                            if (message.Disconnect)
                                uiContext.Send((parametr) => listBox1.Items.Remove(message.message!), null);
                            else if ((message.message != null) && (message.Disconnect == false))
                            {
                                uiContext.Send((parametr) =>
                                {
                                    listBox2.Items.Add($"[{DateTime.Now}] {message.user}");
                                    listBox2.Items.Add(message.message);
                                }, null);
                            }
                            else if ((message.message == null) && (message.Disconnect == false) && (Connect == true))
                            {
                                uiContext.Send((parametr) => listBox1.Items.Add(message.user!), null);
                                if ((User.name != message.user))
                                {
                                    Task task = Send(new Message { message = null, user = User.name, Disconnect = false, Connect = false });
                                }
                            }
                            int index = -1;
                            uiContext.Send((parametr) => index = listBox1.Items.IndexOf(message.user!), null);
                            if ((Connect == false) && (message.user != User.name) && (index == -1))
                            {
                                uiContext.Send((parametr) => listBox1.Items.Add(message.user!), null);
                            }
                        }
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private async Task Send(Message message)
        {
            await Task.Run(() =>
            {
                try
                {
                    while (!Connect)
                    {
                    }
                    IPAddress ip = IPAddress.Parse("235.0.0.0");
                    IPEndPoint ipEndPoint = new(ip, 49154);
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
                    MemoryStream stream = new MemoryStream();
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message));
                    serializer.WriteObject(stream, message);
                    byte[] bytes = stream.ToArray();
                    socket.SendTo(bytes, ipEndPoint);
                    socket.Shutdown(SocketShutdown.Send);
                    socket.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void Click_Send(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                Message message = new Message { message = textBox1.Text, user = User.name, Disconnect = false };
                Task tsk = Send(message);
            }
            else
                MessageBox.Show("Заповніть поле", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private async void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Task tsk = Send(new Message { message = User.name, Disconnect = true });
            await Task.Run(() => tsk.Wait());
        }
    }
}
[Serializable]
[DataContract]
class Message
{
    [DataMember]
    public string? message { get; set; }
    [DataMember]
    public string? user { get; set; }
    [DataMember]
    public bool Disconnect { get; set; }
    [DataMember]
    public bool Connect { get; set; }
}

class UserName
{
    public string? name { get; set; }
}