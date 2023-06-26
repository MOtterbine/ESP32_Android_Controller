using System.Text;
using Java.Util;
using Android.Bluetooth;
using Communication;
using ESP32_Android_Controller.Interfaces;

namespace ESP32_Android_Controller.Services.PartialMethods;

public partial class AndroidBlueToothDevice : IBlueToothService
{

    // ELM327 uart rx buffer is 512 bytes
    public const int BUFFER_SIZE = 2048;
    public event BluetoothEvent DeviceEvent;

    private BluetoothDevice bluetoothDevice = null;
    private BluetoothSocket bluetoothSocket = null;
    private BluetoothAdapter bluetoothAdapter = null;
    private byte[] RespBuffer = new byte[BUFFER_SIZE];
    private byte[] TmpBuffer = new byte[BUFFER_SIZE];
    private CancellationTokenSource tokenSource = new CancellationTokenSource();
    private CancellationToken _cancelReadToken;
    private Action tokenCancelOperation = null;

    public AndroidBlueToothDevice()
    {
        this._cancelReadToken = this.tokenSource.Token;
        tokenCancelOperation = this.NullFunction;
        BluetoothManager bManager =  Platform.CurrentActivity.GetSystemService(Android.Content.Context.BluetoothService) as BluetoothManager;
        this.bluetoothAdapter = bManager.Adapter;

        // required to syncronize the cancellation token
        this.Close();
    }

    public bool IsEnabled => this.bluetoothAdapter.IsEnabled;

    public IList<string> GetDeviceList()
    {
        var btdevice = this.bluetoothAdapter?.BondedDevices.Select(d => d.Name).ToList();
        return btdevice;
    }

    public bool IsOpen
    {
        get => this.bluetoothSocket == null ? false : this.bluetoothSocket.IsConnected;
    }

    public string DeviceName { get; set; }

    public bool IsConnected
    {
        get => this.bluetoothSocket == null ? false : this.bluetoothSocket.IsConnected;
    }

    public string Description => $"Bluetooth device: {this.DeviceName}";

    public bool Open()
    {
        try
        {
            if (this.bluetoothAdapter == null)
            {
                FireErrorEvent("No Bluetooth adapter found");
                return false;
            }

            if (this.bluetoothAdapter.State == State.Off)
            {
                FireErrorEvent("Bluetooth is not enabled on this device.");
                return false;
            }

            if (this.IsOpen)
            {
                return true;
            }

            if (String.IsNullOrEmpty(this.DeviceName))
            {
                var dd = this.bluetoothAdapter?.BondedDevices;

                foreach (var d in this.bluetoothAdapter.BondedDevices)
                {
                    var lx = d;
                }

                FireErrorEvent("Please set device in Setup page");
                return false;
            }

            this.bluetoothDevice = (from bd in this.bluetoothAdapter?.BondedDevices
                                where bd?.Name == this.DeviceName
                                select bd).FirstOrDefault();

            if (this.bluetoothDevice == null)
            {
                FireErrorEvent($"Cannot find Bluetooth device '{this.DeviceName}'");
                return false;
            }
                
            this.bluetoothSocket = this.bluetoothDevice?.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            if (this.bluetoothSocket == null)
            {
                FireErrorEvent("Invalid Bluetooth Channel");
                return false;
            }

            tokenCancelOperation = this.NullFunction;
            this.tokenSource?.Cancel();
            tokenCancelOperation = this.Listen;

            if (this.bluetoothSocket.IsConnected)
            {
                bluetoothSocket.InputStream.ReadAsync(TmpBuffer, this._cancelReadToken).GetAwaiter().OnCompleted(tokenCancelOperation);
                this.bluetoothSocket?.Close();
            }
            this.bluetoothSocket?.Connect();
            bluetoothSocket.InputStream.ReadAsync(TmpBuffer, this._cancelReadToken).GetAwaiter().OnCompleted(tokenCancelOperation);

            this.FireEvent(CommunicationEvents.ConnectedAsClient);
        }
        catch (Java.IO.IOException ioex)
        {
            this.FireErrorEvent($"ERROR: Unable to access Bluetooth device");
            return false;
        }
        catch (Exception ex)
        {
            this.FireErrorEvent($"ERROR: Unable to access Bluetooth device");
            return false;
        }

        return true;

    }

