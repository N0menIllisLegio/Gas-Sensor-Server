import PagedTable from "./PagedTable";
import { Link } from 'react-router-dom'
import { Button } from "@material-ui/core";
import { useState } from "react";
import Popover from '@material-ui/core/Popover';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';

const columns = [
  { field: 'Name', headerName: 'Name', flex: 0.8 },
  { field: 'Latitude', headerName: 'Latitude', flex: 0.8 },
  { field: 'Longitude', headerName: 'Longitude', flex: 0.8 },
  { field: 'LastResponseTime', headerName: 'LRT', flex: 0.8, type: 'dateTime', description: 'Last Response Time', valueFormatter: (params) => new Date(params.value).toUTCString() },
  { field: 'Sensors', headerName: 'Sensors Connected', flex: 0.5,
    sortComparator: (v1, v2) => {
      if (v1 !== null && v2 !== null) {
        return v1.length - v2.length;
      } else if (v1 === v2) {
        return 0;
      } else if (v1 === null) {
        return -1;
      } else {
        return 1;
      }
    },
    renderCell: (params) => (<Sensors cellContent={params.value.length} popoverContent={params.value} />)},
  { field: 'UserInfo', headerName: 'Owner Email', flex: 1, align: 'center', headerAlign: 'center',
    sortComparator: (v1, v2) => {
      if (v1 !== null && v2 !== null) {
        return v1.Email.localeCompare(v2.Email);
      } else if (v1 === v2) {
        return 0;
      } else if (v1 === null) {
        return -1;
      } else {
        return 1;
      }
    },
    renderCell: (params) => params.value === null ? (<div>—</div>) : (
    <Link to={`/user/${params.value.ID}`} style={{color: 'inherit', textDecoration: 'none'}}>
      <Button variant="contained" color="primary" disableElevation>
       {params.value.Email}
      </Button>
    </Link>
  )}
];

export default function MicrocontrollersPublicTable() {
  return (
    <PagedTable columns={columns} url={'api/Microcontrollers/GetPublicMicrocontrollers'} detailsUrl={'/microcontroller/'} />
  )
}

const useStyles = makeStyles((theme) => ({
  popover: {
    pointerEvents: 'none',
  },
  paper: {
    padding: theme.spacing(1),
  },
  content: {
    paddingRight: theme.spacing(4)
  }
}));

function Sensors(props) {
  const classes = useStyles();
  const [anchorEl, setAnchorEl] = useState(null);

  const handlePopoverOpen = (event) => {
    if (props.popoverContent != null && props.popoverContent.length > 0) {
      setAnchorEl(event.currentTarget);
    }
  };

  const handlePopoverClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);

  return (
    <div>
      <Typography
        aria-owns={open ? 'mouse-over-popover' : undefined}
        aria-haspopup="true"
        onMouseEnter={handlePopoverOpen}
        onMouseLeave={handlePopoverClose}>
        {props.cellContent}
      </Typography>

      <Popover
        className={classes.popover}
        classes={{
          paper: classes.paper,
        }}
        open={open}
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'left',
        }}
        onClose={handlePopoverClose}
        disableRestoreFocus>
          <ul className={classes.content}>
            {props.popoverContent?.map(sensor => (
              <li key={sensor.ID}>{sensor.Name}</li>
            ))}
          </ul>
      </Popover>
    </div>
  );
}