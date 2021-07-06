# Sensor's Server

Server for receiving, processing and displaying data received from sensors. [Software for microcontroller.](https://github.com/N0menIllisLegio/NUCLEO-F401RE-Sensor)

Features:
- Request value from sensor in real time
  Microcontroller longpolls server for such requests and sends sensor's value in response if needed.
  Server through WebSocket (SignalR) notifies client which posted request that data was received (and what data exactly).

![Request sensor data](../images/RequestSensorData.png?raw=true)

![Notification](../images/Notification.png?raw=true)

- Set email notification when sensor reached assigned threshold

![Set email threshold](../images/SetEmailThreshold.png?raw=true)

![Email](../images/EmailThreshold.png?raw=true)

- Compare sensors data from different periods of time

![Data plots](../images/DataPlots.png?raw=true)

## Configuration

### Server configuration

File `appsettings.json` in `Gss.Web`

- `Email` section contains creds for site's mailer which will send warning (when sensor value reached set limit) or account messages.
- `AzureImages` section contains creds for Azure Blob Storage

  Each received image will be compressed in thumbnail with [Function App](https://github.com/N0menIllisLegio/function-image-upload-resize)

- `MicrocontrollersConnectionsOptions:Socket:IPAddress` address of server (on this address microcontrollers will send data)
- `MicrocontrollersConnectionsOptions:Socket:Port` port of server (on this port microcontrollers will send data)

```JSON
{
  "ConnectionStrings": {
    "AzureDB": ""
  },
  "Authentication": {
    "JWT": {
      "Issuer": "",
      "Audience": "",
      "Key": "",
      "AccessTokenLifetimeMinutes": "30",
      "RefreshTokenLifetimeDays": "10"
    },
    "Google": {
      "ClientID": "",
      "ClientSecret": ""
    }
  },
  "Email": {
    "Address": "",
    "Password": "",
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "465",
    "SmtpUseSsl": "true"
  },
  "AzureImages": {
    "AccountName": "",
    "AccountKey": "",
    "ImagesContainer": "images",
    "ThumbnailsContainer": "thumbnails",
    "SupportedExtensions": [
      ".png",
      ".jpg",
      ".jpeg",
      ".gif"
    ]
  },
  "MicrocontrollersConnectionsOptions": {
    "Socket": {
      "IPAddress": "",
      "Port": "",
      "SendTimeout": 100000,
      "ReceiveTimeout": 100000,
      "ListenQueue": 10
    }
  }
}
```

***Apply migrations to DB***
`Update-Database` - in Visual Studio's Package Manager Console


### Client configuration

File `.env` in `Gss.Web\ClientApp`

```
REACT_APP_SERVER_URL=http://localhost:5000/
REACT_APP_IMAGES_URL=https://sensorsappimagestorage.blob.core.windows.net/images/
REACT_APP_THUMBNAILS_URL=https://sensorsappimagestorage.blob.core.windows.net/thumbnails/
REACT_APP_AVATAR_PLACEHOLDER_URL=https://sensorsappimagestorage.blob.core.windows.net/images/acf7abad-1d34-45c4-b3fe-6b02637b599e.png
REACT_APP_UNAUTHORIZED_URL=/unauthorized
REACT_APP_SERVER_ERROR_URL=/serverError
```

## Communication with microcontrollers

To communicate with server microcontroller uses TCP protocol.
**If there any error in session handling than connection between server and microcontroller will be terminated.**

Session consists of three phases:
1. Microcontroller authorization
  Microcontroller sends `STM_AUTH|userID;microcontrollerID;password` request. If authorization is successful server will send `Server_OK` response, after which server will wait for second request.
    - Part before `|` character is command - `STM_AUTH`
    - `userID` - id of microcontroller owner
    - `microcontrollerID` - id of microcontroller
    - `password` - microcontroller's password

2. Request handling
  There are three requests:
    1. `STM_DATA|sensorID;sensorValueReadedDateTime;sensorValue` - microcontroller wants to send sensor's data.
      **Successful response - `Server_OK`**
        - `sensorID` - id of sensor
        - `sensorValueReadedDateTime` - date and time when value was read (`en-US`)
        - `sensorValue` - sensor's value

    2. `STM_RQ|` - microcontroller asks if there is pending request to read sensor value
      If there is **NO** pending requests server response will be: `Server_AT`.
      If there is pending request server response will be: `Server_SV|requestedSensorID;` after which server will wait for microcontroller request which should be `STM_DATA|sensorID;sensorValueReadedDateTime;sensorValue`.

    3. `STM_DT|` - microcontroller wants to receive server's UTC date&time
      Response: `Server_DT|Date=16;Month=04;Year=12;WeekDay=0;Hours=23;Minutes=12;Seconds=11;` - *16 Apr 2012 Sunday 23:12:11*

3. Close connection

## Screenshots

![Global microcontrollers map](../images/GlobalMap.png?raw=true)

![Table of public microcontrollers](../images/PublicMCsTable.png?raw=true)

![Pagination](../images/Pagination.png?raw=true)

![Config file generator](../images/ConfigFileGen.png?raw=true)

![Profile page](../images/ProfilePage.png?raw=true)

![Microcontroller page](../images/MicrocontrollerPage.png?raw=true)

## TODO

- [ ] Change response status codes from 200 to 201 (where needed)
- [ ] Add auth from external services (Google, GitHub etc.)
- [ ] Add filters support for Client
- [ ] *Forgot password?* function
- [ ] Implement email features on Client (email verification, email change, password reset)
- [ ] Time sync logic for `STM_DT`
- [ ] Requests encryption