    private void NullFunction()
    { 
        int i = 0; 
    }

    private void Listen()
    {
        try
        {
            if (bluetoothSocket == null) return;
            if (!this.bluetoothSocket.IsConnected) return;
            int rcvCount = 0;

            try
            {
                rcvCount = bluetoothSocket.InputStream.Read(TmpBuffer);
            }
            catch (IndexOutOfRangeException)
            {
            }
            catch (Java.IO.IOException e)
            {
                rcvCount = -1;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (rcvCount > 0)
                {
                    FireReceiveEvent(TmpBuffer.Take(rcvCount).ToArray());
                }
                rcvCount = 0;
            }
            if (bluetoothSocket == null) return;

            bluetoothSocket.InputStream.ReadAsync(TmpBuffer, this._cancelReadToken).GetAwaiter().OnCompleted(tokenCancelOperation);
        }
        catch(TaskCanceledException)
        {
            Console.Write($"_____________________________________________________________{Environment.NewLine}");
            Console.Write($"***************** Listen Operation Stoppped *****************");
            Console.Write($"_____________________________________________________________");
        }
    }


    public bool Close()
    {

        tokenCancelOperation = this.NullFunction;

        this.tokenSource?.Cancel();

        this.bluetoothSocket?.Close();
        bluetoothSocket?.Dispose();
        bluetoothSocket = null;
        bluetoothDevice = null;

        this.FireEvent(CommunicationEvents.Disconnected, "Connection Closed");

        return true;
    }

    protected void FireDeviceEvent(ChannelEventArgs e)
    {
        if (this.CommunicationEvent != null)
        {
            CommunicationEvent(this, e);
        }
    }

    public event DeviceEvent CommunicationEvent;

    private void FireErrorEvent(string message)
    {
        using (ChannelEventArgs evt = new ChannelEventArgs())
        {
            evt.Event = CommunicationEvents.Error;
            evt.Description = message;
            this.FireDeviceEvent(evt);
        }
    }
    private void FireEvent(CommunicationEvents eventType, string message = null)
    {
        using (ChannelEventArgs evt = new ChannelEventArgs())
        {
            evt.Event = eventType;
            evt.Description = message;
            this.FireDeviceEvent(evt);
        }
    }

    private void FireReceiveEvent(byte[] bytes)
    {
        using (ChannelEventArgs evt = new ChannelEventArgs())
        {
            evt.data = bytes;
            evt.Event = CommunicationEvents.ReceiveEnd;
            evt.Description = "Bluetooth Response";
            this.FireDeviceEvent(evt);
        }
    }

    public bool Initialize()
    {
        return true;
        //throw new NotImplementedException();
    }

    public override string ToString()
    {
        return this.DeviceName;
    }

    public byte[] Read()
    {
        byte[] tmpBuffer = null;
        var buffSize = bluetoothSocket.InputStream.Read(null);
        tmpBuffer = new byte[buffSize];
        bluetoothSocket.InputStream.Read(tmpBuffer);
        return tmpBuffer;
    }

    public async Task<bool> Send(string text)
    {
        await this.Send(Encoding.UTF8.GetBytes(text), 0, text.Length);
        return false;
    }

    protected IAsyncResult _WriteAsyncResult = null;

    public async Task<bool> Send(byte[] buffer, int offset, int count)
    {
        try
        {
            if (!this.IsOpen)
            {
                this.FireErrorEvent($"Bluetooth device is not open");
                return false;
            }

            await this.bluetoothSocket.OutputStream.WriteAsync(buffer, offset, count);

            return true;
        }
        catch (Exception ett)
        {
            FireErrorEvent($"Bluetooth error - {ett.Message}");
        }
        return false;
    }

}
