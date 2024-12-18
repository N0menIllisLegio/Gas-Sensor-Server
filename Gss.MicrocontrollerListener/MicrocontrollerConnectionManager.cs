using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Gss.MicrocontrollerListener;

internal sealed class MicrocontrollerConnectionManager : IDisposable
{
    private const string OkResponse = "Server_OK";
    private const string AttentionResponse = "Server_AT";
    private const string SensorValueResponse = "Server_SV";

    private readonly MicrocontrollersConnectionsOptions _options;
    private readonly Socket _socket;
    private readonly ILogger _logger;

    private bool _disposedValue;

    public MicrocontrollerConnectionManager(Socket socket, MicrocontrollersConnectionsOptions options, ILogger logger)
    {
        _socket = socket;
        _options = options;
        _logger = logger;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<MicrocontrollerRequest?> ReceiveRequestAsync(CancellationToken cancellationToken = default)
    {
        var request = await ReceiveAsync(cancellationToken);
        var microcontrollerRequest = new MicrocontrollerRequest(request);

        if (microcontrollerRequest.ValidateCommand())
            return microcontrollerRequest;

        _logger.LogWarning("Unknown command. {Endpoint} --- {Request}", _socket.RemoteEndPoint, request);

        return null;
    }

    public async Task SendOkAsync(CancellationToken cancellationToken = default)
    {
        await SendAsync(OkResponse, cancellationToken);
    }

    public async Task SendRequestSensorValueAsync(Guid sensorId, CancellationToken cancellationToken = default)
    {
        await SendAsync($"{SensorValueResponse}|{sensorId};", cancellationToken);
    }

    public async Task SendAttentionAsync(CancellationToken cancellationToken = default)
    {
        await SendAsync(AttentionResponse, cancellationToken);
    }

    public async Task SendDateTimeAsync(CancellationToken cancellationToken = default)
    {
        var currentDateTime = DateTime.UtcNow;

        await SendAsync($"Server_DT|" +
                        $"Date={currentDateTime.Day};Month={currentDateTime.Month};Year={currentDateTime.Date:yy};" +
                        $"WeekDay={(int)currentDateTime.DayOfWeek};Hours={currentDateTime.Hour};" +
                        $"Minutes={currentDateTime.Minute};Seconds={currentDateTime.Second};", cancellationToken);
    }

    private async Task<string> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        var receivedMessageBuilder = new StringBuilder();
        var receivedData = new byte[256];
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationTokenSource.CancelAfter(_options.ReceiveTimeout);

        do
        {
            var bytesReceived =
                await _socket.ReceiveAsync(receivedData, SocketFlags.None, cancellationTokenSource.Token);

            receivedMessageBuilder.Append(Encoding.ASCII.GetString(receivedData, 0, bytesReceived));
        } while (_socket.Available > 0);

        _logger.LogDebug("Received Data: {Endpoint} --- {Message}", _socket.RemoteEndPoint, receivedMessageBuilder);

        return receivedMessageBuilder.ToString();
    }

    private async Task SendAsync(string response, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending Data. {Endpoint} --- {Response}", _socket.RemoteEndPoint, response);

        var responseBytes = Encoding.ASCII.GetBytes(response);
        await _socket.SendAsync(responseBytes, SocketFlags.None, cancellationToken);
    }

    protected void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket.Dispose();
            }

            _disposedValue = true;
        }
    }

    ~MicrocontrollerConnectionManager()
    {
        Dispose(false);
    }
}