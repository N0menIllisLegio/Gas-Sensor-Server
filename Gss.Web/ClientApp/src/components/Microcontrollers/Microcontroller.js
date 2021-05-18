import { useParams } from 'react-router-dom';
import { Map, Marker } from 'pigeon-maps';
import useGet from '../../hooks/useGet';
import Progress from '../Progress';
import { Grid, IconButton, Typography } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import '@fontsource/roboto/300.css';
import Divider from '@material-ui/core/Divider';
import RouterTwoToneIcon from '@material-ui/icons/RouterTwoTone';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
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

const useStyles = makeStyles((theme) => ({
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
  const { id } = useParams();
  const { data: microcontroller, isPending, errors } = useGet(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/GetMicrocontroller/${id}`);
  const [ userInfo, setUserInfo ] = useState(null);

  useEffect(() => {
    if (microcontroller != null) {
      setUserInfo(microcontroller.UserInfo);
    }
  }, [microcontroller])

  return (<div>
    { isPending && <Progress /> }
    { microcontroller && (
      <div>
        {/* Map */}
        { microcontroller.Latitude && microcontroller.Longitude && (
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
        <Divider  style={{marginBottom: '1px'}}/>
          <Paper elevation={0} className={classes.actionsButtonsRow}>
            <Tooltip title="Edit microcontroller">
              <Button>
                <EditTwoToneIcon fontSize="large" />
              </Button>
            </Tooltip>
            <Tooltip title="Delete microcontroller" style={{marginLeft: '16px', marginRight: '16px'}}>
              <Button>
                <DeleteForeverTwoToneIcon fontSize="large" />
              </Button>
            </Tooltip>
            <Tooltip title="Generate configuration file">
              <Button>
                <DescriptionTwoToneIcon fontSize="large" />
              </Button>
            </Tooltip>
          </Paper>
        <Divider className={classes.divider} />

      </div>
    )}
  </div>
  )
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
            <CardDetailRow icon={(<WcTwoToneIcon />)} name="Gender" content={userInfo.Gender} />
            <CardDetailRow icon={(<EventTwoToneIcon />)} name="Age" content={getAge(new Date(userInfo.Birthday))} />
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
          <CardDetailRow icon={(<AlarmTwoToneIcon />)} name="Last Response Time" content={new Date(microcontroller.LastResponseTime).toLocaleString("en-US", dateTimeOptions)} />
          <CardDetailRow icon={(<LocationOnTwoToneIcon />)} name="Position" content={(<span><b>lat:</b> {microcontroller.Latitude || '—'} <b>long:</b> {microcontroller.Longitude || '—'}</span>)} />
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