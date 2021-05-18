import { useEffect, useState } from "react";
import { usePagedPost } from "../../hooks/usePost";
import Progress from "../Progress";
import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Typography from '@material-ui/core/Typography';
import { FormControl, Grid, InputLabel, MenuItem, Select, TextField } from "@material-ui/core";
import DnsTwoToneIcon from '@material-ui/icons/DnsTwoTone';
import AlarmTwoToneIcon from '@material-ui/icons/AlarmTwoTone';
import LocationOnTwoToneIcon from '@material-ui/icons/LocationOnTwoTone';
import VpnLockTwoToneIcon from '@material-ui/icons/VpnLockTwoTone';
import lightGreen from '@material-ui/core/colors/lightGreen';
import red from '@material-ui/core/colors/red';
import { Link } from "react-router-dom";
import { Pagination } from "@material-ui/lab";

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'center'
  },
  card: {
    width: '600px',
    margin: theme.spacing(2),
    cursor: 'pointer'
  },
  title: {
    fontSize: 17,
    marginBottom: theme.spacing(2)
  },
  inputsRow: {
    marginBottom: theme.spacing(2),
    display: 'flex',
    justifyContent: 'space-between', 
    alignItems: 'center'
  },
  formControl: {
    margin: theme.spacing(1),
    minWidth: 120,
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

export default function UserMicrocontrollersList(props) {
  const classes = useStyles();
  const [searchString, setSearchString] = useState('');
  const [totalPagesNumber, setTotalPagesNumber] = useState(1);
  const [currentPageNumber, setCurrentPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const { data: pagedResponse, isPending } = usePagedPost(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/GetUserMicrocontrollers/${props.userID}`, currentPageNumber, pageSize, searchString);

  const [microcontrollers, setMicrocontrollers] = useState(null);
  
  useEffect(() => {
    if (pagedResponse != null) {
      setMicrocontrollers(pagedResponse.Items);
      setTotalPagesNumber(Math.ceil(pagedResponse.TotalItemsCount / pagedResponse.PagedInfo.PageSize));
    }
  }, [pagedResponse]);

  useEffect(() => {
    setCurrentPageNumber(1);
  }, [searchString]);
  
  return (
    <div>
      { isPending && (<Progress />) }
      { microcontrollers && (
        <div>
          <div className={classes.inputsRow}>
            <TextField label="Search..." value={searchString} onChange={(e) => setSearchString(e.target.value)}/>

            {totalPagesNumber > 1 &&
              (
                <Pagination
                  variant="outlined"
                  color="primary"
                  count={totalPagesNumber}
                  page={currentPageNumber}
                  onChange={(e, pageNumber) => setCurrentPageNumber(pageNumber)} />
              )
            }

            <FormControl className={classes.formControl}>
              <InputLabel>MCs</InputLabel>
              <Select label="MCs"
                value={pageSize}
                onChange={(e) => setPageSize(e.target.value)}>
                <MenuItem value={10}>10</MenuItem>
                <MenuItem value={20}>20</MenuItem>
                <MenuItem value={30}>30</MenuItem>
              </Select>
            </FormControl>
          </div>
          <div className={classes.root}>
            { microcontrollers && microcontrollers.map(microcontroller => (
              <MicrocontrollersDetailCard key={microcontroller.ID} classes={classes} microcontroller={microcontroller} />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

function MicrocontrollersDetailCard(props) {
  const classes = props.classes;
  const microcontroller = props.microcontroller;
  const [ cardRaised, setCardRaised ] = useState(false);

  return (
    <Link to={`/microcontroller/${microcontroller.ID}`} style={{ textDecoration: 'none' }}>
      <Card className={classes.card} raised={cardRaised} onMouseEnter={() => setCardRaised(true)} onMouseLeave={() => setCardRaised(false)}>
        <CardContent>
          <Grid container>
            <Grid item xs={12}>
              <Typography className={classes.title} color="textSecondary" gutterBottom>
                {microcontroller.Name}
              </Typography>
            </Grid>

            <MicrocontrollerDetail icon={(<DnsTwoToneIcon />)} name="IP" content={microcontroller.IPAddress} />
            <MicrocontrollerDetail icon={(<AlarmTwoToneIcon />)} name="Last Response Time" content={new Date(microcontroller.LastResponseTime).toLocaleString("en-US", dateTimeOptions)} />
            <MicrocontrollerDetail icon={(<LocationOnTwoToneIcon />)} name="Position" content={(<span><b>lat:</b> {microcontroller.Latitude || '—'} <b>long:</b> {microcontroller.Longitude || '—'}</span>)} />
            <MicrocontrollerDetail icon={(<VpnLockTwoToneIcon />)} name="Privacy" content={(<span style={{color: microcontroller.Public ? lightGreen[500] : red[300]}}>{microcontroller.Public ? 'Public' : 'Private'}</span>)} />
          </Grid>
        </CardContent>
      </Card>
    </Link>
  );
}

function MicrocontrollerDetail(props) {
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