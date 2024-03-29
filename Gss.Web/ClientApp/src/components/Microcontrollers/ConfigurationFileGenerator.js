import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { GetRequest, MakeAuthorizedRequest } from '../../requests/Requests';
import { useSelector } from 'react-redux';
import { selectUser } from '../../redux/reducers/authSlice';
import { useHistory } from 'react-router-dom';
import FormErrors from '../FormErrors';
import '@fontsource/roboto/300.css';
import { Button, ButtonGroup, Divider, FormControl, Grid, IconButton, InputAdornment, InputLabel, MenuItem, Snackbar, TextField, Tooltip, Typography } from '@material-ui/core';
import SettingsTwoToneIcon from '@material-ui/icons/SettingsTwoTone';
import { makeStyles } from '@material-ui/core/styles';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';
import DateFnsUtils from '@date-io/date-fns';
import Select from '@material-ui/core/Select';
import FileCopyTwoToneIcon from '@material-ui/icons/FileCopyTwoTone';
import MuiAlert from '@material-ui/lab/Alert';
import TodayTwoToneIcon from '@material-ui/icons/TodayTwoTone';
import { DateTimePicker } from "@material-ui/pickers";

const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
const ipRegex = /^(?!0)(?!.*\.$)((1?\d?\d|25[0-5]|2[0-4]\d)(\.|$)){4}$/;

const useStyles = makeStyles((theme) => ({
  title: {
    fontWeight: '300',
    marginBottom: theme.spacing(1),
    display: 'flex',
    alignItems: 'center'
  },
  titleIcon: {
    marginLeft: theme.spacing(2),
    marginRight: theme.spacing(2)
  },
  content: {
    marginTop: theme.spacing(2)
  },
  select: {
    width: '100%'
  },
  buttons: {
    display: 'flex',
    justifyContent: 'center'
  }
}));

