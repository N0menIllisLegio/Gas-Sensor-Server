import { useEffect, useState } from 'react';
import { useHistory, useParams } from 'react-router-dom';
import { GetRequest, MakeAuthorizedRequest, PostRequest, PutRequest } from '../../requests/Requests';
import { useSelector } from 'react-redux';
import { selectUser } from '../../redux/reducers/authSlice';
import { Divider, FormControlLabel, Grid, TextField, Typography, Switch, Snackbar, Button, CircularProgress } from '@material-ui/core';
import '@fontsource/roboto/300.css';
import { makeStyles } from '@material-ui/core/styles';
import { Map, Marker } from 'pigeon-maps';
import SensorsPagedList from './SensorsPagedList';
import EditTwoToneIcon from '@material-ui/icons/EditTwoTone';
import PickedSensorsList from './PickedSensorsList';
import MuiAlert from '@material-ui/lab/Alert';
import FormErrors from '../FormErrors';

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
  sensorsList: {
    width: '100%',
    backgroundColor: theme.palette.background.paper,
    height: theme.spacing(50)
  },
  divider: {
    marginBottom: theme.spacing(2)
  },
  microcontrollersSensorsTitle: {
    fontWeight: '300',
    marginBottom: theme.spacing(1),
    display: 'flex',
    alignItems: 'center',
    marginTop: theme.spacing(2)
  },
  progress: {
    color: theme.palette.grey[400],
    marginRight: theme.spacing(1)
  },
}));

const maxSensors = 5;

