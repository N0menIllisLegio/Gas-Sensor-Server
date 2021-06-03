import { Map, Marker } from 'pigeon-maps';
import { makeStyles } from '@material-ui/core/styles';
import { useState } from 'react';
import { useVisibleMicrocontrollers } from '../hooks/usePost';
import { useHistory } from 'react-router-dom';
import { Avatar, List, ListItem, ListItemAvatar, ListItemText, Popover, Typography } from '@material-ui/core';
import '@fontsource/roboto/300.css';

const useStyles = makeStyles((theme) => ({
  root: {
    height: `calc(100vh - (${theme.spacing(theme.mainContent.marginTop)}px + ${theme.spacing(theme.mainContent.padding)}px * 2))`,
  },
  listItemText: {
    fontWeight: '300'
  }
}));

export default function MicrocontrollersMap() {
  const classes = useStyles();
  const history = useHistory();
  const [neLatitude, setNeLatitude ] = useState(null);
  const [neLongitude, setNeLongitude ] = useState(null);
  const [swLatitude, setSwLatitude ] = useState(null);
  const [swLongitude, setSwLongitude ] = useState(null);

  const [anchorEl, setAnchorEl] = useState(null);
  const [popoverContent, setPopoverContent] = useState(null);

  const handlePopoverOpen = (event, content) => {
    event.preventDefault();
    setPopoverContent(content);
    setAnchorEl(event.currentTarget);
  };

  const handlePopoverClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);

  const { data: visibleMicrocontrollers } = useVisibleMicrocontrollers(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/GetPublicMicrocontrollersMap`,
    neLatitude, neLongitude, swLatitude, swLongitude);

  return (
    <div className={classes.root}>
      <Map
        center={[30, 0]}
        zoom={3}
        onBoundsChanged={e => {
          setNeLatitude(e.bounds.ne[0]);
          setNeLongitude(e.bounds.ne[1]);

          setSwLatitude(e.bounds.sw[0]);
          setSwLongitude(e.bounds.sw[1]);
        }}>
          { visibleMicrocontrollers && visibleMicrocontrollers.length > 0 && (
            visibleMicrocontrollers.map(mc => (
              <Marker
                key={mc.MicrocontrollerID}
                width={50}
                anchor={[mc.Latitude, mc.Longitude]}
                onContextMenu={(e) => handlePopoverOpen(e.event, mc.SensorTypes)}
                onClick={() => history.push(`/microcontroller/${mc.MicrocontrollerID}`)} />
            ))
          )}
      </Map>
      
      { popoverContent && (
        <Popover
          open={open}
          anchorEl={anchorEl}
          onClose={handlePopoverClose}
          anchorOrigin={{
            vertical: 'bottom',
            horizontal: 'center',
          }}
          transformOrigin={{
            vertical: 'top',
            horizontal: 'center',
          }}>
            <List dense>
              { popoverContent.map(sensorType => (
                <ListItem key={sensorType.ID}>
                  <ListItemAvatar>
                    <Avatar src={sensorType.Icon}>
                     {sensorType.Unit}
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText>
                    <Typography className={classes.listItemText} variant="caption">
                      {sensorType.Name}
                    </Typography>
                  </ListItemText>
                </ListItem>
              )) }
            </List>
        </Popover>
      )}
    </div>
  );
}