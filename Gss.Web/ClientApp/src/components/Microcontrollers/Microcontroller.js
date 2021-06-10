import { useParams } from 'react-router-dom';
import { Map, Marker } from 'pigeon-maps';
import useGet from '../../hooks/useGet';
import Progress from '../Progress';
import { Avatar, CircularProgress, Grid, IconButton, TextField, Typography } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import '@fontsource/roboto/300.css';
import Divider from '@material-ui/core/Divider';
import RouterTwoToneIcon from '@material-ui/icons/RouterTwoTone';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Button from '@material-ui/core/Button';
import DnsTwoToneIcon from '@material-ui/icons/DnsTwoTone';
import AlarmTwoToneIcon from '@material-ui/icons/AlarmTwoTone';
import LocationOnTwoToneIcon from '@material-ui/icons/LocationOnTwoTone';
import VpnLockTwoToneIcon from '@material-ui/icons/VpnLockTwoTone';
import lightGreen from '@material-ui/core/colors/lightGreen';
import red from '@material-ui/core/colors/red';
import { useEffect, useState } from 'react';
import { Link } from "react-router-dom";
import EmailTwoToneIcon from '@material-ui/icons/EmailTwoTone';
import AccountCircleTwoToneIcon from '@material-ui/icons/AccountCircleTwoTone';
import WcTwoToneIcon from '@material-ui/icons/WcTwoTone';
import EventTwoToneIcon from '@material-ui/icons/EventTwoTone';
import Paper from '@material-ui/core/Paper';
import EditTwoToneIcon from '@material-ui/icons/EditTwoTone';
import DeleteForeverTwoToneIcon from '@material-ui/icons/DeleteForeverTwoTone';
import DescriptionTwoToneIcon from '@material-ui/icons/DescriptionTwoTone';
import Tooltip from '@material-ui/core/Tooltip';
import Accordion from '@material-ui/core/Accordion';
import AccordionDetails from '@material-ui/core/AccordionDetails';
import AccordionSummary from '@material-ui/core/AccordionSummary';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import SensorsDataChart from './SensorsDataChart';
import { useSelector, useDispatch } from 'react-redux';
import { selectUser } from '../../redux/reducers/authSlice';
import { useHistory } from 'react-router-dom';
import { MakeAuthorizedRequest, DeleteRequest, PatchRequest } from '../../requests/Requests';
import FormErrors from '../FormErrors';
import SwapVertTwoToneIcon from '@material-ui/icons/SwapVertTwoTone';
import { selectNotifications, markMicrocontrollerNotificationsAsOld } from '../../redux/reducers/notificationsSlice';
import ConfirmationPopup from '../ConfirmationPopup';
import yellow from '@material-ui/core/colors/yellow';

const useStyles = makeStyles((theme) => ({
  root: {
    paddingBottom: theme.spacing(3)
  },
  map: {
    marginBottom: theme.spacing(2)
  },
  microcontrollerName: {
    fontWeight: '300',
    marginBottom: theme.spacing(1),
    display: 'flex',
    alignItems: 'center'
  },
  titleIcon: {
    marginLeft: theme.spacing(2),
    marginRight: theme.spacing(2)
  },
  divider: {
    marginBottom: theme.spacing(2)
  },
  detailsCardsRoot: {
    display: 'flex',
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-around',
    alignItems: 'flex-end',
    marginBottom: theme.spacing(4)
  },
  detailsCard: {
    width: '600px',
    marginTop: theme.spacing(4)
  },
  detailsCardContent: {
    padding: theme.spacing(3)
  },
  detailsCardHeader: {
    fontSize: 17,
    marginBottom: theme.spacing(2)
  },
  actionsButtonsRow: {
    display: 'flex',
    width: '100%',
    justifyContent: 'center'
  },
  errorsRoot: {
    margin: theme.spacing(2),
    display: 'flex',
    justifyContent: 'center'
  },
  deleteMicrocontrollerIcon: {
    marginRight: theme.spacing(2),
    marginLeft: theme.spacing(2)
  },
  accordionRow: {
    display: 'flex',
    alignItems: 'center'
  },
  requestSensorButton: {
    marginRight: theme.spacing(2)
  }
}));

