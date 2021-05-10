import { useState, useEffect } from 'react';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogTitle from '@material-ui/core/DialogTitle';
import { Avatar, CircularProgress, Grid } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import { MakeAuthorizedRequest, PostRequest, PutRequest, DeleteRequest } from '../../requests/Requests';
import { useSelector } from 'react-redux';
import { selectUser } from '../../redux/reducers/authSlice';
import { useHistory } from 'react-router-dom'
import FormErrors from '../FormErrors';
import PagedTable from '../PagedTable';

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
    color: theme.palette.primary.main,
    marginRight: theme.spacing(1)
  },
  sensorTypeID: {
    marginRight: theme.spacing(2)
  },
  sensorTypeName: {
    marginLeft: theme.spacing(2)
  }
}));

const columns = [
  {field: 'Icon', headerName: 'Icon', flex: 0.2, align: 'center', headerAlign: 'center',
    renderCell: (params) => (
      <Avatar src={params.value} variant="square">{params.row.Units}</Avatar>
  )},
  { field: 'ID', headerName: 'ID', flex: 0.5 },
  { field: 'Units', headerName: 'Units', flex: 0.2, align: 'center', headerAlign: 'center' },
  { field: 'Name', headerName: 'Name', flex: 0.9 },
];

export default function Sensor(props) {
  const classes = useStyles();
  const history = useHistory();

  const [sensorName, setSensorName] = useState('');
  const [sensorDescription, setSensorDescription] = useState('');
  const [sensorType, setSensorType] = useState('');
  
  const [sensorNameError, setSensorNameError] = useState('');
  const [isSensorNameError, setIsSensorNameError] = useState(false);
  const [sensorDescriptionError, setSensorDescriptionError] = useState('');
  const [isSensorDescriptionError, setIsSensorDescriptionError] = useState(false);
  const [sensorTypeError, setSensorTypeError] = useState('');
  const [isSensorTypeError, setIsSensorTypeError] = useState(false);

  const [serverErrors, setServerErrors] = useState(null);
  const [isPending, setIsPending] = useState(false);

  const user = useSelector(selectUser);

  useEffect(() => {
    if (props.selectedSensor != null) {
      setSensorName(props.selectedSensor.Name);
      setSensorDescription(props.selectedSensor.Description);
      setSensorType(props.selectedSensor.SensorType.ID);
    }
  }, [props.selectedSensor]);

  useEffect(() => {
    if (sensorName == null || sensorName === '') {
      setSensorNameError('Sensor name is required');
      setIsSensorNameError(true);
    } else if (sensorName.length > 200) {
      setSensorNameError('Sensor name should be less than 200 charaters');
      setIsSensorNameError(true);
    } else {
      setSensorNameError('');
      setIsSensorNameError(false);
    }
  }, [sensorName]);

  useEffect(() => {
    if (sensorDescription?.length > 1800) {
      setSensorDescriptionError('Sensor\'s description should be less than 1800 charaters');
      setIsSensorDescriptionError(true);
    } else {
      setSensorDescriptionError('');
      setIsSensorDescriptionError(false);
    }
  }, [sensorDescription]);

  useEffect(() => {
    if (sensorType == null || sensorType === '') {
      setSensorTypeError('Sensor type is required');
      setIsSensorTypeError(true);
    } else {
      setSensorTypeError('');
      setIsSensorTypeError(false);
    }
  }, [sensorType]);

  const handleClose = () => {
    setSensorName('');
    setSensorDescription('');
    setSensorType('');

    setSensorDescriptionError('');
    setIsSensorDescriptionError(false);

    setSensorNameError('');
    setIsSensorNameError(false);

    setSensorTypeError('');
    setIsSensorTypeError(false);
    
    setServerErrors(null);

    props.setOpenDialog(false);
    props.setSelectedSensor(null);
  };

  const handleDelete = async () => {
    setIsPending(true);

    const deleteSensorRequestFactory = () =>
      DeleteRequest(`${process.env.REACT_APP_SERVER_URL}api/Sensors/Delete/${props.selectedSensor.ID}`, user?.AccessToken);
  
    const response = await MakeAuthorizedRequest(deleteSensorRequestFactory, user);

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
      props.setSensorChanged(!props.sensorChanged);
      handleClose();
    }
  };

  const handleSave = async () => {
    if (isSensorNameError || isSensorDescriptionError || isSensorTypeError) {
      return;
    }

    setIsPending(true);

    const createSensorRequestFactory = () =>
      PostRequest(`${process.env.REACT_APP_SERVER_URL}api/Sensors/Create`, {
        Name: sensorName,
        Description: sensorDescription,
        TypeID: sensorType
      }, user?.AccessToken);

    const updateSensorRequestFactory = () =>
      PutRequest(`${process.env.REACT_APP_SERVER_URL}api/Sensors/Update/${props.selectedSensor?.ID}`, {
        Name: sensorName,
        Description: sensorDescription,
        TypeID: sensorType
      }, user?.AccessToken);

    const sensorRequestFactory = props.selectedSensor != null
      ? updateSensorRequestFactory
      : createSensorRequestFactory;

    const sensorResponse = await MakeAuthorizedRequest(sensorRequestFactory, user);

    setIsPending(false);
    
    if (sensorResponse.status !== 200) {
      if (sensorResponse.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (sensorResponse.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setServerErrors(sensorResponse.errors);
      }
    } else {
      props.setSensorChanged(!props.sensorChanged);
      handleClose();
    }
  };

  const handleAddSensorType = (e) => {
    if (e?.row != null) {
      setSensorType(e.row.ID);
    }
  };

  return (
    <Dialog
      open={props.openDialog}
      fullWidth={true}
      maxWidth={'md'}>
      <DialogTitle style={{paddingBottom: '0'}}>
        { props.selectedSensor ? 'Update Sensor' : 'Create Sensor' }
      </DialogTitle>
      <DialogContent>
        <Grid container>
          <Grid item xs={12}>
            <TextField
              value={sensorName}
              onChange={(e) => setSensorName(e.target.value)}
              margin="dense"
              label="Name"
              fullWidth
              error={isSensorNameError}
              helperText={sensorNameError} />
          </Grid>
          <Grid item xs={12}>
            <TextField
              value={sensorDescription}
              onChange={(e) => setSensorDescription(e.target.value)}
              margin="dense"
              label="Description"
              multiline
              rows={4}
              fullWidth
              error={isSensorDescriptionError}
              helperText={sensorDescriptionError} />
          </Grid>
          <Grid item xs={12}>
            <TextField
              value={sensorType}
              margin="dense"
              label="Sensor's Type"
              fullWidth
              disabled
              error={isSensorTypeError}
              helperText={sensorTypeError} />
          </Grid>
          
          {serverErrors && (
            <Grid item xs={12} className={classes.serverErrors}>
              <FormErrors errors={serverErrors} />
            </Grid>
          )}

          <Grid item xs={12}>
            <PagedTable columns={columns} url={'api/SensorsTypes/GetAllSensorsTypes'} detailsAction={handleAddSensorType} tableHeight={250}/>
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        {props.selectedSensor && (
          <Button onClick={handleDelete} color="secondary" disabled={isPending}>
            Delete
          </Button>
        )}

        <Button onClick={handleClose} color="secondary" disabled={isPending}>
          Cancel
        </Button>
        <Button onClick={handleSave} color="primary" disabled={isPending}>
          {isPending && (<CircularProgress className={classes.progress} size={15} />)}
          Save
        </Button>
      </DialogActions>
    </Dialog>
  );
}