export default function EditMicrocontroller() {
  const classes = useStyles();
  const { id } = useParams();
  const history = useHistory();
  const user = useSelector(selectUser);
  const [ serverErrors, setServerErrors ] = useState(null);
  const [ isSavePending, setIsSavePending ] = useState(false);
  const [ addSensorButtonEnabled, setAddSensorButtonEnabled ] = useState(true);
  const [ openSnackbar, setOpenSnackbar ] = useState(false);
  const [ snackbarMessage, setSnackbarMessage ] = useState('');

  const [ name, setName ] = useState('');
  const [ password, setPassword ] = useState('');
  const [ publicMode, setPublicMode ] = useState(true);
  const [ latitude, setLatitude ] = useState(50);
  const [ longitude, setLongitude ] = useState(15);
  const [ sensors, setSensors ] = useState([]);
 
  useEffect(() => {
    if (id != null) {
      const getMicrocontrollerRequestFactory = token =>
        GetRequest(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/GetMicrocontroller/${id}`, token);
        
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
          setName(response.data.Name);
          setPublicMode(response.data.Public);
          setLatitude(response.data.Latitude);
          setLongitude(response.data.Longitude);
          setSensors(response.data.Sensors);
        }
      });
    }
  }, [id, history, user]);

  useEffect(() => {
    setAddSensorButtonEnabled(sensors.length < maxSensors);
  }, [sensors]);

  const handleSensorClick = (sensor) => {
    if (sensors.find((element, index, array) => element.ID === sensor.ID)) {
      setSnackbarMessage(`${sensor.Name} already added to microcontroller!`);
      setOpenSnackbar(true);
    } else {
      if (sensors.length < maxSensors) {
        setSensors([...sensors, sensor]);
      } else {
        setSnackbarMessage(`Max sensors number on one microcontroller: ${maxSensors}`);
        setOpenSnackbar(true);
      }
    }
  };

  const handleDeletePickedSensor = (sensor) => {
    setSensors(sensors.filter(s => s !== sensor));
  };

  const handleSnackbarClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }

    setOpenSnackbar(false);
  };

  const handleMapClick = (e) => {
    setLatitude(e.latLng[0] ?? '');
    setLongitude(e.latLng[1] ?? '');
  };

  const handleSaveMicrocontrollerClick = async (e) => {
    e.preventDefault();
    setIsSavePending(true);

    const requestBody = {
      Name: name,
      Public: publicMode,
      Latitude: latitude,
      Longitude: longitude,
      Password: password,
      SensorIDs: sensors.reduce((acc, element) => [...acc, element.ID], [])
    };

    const postMicrocontrollerRequestFactory = (token) =>
      PostRequest(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/Create`, requestBody, token);

    const putMicrocontrollerRequestFactory = (token) =>
      PutRequest(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/Update/${id}`, requestBody, token);

    const requestFactory = id != null
      ? putMicrocontrollerRequestFactory
      : postMicrocontrollerRequestFactory;

    const response = await MakeAuthorizedRequest(requestFactory, user);

    setIsSavePending(false);

    if (response.status !== 200) {
      if (response.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (response.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setServerErrors(response.errors);
      }
    } else {
      history.push(`/microcontroller/${response.data.ID}`);
    }
  };

  return (
    <div>
      <Typography className={classes.title} variant="h3">
        <EditTwoToneIcon fontSize="large" className={classes.titleIcon}/>
        {(id && (<span>Edit {name ?? 'Microcontroller'}</span>)) || (<span>Create Microcontroller</span>)}
      </Typography>

      <Divider />

      <form className={classes.content} onSubmit={handleSaveMicrocontrollerClick}>
        <Grid container spacing={3}>
          <Grid item xs={5}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <TextField
                  required
                  fullWidth
                  label="Microcontroller's Name"
                  variant="outlined"
                  
                  value={name ?? ''}
                  onChange={(e) => setName(e.target.value)}/>
              </Grid>
              <Grid item xs={12}>
                <TextField
                  required
                  type="password"
                  fullWidth
                  label="Microcontroller's Password"
                  variant="outlined"

                  value={password ?? ''}
                  onChange={(e) => setPassword(e.target.value)} />
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel control={<Switch color="primary" checked={publicMode ?? false} onChange={(e) => setPublicMode(e.target.checked)} />} label="Public"  />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  fullWidth
                  label="Microcontroller's Latitude"
                  variant="outlined"
                  value={latitude ?? ''}
                  onChange={(e) => setLatitude(e.target.value)} />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  fullWidth
                  label="Microcontroller's Longitude"
                  variant="outlined"
                  value={longitude ?? ''}
                  onChange={(e) => setLongitude(e.target.value)} />
              </Grid>

              {sensors && sensors.length > 0 && (
                <Grid item xs={12}>
                  <Typography className={classes.microcontrollersSensorsTitle} variant="h5">
                    Microcontroller's Sensors
                  </Typography>
                  
                  <Divider className={classes.divider}/>

                  <PickedSensorsList pickedSensors={sensors} handleDeletePickedSensor={handleDeletePickedSensor} />
                </Grid>
              )}

              <Grid item xs={12}>
                <Button 
                  variant="contained"
                  color="primary"
                  disableElevation
                  size="large"
                  type="submit"
                  style={{width: '100%'}}
                  disabled={isSavePending}>
                  {isSavePending && (<CircularProgress className={classes.progress} size={15} />)}
                  Save Microcontroller
                </Button>
              </Grid>

              {serverErrors && (
                <Grid item xs={12}>
                  <FormErrors errors={serverErrors} />
                </Grid>
              )}
            </Grid>
          </Grid>
          <Grid item xs={7}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Typography className={classes.title} variant="h5">
                  Location
                </Typography>
                
                <Divider className={classes.divider}/>

                <Map
                  height={400}
                  center={[latitude, longitude]}
                  zoom={3}
                  onClick={handleMapClick}>
                  <Marker width={50} anchor={[latitude, longitude]} />
                </Map>
              </Grid>
              
              <Grid item xs={12}>
                <Typography className={classes.title} variant="h5">
                  Sensors
                </Typography>
                
                <Divider className={classes.divider}/>

                <SensorsPagedList handleSensorClick={handleSensorClick} addSensorButtonEnabled={addSensorButtonEnabled} />
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </form>

      <Snackbar open={openSnackbar} autoHideDuration={3000} onClose={handleSnackbarClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <MuiAlert elevation={6} variant="filled" onClose={handleSnackbarClose} severity="error">
          {snackbarMessage}
        </MuiAlert>
      </Snackbar>
    </div>
  )
}