const dateTimeOptions = {
  year: 'numeric',
  month: 'long',
  day: 'numeric',
  weekday: 'long',
  hour: 'numeric',
  minute: 'numeric',
  second: 'numeric',
  hour12: false
};

export default function Microcontroller() {
  const classes = useStyles();
  const history = useHistory();
  const dispatch = useDispatch();
  const { id } = useParams();
  const { data: microcontroller, isPending } = useGet(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/GetMicrocontroller/${id}`);
  const [ userInfo, setUserInfo ] = useState(null);
  const [ expanded, setExpanded ] = useState(false);
  const [ requestedSensorID, setRequestedSensorID ] = useState(null);

  const [serverErrors, setServerErrors] = useState(null);
  const user = useSelector(selectUser);
  const [isAdministrator, setIsAdministrator] = useState(false);
  const [isOwner, setIsOwner] = useState(false);

  const notifications = useSelector(selectNotifications);

  useEffect(() => {
    if (microcontroller?.Sensors && microcontroller.Sensors.length > 0
      && notifications && notifications.length > 0) {
      const requestedSensorNotification = notifications
        .find(notification => notification.microcontrollerID === microcontroller.ID
          && notification.isOld !== undefined && !notification.isOld && notification.sensorID === requestedSensorID);

      if (requestedSensorNotification) {
        setRequestedSensorID(null);
      }
    }
  }, [notifications, microcontroller?.ID, microcontroller?.Sensors, requestedSensorID]);

  useEffect(() => {
    if (user != null && userInfo != null) {
      setIsOwner(user.UserID === userInfo.ID);
      setIsAdministrator(user.Administrator);
    }
  }, [user, userInfo])

  const handleAccordionChange = (panel) => (event, isExpanded) => {
    setExpanded(isExpanded ? panel : false);
  };

  const handleMicrocontrollerDeletion = async () => {
    setOpenConfirmationPopup(true);
  }

  useEffect(() => {
    if (microcontroller != null) {
      setUserInfo(microcontroller.UserInfo);
      setRequestedSensorID(microcontroller.RequestedSensorID);
    }
  }, [microcontroller]);

  const handleSensorValueRequest = async (sensorID) => {
    const body = {
      MicrocontrollerID: microcontroller.ID,
      SensorID: sensorID
    };

    const requestSensorValueRequestFactory = (token) =>
      PatchRequest(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/RequestSensorValue`, body, token);
  
    const response = await MakeAuthorizedRequest(requestSensorValueRequestFactory, user);

    if (response.status !== 200) {
      if (response.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (response.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setServerErrors(response.errors);
      }
    } else {
      dispatch(markMicrocontrollerNotificationsAsOld({
        microcontrollerID: microcontroller.ID,
        sensorID: response.data.CurrentRequestedSensorID
      }));

      setRequestedSensorID(response.data.CurrentRequestedSensorID);
    }
  };

  const [openConfirmationPopup, setOpenConfirmationPopup] = useState(false);

  let handleAgreeConfirmationPopupAction = async () => {
    setOpenConfirmationPopup(false);
    
    const deleteMicrocontrollerRequestFactory = (token) =>
      DeleteRequest(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/Delete/${id}`, token);
  
    const response = await MakeAuthorizedRequest(deleteMicrocontrollerRequestFactory, user);

    if (response.status !== 200) {
      if (response.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (response.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setServerErrors(response.errors);
      }
    } else {
      history.push(`/user/${user.UserID}`);
    }
  };

  let handleDisagreeConfirmationPopupAction = () => {
    setOpenConfirmationPopup(false);
  };

  return (<div className={classes.root}>
    { isPending && <Progress /> }
    { microcontroller && (
      <div>
        {/* Map */}
        { microcontroller.Latitude != null && microcontroller.Longitude != null && (
          <div className={classes.map}>
            <Map
              height={400}
              defaultCenter={[microcontroller.Latitude, microcontroller.Longitude]}
              defaultZoom={11}>
              <Marker width={50} anchor={[microcontroller.Latitude, microcontroller.Longitude]} />
            </Map>
          </div>
        )}
      
        {/* Name */}
        <Typography className={classes.microcontrollerName} variant="h3">
          <RouterTwoToneIcon fontSize="large" className={classes.titleIcon}/> {microcontroller.Name}
        </Typography>

        <Divider />
        
        {/* Details cards */}
        <div className={classes.detailsCardsRoot}>
          <MicrocontrollerDetailCard classes={classes} microcontroller={microcontroller} />
          { userInfo && (<UserDetailCard classes={classes} userInfo={userInfo} />)}
        </div>

        {/* Actions buttons */}

        { (isOwner || isAdministrator) && (
          <div>
            <Divider style={{marginBottom: '1px'}}/>
            <Paper elevation={0} className={classes.actionsButtonsRow}>
              <Tooltip title="Edit microcontroller">
                <Button onClick={() => history.push(`/edit/microcontroller/${microcontroller.ID}`)}>
                  <EditTwoToneIcon fontSize="large" />
                </Button>
              </Tooltip>
              <Tooltip title="Delete microcontroller" className={classes.deleteMicrocontrollerIcon}>
                <Button onClick={handleMicrocontrollerDeletion}>
                  <DeleteForeverTwoToneIcon fontSize="large" color="secondary"/>
                </Button>
              </Tooltip>
              <Tooltip title="Generate configuration file">
                <Button onClick={() => history.push(`/configFileGenerator/${microcontroller.ID}`)}>
                  <DescriptionTwoToneIcon fontSize="large" />
                </Button>
              </Tooltip>
            </Paper>
            <Divider className={classes.divider} />
            <ConfirmationPopup
              open={openConfirmationPopup}
              handleAgree={handleAgreeConfirmationPopupAction}
              handleDisagree={handleDisagreeConfirmationPopupAction}
              title="Delete Microcontroller?"
              content={<span>Do you realy want to delete <b>{microcontroller.Name}</b> microcontroller?</span>} />
          </div>
        )}

        {serverErrors && (
          <div className={classes.errorsRoot}>
            <FormErrors errors={serverErrors} />
          </div>
        )}

        {/* Sensors */}
        { microcontroller.Sensors && microcontroller.Sensors.map((sensor) => {
            let marginBottom = '0px';
            if (sensor.ID === expanded) {
              marginBottom = '16px';
            }

            let tooltipTitle = requestedSensorID === sensor.ID
             ? 'Sensor\'s value already requested. Pleas wait.'
             : 'Request sensor\'s value';

            return (
              <div key={sensor.ID} className={classes.accordionRow} style={{marginBottom: marginBottom}}>
                {isOwner && (
                  <Tooltip title={tooltipTitle}>
                    <span>
                      <IconButton
                        className={classes.requestSensorButton}
                        disabled={requestedSensorID === sensor.ID}
                        onClick={() => handleSensorValueRequest(sensor.ID)}>
                        <SwapVertTwoToneIcon />
                      </IconButton>
                    </span>
                  </Tooltip>
                )}

                <SensorsAccordion
                  isOwner={isOwner}
                  user={user}
                  requestedSensorID={requestedSensorID}
                  microcontroller={microcontroller}
                  sensor={sensor}
                  handleChange={handleAccordionChange}
                  expanded={expanded}/>
              </div>
            )
          }
        )}
      </div>
    )}
  </div>
  )
}

const useAccordionStyles = makeStyles((theme) => ({
  accordion: {
    marginBottom: theme.spacing(1),
    flexBasis: '100%'
  },
  header: {
    display: 'flex',
    alignItems: 'center',
    width: '100%'
  },
  avatar: {
    marginRight: theme.spacing(2)
  },
  heading: {
    fontSize: theme.typography.pxToRem(15),
    flexBasis: '25%',
    flexShrink: 0,
  },
  secondaryHeading: {
    fontSize: theme.typography.pxToRem(15),
    color: theme.palette.text.secondary,
    flexBasis: '25%'
  },
  details: {
    flexDirection: 'column'
  },
  headerDivider: {
    marginTop: -theme.spacing(1),
    marginLeft: -theme.spacing(2),
    marginRight: -theme.spacing(2),
    marginBottom: theme.spacing(2),
  },
  criticalValueRow: {
    display: 'flex',
    alignItems: 'center',
    marginBottom: theme.spacing(4),
    padding: theme.spacing(4),
    backgroundColor: yellow[50]
  },
  criticalValueRowLabel: {
    paddingBottom: theme.spacing(3/4),
    marginRight: theme.spacing(2)
  },
  criticalValueRowButton: {
    marginLeft: theme.spacing(2)
  },
  errorsRoot: {
    margin: theme.spacing(2),
    display: 'flex',
    justifyContent: 'center'
  },
}));

function SensorsAccordion(props) {
  const classes = useAccordionStyles();
  const sensor = props.sensor;
  const sensorType = sensor.SensorType;
  const [ criticalValue, setCriticalValue ] = useState(sensor.CriticalValue);
  const [ isPending, setIsPending ] = useState(false);
  const [ errors, setErrors ] = useState(null);

  const handleSaveCriticalValueClick = async () => {
    if (criticalValue !== '' && (criticalValue <= 0 || criticalValue > 10000)) {
      return;
    }

    setIsPending(true);
    const body = {
      MicrocontrollerID: props.microcontroller.ID,
      SensorID: sensor.ID,
      CriticalValue: isNaN(criticalValue) || criticalValue === ''
        ? null
        : criticalValue
    };

    const saveCriticalValueRequestFactory = (token) =>
      PatchRequest(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/SetSensorsCriticalValue`, body, token);
  
    const response = await MakeAuthorizedRequest(saveCriticalValueRequestFactory, props.user);

    setIsPending(false);

    if (response.status !== 200) {
      if (response.status === 401) {
        props.history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (response.status === 500) {
        props.history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setErrors(response.errors);
      }
    } else {
      setErrors(null);
    }
  };

  const handleCriticalValueChange = (e) => {
    let number = e.target.value;

    if (number === '') {
      setErrors(null);
    } else if (number <= 0 || number > 10000) {
      setErrors(['Critical value can\'t be ≤ 0 or > 10000']);
    } else {
      setErrors(null);
    }

    setCriticalValue(number);
  };

  return (
    <Accordion
      expanded={props.expanded === sensor.ID}
      onChange={props.handleChange(sensor.ID)}
      className={classes.accordion}>
      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
        <div className={classes.header}>
          <Avatar src={sensorType.Icon} className={classes.avatar}>{sensorType.Units}</Avatar>
          <Typography className={classes.heading}>{sensor.Name}</Typography>
          <Typography className={classes.secondaryHeading}>{sensorType.Name}</Typography>
          { props.requestedSensorID === sensor.ID && (
            <Typography className={classes.secondaryHeading}>Awaiting sensor's response...</Typography>
          )}
        </div>
      </AccordionSummary>
      <AccordionDetails className={classes.details}>
        <Divider className={classes.headerDivider} />
        
        {props.isOwner && (
          <Card className={classes.criticalValueRow} variant="outlined">
            <Typography className={classes.criticalValueRowLabel}>
              Send email notification when sensor's value reaches threshold:
            </Typography>
            <TextField
              value={criticalValue}
              onChange={handleCriticalValueChange}
              type="number"
              label="Critical value"
              helperText="Save empty field to turn off notifications" />
            <Button
              onClick={handleSaveCriticalValueClick}
              className={classes.criticalValueRowButton}
              variant="outlined"
              color="primary">
                {(isPending && (<CircularProgress size={20} />)) || (<span>Save</span>)}
            </Button>
            {errors && (
              <div className={classes.errorsRoot}>
                <FormErrors errors={errors} />
              </div>
            )}
          </Card>
        )}
        
        {sensor.Description && (
          <Typography style={{whiteSpace: 'pre-line'}}>
            {sensor.Description}
          </Typography>
        )}
        
        {props.expanded === sensor.ID && (<SensorsDataChart microcontrollerID={props.microcontroller.ID} sensor={sensor} />)}
      </AccordionDetails>
    </Accordion>
  );
}

function UserDetailCard(props) {
  const classes = props.classes;
  const userInfo = props.userInfo;

  return (
    <Link to={`/user/${userInfo.ID}`} style={{ textDecoration: 'none' }}>
      <Card className={classes.detailsCard} variant="outlined">
        <CardContent className={classes.detailsCardContent}>
          <Grid container>
            <Grid item xs={12}>
              <Typography className={classes.detailsCardHeader} color="textSecondary">
                Owner Info
              </Typography>
            </Grid>

            <CardDetailRow icon={(<EmailTwoToneIcon />)} name="Email" content={userInfo.Email} />
            <CardDetailRow icon={(<AccountCircleTwoToneIcon />)} name="Full name" content={`${userInfo.FirstName} ${userInfo.LastName || '—'}`} />
            <CardDetailRow icon={(<WcTwoToneIcon />)} name="Gender" content={userInfo.Gender != null && userInfo.Gender != '' ? userInfo.Gender : '—'} />
            <CardDetailRow icon={(<EventTwoToneIcon />)} name="Age" content={userInfo.Birthday != null ? getAge(new Date(userInfo.Birthday)) : '—'} />
          </Grid>
        </CardContent>
      </Card>
    </Link>
  );
}

function MicrocontrollerDetailCard(props) {
  const classes = props.classes;
  const microcontroller = props.microcontroller;

  return (
    <Card className={classes.detailsCard} variant="outlined">
      <CardContent className={classes.detailsCardContent}>
        <Grid container>
          <Grid item xs={12}>
            <Typography className={classes.detailsCardHeader} color="textSecondary">
              Microcontroller Info
            </Typography>
          </Grid>

          <CardDetailRow icon={(<DnsTwoToneIcon />)} name="IP" content={microcontroller.IPAddress} />
          <CardDetailRow icon={(<AlarmTwoToneIcon />)} name="Last Response Time" content={new Date(microcontroller.LastResponseTime).toLocaleString("en-GB", dateTimeOptions)} />
          <CardDetailRow icon={(<LocationOnTwoToneIcon />)} name="Position" content={(<span><b>lat:</b>&nbsp;{microcontroller.Latitude || '—'} <b>long:</b>&nbsp;{microcontroller.Longitude || '—'}</span>)} />
          <CardDetailRow icon={(<VpnLockTwoToneIcon />)} name="Privacy" content={(<span style={{color: microcontroller.Public ? lightGreen[500] : red[300]}}>{microcontroller.Public ? 'Public' : 'Private'}</span>)} />
        </Grid>
      </CardContent>
    </Card>
  );
}

function CardDetailRow(props) {
  return (
    <Grid container>
      <Grid item xs={1}>
        {props.icon}
      </Grid>
      <Grid item xs={4}>
        <Typography>
          {props.name}:
        </Typography>
      </Grid>
      <Grid item xs={7}>
        <Typography style={{textAlign: 'right'}}>
          {props.content || '—'}
        </Typography>
      </Grid>
    </Grid>
  );
}

function getAge(birthDate) {
  var today = new Date();
  var age = today.getFullYear() - birthDate.getFullYear();
  var monthDiff = today.getMonth() - birthDate.getMonth();

  return monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate()) ? age - 1 : age;
}