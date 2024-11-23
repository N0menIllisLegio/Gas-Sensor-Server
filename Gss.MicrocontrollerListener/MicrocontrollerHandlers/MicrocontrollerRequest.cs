using System.Globalization;

namespace Gss.MicrocontrollerListener.MicrocontrollerHandlers;

record AuthRequest(Guid UserId, Guid MicrocontrollerId, string Password);
record DataRequest(Guid MicrocontrollerSensorId, DateTime SensorValueReadTime, int SensorValue);

/*
a) Server_OK – server's OK;
b) Server_AT – server's echo response;
c) Server_SV|{id} – server's answer to STM's request if there pending sensor read requests;
d) Server_DT|Date={day};Month={month};Year={year:yy};WeekDay={dayOfWeek};Hours={hours};Minutes={minutes};Seconds={seconds}; - server's answer to sync time;
e) STM_AUTH|{userID};{microcontrollerID};{password} – MC request to auth;
f) STM_DATA|{sensorID};{sensorValueReadDateTime};{sensorValue} – MC request to write data to the DB;
g) STM_DT| – time sync request;
h) STM_RQ – sensor data request (checks if user requested unscheduled data read from sensor);
*/

internal sealed class MicrocontrollerRequest
{
    private const char CommandSeparator = '|';

    public const string AuthCommand = "STM_AUTH";
    public const string DataCommand = "STM_DATA";
    public const string DateSyncCommand = "STM_DT";
    public const string RequestSensorValueCommand = "STM_RQ";

    private readonly string[] _availableCommands =
        [AuthCommand, DataCommand, DateSyncCommand, RequestSensorValueCommand];

    private readonly string _command;
    private readonly string[] _commandParams;

    public MicrocontrollerRequest(string response)
    {
        (_command, _commandParams) = ParseRequest(response);
    }

    public string Command => _command;

    public bool ValidateCommand(string? expectedCommand = null)
    {
        return string.IsNullOrEmpty(expectedCommand)
            ? _availableCommands.Contains(_command)
            : expectedCommand == _command;
    }

    public AuthRequest? ParseAuthRequest()
    {
        if (!ValidateCommand(AuthCommand)
            || _commandParams.Length != 3
            || !Guid.TryParse(_commandParams[0], out var userId)
            || !Guid.TryParse(_commandParams[1], out var microcontrollerId)
            || string.IsNullOrEmpty(_commandParams[2]))
        {
            return null;
        }

        return new AuthRequest(userId, microcontrollerId, _commandParams[2]);
    }

    public DataRequest? ParseDataRequest()
    {
        if (!ValidateCommand(DataCommand)
            || _commandParams.Length != 3
            || !Guid.TryParse(_commandParams[0], out var microcontrollerSensorId)
            || !DateTime.TryParse(_commandParams[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out var sensorValueReadDateTime)
            || !int.TryParse(_commandParams[2], out var sensorValue))
        {
            return null;
        }

        return new DataRequest(microcontrollerSensorId, sensorValueReadDateTime, sensorValue);
    }

    private static (string command, string[] arguments) ParseRequest(string request)
    {
        var splitReceivedMessage = request.Split(CommandSeparator);
        var receivedCommand = splitReceivedMessage.First();
        var receivedArguments = splitReceivedMessage.Last().Split(';');

        return (receivedCommand, receivedArguments);
    }
}
