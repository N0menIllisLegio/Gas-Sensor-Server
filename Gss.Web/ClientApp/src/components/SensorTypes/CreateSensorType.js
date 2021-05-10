import { useState, useEffect } from 'react';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogTitle from '@material-ui/core/DialogTitle';
import { CircularProgress, Grid } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import AvatarButton from '../AvatarButton';
import { theme as appTheme } from '../../App';
import { MakeAuthorizedRequest, Request, PostRequest } from '../../requests/Requests';
import { useSelector } from 'react-redux';
import { selectUser } from '../../redux/reducers/authSlice';
import { useHistory } from 'react-router-dom'
import FormErrors from '../FormErrors';

const useStyles = makeStyles((theme) => ({
  avatarContainer: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center'
  },
  serverErrors: {
    marginTop: theme.spacing(2),
    marginLeft: theme.spacing(2),
  }
}));

export default function CreateSensorType(props) {
  const classes = useStyles();
  const history = useHistory();

  const [sensorImageSrc, setSensorImageSrc] = useState(null);
  const [sensorImage, setSensorImage] = useState(null);
  const [sensorName, setSensorName] = useState('');
  const [sensorUnit, setSensorUnit] = useState('');
  
  const [sensorNameError, setSensorNameError] = useState('');
  const [isSensorNameError, setIsSensorNameError] = useState(false);
  const [sensorUnitError, setSensorUnitError] = useState('');
  const [isSensorUnitError, setIsSensorUnitError] = useState(false);

  const [serverErrors, setServerErrors] = useState(null);
  const [isPending, setIsPending] = useState(false);

  const user = useSelector(selectUser);

  useEffect(() => {
    if (sensorName == null || sensorName === '') {
      setSensorNameError('Sensor name is required');
      setIsSensorNameError(true);
    } else if (sensorName.length > 200) {
      setSensorNameError('Sensor name length should be less than 200 charaters');
      setIsSensorNameError(true);
    } else {
      setSensorNameError('');
      setIsSensorNameError(false);
    }
  }, [sensorName]);

  useEffect(() => {
    if (sensorUnit == null || sensorUnit === '') {
      setSensorUnitError('Sensor unit is required');
      setIsSensorUnitError(true);
    } else if (sensorUnit.length > 20) {
      setSensorUnitError('Sensor unit length should be less than 20 charaters');
      setIsSensorUnitError(true);
    } else {
      setSensorUnitError('');
      setIsSensorUnitError(false);
    }
  }, [sensorUnit]);

  const handleClose = () => {
    setSensorImageSrc(null);
    setSensorName('');
    setSensorUnit('');

    props.setOpenDialog(false);
  };

  const handleSave = async () => {
    if (isSensorNameError || isSensorUnitError) {
      return;
    }

    setIsPending(true);

    let imageResponse = null;

    if (sensorImage != null) {
      const imageRequestData = new FormData();
      imageRequestData.append('FileForm', sensorImage);
  
      const saveImageRequestFactory = () =>
        Request(`${process.env.REACT_APP_SERVER_URL}api/Files/AvatarUpload`, {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${user?.AccessToken ?? ''}`
          },
          body: imageRequestData
        });
  
      imageResponse = await MakeAuthorizedRequest(saveImageRequestFactory, user);

      if (imageResponse.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      }
    }

    const saveSensorTypeRequestFactory = () =>
      PostRequest(`${process.env.REACT_APP_SERVER_URL}api/SensorsTypes/Create`, {
        Name: sensorName,
        Icon: imageResponse?.status === 200
          ? imageResponse.data.FileUrl
          : null,
        Units: sensorUnit
      }, user?.AccessToken);

    const sensorTypeResponse = await MakeAuthorizedRequest(saveSensorTypeRequestFactory, user);

    setIsPending(false);
    
    if (sensorTypeResponse.status !== 200) {
      if (sensorTypeResponse.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (sensorTypeResponse.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setServerErrors(sensorTypeResponse.errors);
        console.log(serverErrors);
      }
    } else {
      handleClose();
    }
  };

  return (
    <Dialog open={props.openDialog}>
      <DialogTitle>Create Sensor Type</DialogTitle>
      <DialogContent>
        <Grid container>
          <Grid item xs={4} className={classes.avatarContainer}>
            <AvatarButton
              avatarWidth={appTheme.spacing(16)}
              avatarHeight={appTheme.spacing(16)}
              imageSrc={sensorImageSrc}
              setImageSrc={setSensorImageSrc}
              setImage={setSensorImage} />
          </Grid>
          <Grid item xs={8}>
            <TextField
              value={sensorName}
              onChange={(e) => setSensorName(e.target.value)}
              margin="dense"
              label="Name"
              fullWidth
              error={isSensorNameError}
              helperText={sensorNameError} />
            <TextField
              value={sensorUnit}
              onChange={(e) => setSensorUnit(e.target.value)}
              margin="dense"
              label="Unit"
              fullWidth
              error={isSensorUnitError}
              helperText={sensorUnitError} />
          </Grid>
          {serverErrors && (
            <Grid item xs={12} className={classes.serverErrors}>
              <FormErrors errors={serverErrors} />
            </Grid>
          )}
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} color="secondary" disabled={isPending}>
          Cancel
        </Button>
        <Button onClick={handleSave} color="primary" disabled={isPending}>
          {isPending && (<CircularProgress size={15} />)}
          Save
        </Button>
      </DialogActions>
    </Dialog>
  );
}