export default function ConfigurationFileGenerator() {
  const classes = useStyles();
  const { microcontrollerID: _microcontrollerID } = useParams();
  const user = useSelector(selectUser);
  const history = useHistory();
  const [serverErrors, setServerErrors] = useState(null);
  const [openSnackbar, setOpenSnackbar] = useState(false);

  const [ date, setDate ] = useState(new Date());
  const [ writeSDIntervalSeconds, setWriteSDIntervalSeconds ] = useState(60);
  const [ transmitIntervalSeconds, setTransmitIntervalSeconds ] = useState(1800);
  const [ requestIntervalSeconds, setRequestIntervalSeconds ] = useState(900);

  const [ SSID, setSSID ] = useState('');
  const [ securityKey, setSecurityKey ] = useState('');
  const [ protocol, setProtocol ] = useState('t');
  const [ privateMode, setPrivateMode ] = useState(2);

  const [ serverIP, setServerIP ] = useState('192.168.100.5');
  const [ serverPort, setServerPort ] = useState('5001');

  const [ sensorID, setSensorID ] = useState('');
  const [ ownerID, setOwnerID ] = useState('');
  const [ microcontrollerID, setMicrocontrollerID ] = useState('');
  const [ microcontrollerPassword, setMicrocontrollerPassword ] = useState('');

  const [ configText, setConfigText ] = useState('');

  useEffect(() => {
    if (_microcontrollerID != null && user != null) {
      const getMicrocontrollerRequestFactory = (token) =>
        GetRequest(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/GetMicrocontroller/${_microcontrollerID}`, token);
  
      MakeAuthorizedRequest(getMicrocontrollerRequestFactory, user).then(response => {
        if (response.status !== 200) {
          if (response.status === 401) {
            history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
          } else if (response.status === 500) {
            history.push(process.env.REACT_APP_SERVER_ERROR_URL);
          } else {
            setServerErrors(response.errors);
          }
        } else {
          setMicrocontrollerID(response.data.ID);
          setOwnerID(response.data.UserInfo.ID);
          setSensorID(response.data.Sensors[0]?.ID ?? '');
        }
      });
    }
  }, [ _microcontrollerID, user, history ]);

  useEffect(() => {
    if (user != null) {
      setOwnerID(user.UserID);
    }
  }, [ user ]);

  useEffect(() => {
    const lines = [
      `Date=${date.getDate()};`,
      `Month=${date.getMonth()};`,
      `Year=${date.getFullYear() % 100};`,
      `WeekDay=${date.getDay()};`,
      `Hours=${date.getHours()};`,
      `Minutes=${date.getMinutes()};`,
      `Seconds=${date.getSeconds()};`,
      '',
      `WriteSDIntervalSeconds=${writeSDIntervalSeconds};`,
      `TransmitIntervalSeconds=${transmitIntervalSeconds};`,
      `RequestIntervalSeconds=${requestIntervalSeconds};`,
      '',
      `SSID=${SSID};`,
      `SecurityKey=${securityKey};`,
      `PrivateMode=${privateMode};`,
      '',
      `IP=${serverIP};`,
      `Port=${serverPort};`,
      `Protocol=${protocol};`,
      '',
      `OwnerID=${ownerID};`,
      `MicrocontrollerID=${microcontrollerID};`,
      `MicrocontrollerPassword=${microcontrollerPassword};`,
      `SensorID=${sensorID};`
    ];

    setConfigText(lines.join('\n'));

  }, [date, writeSDIntervalSeconds, transmitIntervalSeconds, requestIntervalSeconds, SSID, securityKey,
      protocol, privateMode, serverIP, serverPort, sensorID, ownerID,
      microcontrollerID, microcontrollerPassword]);


  const guidValidator = (value) => {
    const result = guidRegex.exec(value);

    return result === null
     ? { error: true, errorMessage: 'Invalid GUID' }
     : { error: false, errorMessage: null }
  };

  const ipValidator = (value) => {
    const result = ipRegex.exec(value);

    return result === null
     ? { error: true, errorMessage: 'Invalid IP Address' }
     : { error: false, errorMessage: null }
  };

  const sdWriteIntervalValidator = (value) => {
    const number = +value;
    if (isNaN(number)) {
      return { error: true, errorMessage: 'NaN' };
    } else if (number < 60 || number > 3195660) {
      return { error: true, errorMessage: 'Interval should be in [60, 3195660]' };
    }
 
    return { error: false, errorMessage: secondsToString(number) };
  };

  const transmitIntervalValidator = (value) => {
    const number = +value;
    if (isNaN(number)) {
      return { error: true, errorMessage: 'NaN' };
    } else if (number < 1800 || number > 3195660) {
      return { error: true, errorMessage: 'Interval should be in [1800, 3195660]' };
    }
 
    return { error: false, errorMessage: secondsToString(number) };
  };

  const requestIntervalValidator = (value) => {
    const number = +value;
    if (isNaN(number)) {
      return { error: true, errorMessage: 'NaN' };
    } else if (number < 900 || number > 3195660) {
      return { error: true, errorMessage: 'Interval should be in [900, 3195660]' };
    }
 
    return { error: false, errorMessage: secondsToString(number) };
  };

  const handleSnackbarClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }

    setOpenSnackbar(false);
  };

  return (
    <div>
      <Typography className={classes.title} variant="h3">
        <SettingsTwoToneIcon fontSize="large" className={classes.titleIcon}/>
        Configuration File Generator
      </Typography>

      <Divider />

      <Grid container className={classes.content} spacing={3}>
        <Grid item xs={5}>
          <Grid container spacing={2}>
          <Grid item xs={12}>
              <ConfigTextField
                label="Owner's ID"
                value={ownerID}
                setValue={setOwnerID}
                valueValidator={guidValidator} />
            </Grid>
            <Grid item xs={12}>
              <ConfigTextField
                label="Microcontroller's ID"
                value={microcontrollerID}
                setValue={setMicrocontrollerID}
                valueValidator={guidValidator} />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Microcontroller's Password"
                variant="outlined"

                value={microcontrollerPassword}
                onChange={(e) => setMicrocontrollerPassword(e.target.value)} />
            </Grid>
            <Grid item xs={12}>
              <ConfigTextField
                label="Sensor's ID"
                value={sensorID}
                setValue={setSensorID}
                valueValidator={guidValidator} />
            </Grid>
            <Grid item xs={12}>
              <ConfigTextField
                label="Server IP address"
                value={serverIP}
                setValue={setServerIP}
                valueValidator={ipValidator} />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Server port"
                variant="outlined"
                disabled

                value={serverPort}
                onChange={(e) => setServerPort(e.target.value)} />
            </Grid>
            <Grid item xs={12}>
              <FormControl variant="outlined" className={classes.select}>
                <InputLabel>Wifi privacy mode</InputLabel>
                <Select
                  value={privateMode ?? 0}
                  onChange={e => setPrivateMode(e.target.value)}
                  label="Wifi privacy mode">
                  <MenuItem value={0}><em>None</em></MenuItem>
                  <MenuItem value={1}>WEP</MenuItem>
                  <MenuItem value={2}>WPA Personal</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="SSID"
                variant="outlined"

                value={SSID}
                onChange={(e) => setSSID(e.target.value)} />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Security key"
                variant="outlined"

                value={securityKey}
                onChange={(e) => setSecurityKey(e.target.value)} />
            </Grid>
            <Grid item xs={12}>
              <FormControl variant="outlined" className={classes.select}>
                <InputLabel>Server connection protocol</InputLabel>
                <Select
                  value={protocol ?? 't'}
                  onChange={(e) => setProtocol(e.target.value)}
                  label="Server connection protocol"
                  disabled>
                  <MenuItem value={'t'}>TCP</MenuItem>
                  <MenuItem value={'u'}>UDP</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12}>
              <ConfigTextField
                label="SD card write interval (seconds)"
                value={writeSDIntervalSeconds}
                setValue={setWriteSDIntervalSeconds}
                valueValidator={sdWriteIntervalValidator} />
            </Grid>
            <Grid item xs={12}>
              <ConfigTextField
                label="Transmit interval (seconds)"
                value={transmitIntervalSeconds}
                setValue={setTransmitIntervalSeconds}
                valueValidator={transmitIntervalValidator} />
            </Grid>
            <Grid item xs={12}>
              <ConfigTextField
                label="Request interval (seconds)"
                value={requestIntervalSeconds}
                setValue={setRequestIntervalSeconds}
                valueValidator={requestIntervalValidator} />
            </Grid>
            <Grid item xs={12}>
              <MuiPickersUtilsProvider utils={DateFnsUtils}>
                <DateTimePicker
                  fullWidth
                  inputVariant="outlined"
                  hideTabs
                  ampm={false}
                  value={date}
                  format="dd.MM.yyyy HH:mm"
                  onChange={date => setDate(date)}
                  minDate={new Date("2015-01-01")}
                  helperText="Initial microcontroller DateTime"
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton>
                          <TodayTwoToneIcon />
                        </IconButton>
                      </InputAdornment>
                    ),
                  }} />
              </MuiPickersUtilsProvider>
            </Grid>
          </Grid>
        </Grid>

        <Grid item xs={7}>
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="config.txt"
                multiline
                rows={24}
                variant="outlined"
                InputProps={{
                  readOnly: true,
                }}

                value={configText || ''}
                onChange={(e) => setConfigText(e.target.value)} />
            </Grid>

            {serverErrors && (
              <Grid item xs={12}>
                <FormErrors errors={serverErrors} />
              </Grid>
            )}

            <Grid item xs={12}>
              <Typography variant="caption">
                This file needs to be saved in microSD's root directory with <b>config.txt</b> name.
              </Typography>
            </Grid>

            <Grid item xs={12} className={classes.buttons}>
              <ButtonGroup color="primary">
                <Tooltip title="Copy to clipboard">
                  <Button onClick={() => {
                    navigator.clipboard.writeText(configText);
                    setOpenSnackbar(true);
                  }}><FileCopyTwoToneIcon /></Button>
                </Tooltip>
                <Button onClick={() => download('config.txt', configText)}>Download config file</Button>
              </ButtonGroup>
            </Grid>
          </Grid>
        </Grid>
      </Grid>

      <Snackbar open={openSnackbar} autoHideDuration={3000} onClose={handleSnackbarClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <MuiAlert elevation={6} variant="filled" onClose={handleSnackbarClose} severity="info">
          Config file content copied to clipboard!
        </MuiAlert>
      </Snackbar>
    </div>
  );
}

function ConfigTextField(props) {
  const { label, value, setValue, valueValidator } = props;
  const [ error, setError ] = useState(false);
  const [ errorMessage, setErrorMessage ] = useState('');

  const handleValueChange = (e) => {
    const value = e.target.value;
    const validationResult = valueValidator(value);

    setError(validationResult.error);
    setErrorMessage(validationResult.errorMessage);

    setValue(value);
  }

  return (
    <TextField
      fullWidth
      label={label}
      variant="outlined"

      value={value}
      onChange={handleValueChange}
      error={error}
      helperText={errorMessage} />
  );
}

function download(filename, text) {
  var element = document.createElement('a');
  element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
  element.setAttribute('download', filename);

  element.style.display = 'none';
  document.body.appendChild(element);

  element.click();

  document.body.removeChild(element);
}

const secondsInAYear = 31536000;
const secondsInADay = 86400;
const secondsInAnHour = 3600;
const secondsInAMinute = 60;

function secondsToString(seconds)
{
  const numYears = Math.floor(seconds / secondsInAYear);
  const numDays = Math.floor((seconds % secondsInAYear) / secondsInADay); 
  const numHours = Math.floor(((seconds % secondsInAYear) % secondsInADay) / secondsInAnHour);
  const numMinutes = Math.floor((((seconds % secondsInAYear) % secondsInADay) % secondsInAnHour) / secondsInAMinute);
  const numSeconds = (((seconds % secondsInAYear) % secondsInADay) % secondsInAnHour) % secondsInAMinute;

  let result = '';
  const resultCreator = (name, value) => {
    if (value > 0) {
      result += `${value} ${name} `;
    }
  }

  resultCreator('years', numYears);
  resultCreator('days', numDays);
  resultCreator('hours', numHours);
  resultCreator('minutes', numMinutes);
  resultCreator('seconds', numSeconds);

  return result;
}
