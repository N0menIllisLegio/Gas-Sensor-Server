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
import { MakeAuthorizedRequest, PostImageRequest, PostRequest, PutRequest, DeleteRequest } from '../../requests/Requests';
import { useSelector } from 'react-redux';
import { selectUser } from '../../redux/reducers/authSlice';
import { useHistory } from 'react-router-dom'
import FormErrors from '../FormErrors';
import ConfirmationPopup from '../ConfirmationPopup';

const useStyles = makeStyles((theme) => ({
  avatarContainer: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center'
  },
  serverErrors: {
    marginTop: theme.spacing(2),
    marginLeft: theme.spacing(2),
  },
  progress: {
    color: theme.palette.grey[400],
    marginRight: theme.spacing(1)
  },
}));

export default function SensorType(props) {
  const classes = useStyles();
  const history = useHistory();

  const [sensorTypeImageSrc, setSensorTypeImageSrc] = useState(null);
  const [sensorTypeImage, setSensorTypeImage] = useState(null);
  const [sensorTypeName, setSensorTypeName] = useState('');
  const [sensorTypeUnit, setSensorTypeUnit] = useState('');
  
  const [sensorTypeNameError, setSensorTypeNameError] = useState('');
  const [isSensorTypeNameError, setIsSensorTypeNameError] = useState(false);
  const [sensorTypeUnitError, setSensorTypeUnitError] = useState('');
  const [isSensorTypeUnitError, setIsSensorTypeUnitError] = useState(false);

  const [serverErrors, setServerErrors] = useState(null);
  const [isPending, setIsPending] = useState(false);

  const user = useSelector(selectUser);

  useEffect(() => {
    if (props.selectedSensorType != null) {
      setSensorTypeImageSrc(props.selectedSensorType.Icon);
      setSensorTypeName(props.selectedSensorType.Name);
      setSensorTypeUnit(props.selectedSensorType.Units);
    }
  }, [props.selectedSensorType]);

  useEffect(() => {
    if (sensorTypeName == null || sensorTypeName === '') {
      setSensorTypeNameError('Sensor type name is required');
      setIsSensorTypeNameError(true);
    } else if (sensorTypeName.length > 200) {
      setSensorTypeNameError('Sensor type name should be less than 200 charaters');
      setIsSensorTypeNameError(true);
    } else {
      setSensorTypeNameError('');
      setIsSensorTypeNameError(false);
    }
  }, [sensorTypeName]);

  useEffect(() => {
    if (sensorTypeUnit == null || sensorTypeUnit === '') {
      setSensorTypeUnitError('Sensor type\'s units of measure is required');
      setIsSensorTypeUnitError(true);
    } else if (sensorTypeUnit.length > 20) {
      setSensorTypeUnitError('Sensor type\'s units of measure should be less than 20 charaters');
      setIsSensorTypeUnitError(true);
    } else {
      setSensorTypeUnitError('');
      setIsSensorTypeUnitError(false);
    }
  }, [sensorTypeUnit]);

  const handleClose = () => {
    props.setOpenDialog(false);
    setSensorTypeImageSrc(null);
    setSensorTypeName('');
    setSensorTypeUnit('');
    setSensorTypeUnitError('');
    setIsSensorTypeUnitError(false);
    setSensorTypeNameError('');
    setIsSensorTypeNameError(false);
    setServerErrors(null);
    props.setSelectedSensorType(null);
  };

  const handleSave = async () => {
    if (isSensorTypeNameError || isSensorTypeUnitError) {
      return;
    }

    setIsPending(true);

    let imageResponse = null;

    if (sensorTypeImage != null) {
      const saveImageRequestFactory = (token) =>
        PostImageRequest(`${process.env.REACT_APP_SERVER_URL}api/Files/AvatarUpload`, sensorTypeImage, token);
  
      imageResponse = await MakeAuthorizedRequest(saveImageRequestFactory, user);

      if (imageResponse.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      }
    }

    const createSensorTypeRequestFactory = (token) =>
      PostRequest(`${process.env.REACT_APP_SERVER_URL}api/SensorsTypes/Create`, {
        Name: sensorTypeName,
        Icon: imageResponse?.status === 200
          ? imageResponse.data.FileUrl
          : null,
        Units: sensorTypeUnit
      }, token);

    const updateSensorTypeRequestFactory = (token) =>
      PutRequest(`${process.env.REACT_APP_SERVER_URL}api/SensorsTypes/Update/${props.selectedSensorType?.ID}`, {
        Name: sensorTypeName,
        Icon: sensorTypeImage != null
          ? imageResponse?.status === 200
            ? imageResponse.data.FileUrl
            : null
          : props.selectedSensorType?.Icon,
        Units: sensorTypeUnit
      }, token);

    const sensorTypeRequestFactory = props.selectedSensorType != null
      ? updateSensorTypeRequestFactory
      : createSensorTypeRequestFactory;

    const sensorTypeResponse = await MakeAuthorizedRequest(sensorTypeRequestFactory, user);

    setIsPending(false);
    
    if (sensorTypeResponse.status !== 200) {
      if (sensorTypeResponse.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (sensorTypeResponse.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setServerErrors(sensorTypeResponse.errors);
      }
    } else {
      props.setSensorTypeChanged(!props.sensorTypeChanged);
      handleClose();
    }
  };

  const handleDelete = async () => {
    setOpenConfirmationPopup(true);
  };

  const [openConfirmationPopup, setOpenConfirmationPopup] = useState(false);

  let handleAgreeConfirmationPopupAction = async () => {
    setOpenConfirmationPopup(false);
    setIsPending(true);

    const deleteSensorTypeRequestFactory = (token) =>
      DeleteRequest(`${process.env.REACT_APP_SERVER_URL}api/SensorsTypes/Delete/${props.selectedSensorType.ID}`, token);
  
    const response = await MakeAuthorizedRequest(deleteSensorTypeRequestFactory, user);

    setIsPending(false);

    if (response.status !== 200) {
      if (response.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (response.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setServerErrors(response.errors);
      }
    } else {
      props.setSensorTypeChanged(!props.sensorTypeChanged);
      handleClose();
    }
  };

  let handleDisagreeConfirmationPopupAction = () => {
    setOpenConfirmationPopup(false);
  };

  return (
    <Dialog open={props.openDialog}>
      <DialogTitle>{ props.selectedSensorType ? 'Update Sensor Type' : 'Create Sensor Type' }</DialogTitle>
      <DialogContent>
        <Grid container>
          <Grid item xs={4} className={classes.avatarContainer}>
            <AvatarButton
              placeholderText={props.selectedSensorType?.Units}
              avatarWidth={appTheme.spacing(16)}
              avatarHeight={appTheme.spacing(16)}
              imageSrc={sensorTypeImageSrc}
              setImageSrc={setSensorTypeImageSrc}
              setImage={setSensorTypeImage} />
          </Grid>
          <Grid item xs={8}>
            <TextField
              value={sensorTypeName}
              onChange={(e) => setSensorTypeName(e.target.value)}
              margin="dense"
              label="Name"
              fullWidth
              error={isSensorTypeNameError}
              helperText={sensorTypeNameError} />
            <TextField
              value={sensorTypeUnit}
              onChange={(e) => setSensorTypeUnit(e.target.value)}
              margin="dense"
              label="Units of measure"
              fullWidth
              error={isSensorTypeUnitError}
              helperText={sensorTypeUnitError} />
          </Grid>
          {serverErrors && (
            <Grid item xs={12} className={classes.serverErrors}>
              <FormErrors errors={serverErrors} />
            </Grid>
          )}
        </Grid>
      </DialogContent>
      <DialogActions>
        {props.selectedSensorType && (
          <Button onClick={handleDelete} color="secondary" disabled={isPending}>
            Delete
          </Button>
        )}

        <Button onClick={handleClose} disabled={isPending}>
          Cancel
        </Button>
        <Button onClick={handleSave} color="primary" disabled={isPending}>
          {isPending && (<CircularProgress className={classes.progress} size={15} />)}
          Save
        </Button>
      </DialogActions>

      <ConfirmationPopup
        open={openConfirmationPopup}
        handleAgree={handleAgreeConfirmationPopupAction}
        handleDisagree={handleDisagreeConfirmationPopupAction}
        title="Delete Sensor Type?"
        content={`Do you realy want to delete ${sensorTypeName} sensor type?`} />
    </Dialog>
  );
}
