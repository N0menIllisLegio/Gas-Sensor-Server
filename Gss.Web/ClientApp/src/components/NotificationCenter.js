import { Avatar, Badge, Divider, IconButton, List, ListItem, ListItemAvatar, ListItemSecondaryAction, ListItemText, makeStyles, Paper, Popover, Typography } from '@material-ui/core';
import NotificationsNoneTwoToneIcon from '@material-ui/icons/NotificationsNoneTwoTone';
import { useEffect, useState } from 'react';
import '@fontsource/roboto/300.css';
import DeleteTwoToneIcon from '@material-ui/icons/DeleteTwoTone';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useSelector, useDispatch } from 'react-redux';
import { selectUser } from '../redux/reducers/authSlice';
import { selectNotifications, addNotification, removeNotification } from '../redux/reducers/notificationsSlice';
import { useHistory } from 'react-router-dom';

const useStyles = makeStyles((theme) => ({
  bellButton: {
    color: theme.palette.primary.contrastText
  },
  notificationPaper: {
    width: theme.spacing(50),
  },
  headerContainer: {
    backgroundColor: theme.palette.primary.light,
    color: theme.palette.primary.contrastText,
    display: 'flex',
    justifyContent: 'center',
    padding: theme.spacing(2),
  },
  notificationList: {
    height: theme.spacing(40) - theme.spacing(6),
    overflow: 'auto',
    padding: '0'
  },
  listItemDivider: {
    marginLeft: theme.spacing(2),
    marginRight: theme.spacing(2),
    marginTop: theme.spacing(1)
  },
  listItem: {
    marginTop: theme.spacing(1)
  },
  noNotificationsString: {
    margin: theme.spacing(4),
    display: 'flex',
    justifyContent: 'center'
  }
}));

export default function NotificationCenter() {
  const classes = useStyles();
  const history = useHistory();
  const dispatch = useDispatch();
  const [ anchorEl, setAnchorEl ] = useState(null);
  const [ connection, setConnection ] = useState(null);

  const accessToken = useSelector(selectUser)?.AccessToken;
  const notifications = useSelector(selectNotifications);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${process.env.REACT_APP_SERVER_URL}api/notifications?access_token=${accessToken}`)
      .configureLogging(LogLevel.Critical)
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, [accessToken]);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(result => {
          connection.on('Notification', notification => {
            dispatch(addNotification(notification));
          });
        })
        .catch(e => console.log('Connection failed: ', e));
    }
  }, [connection, dispatch]);

  const handleDeleteNotification = (id) => {
    dispatch(removeNotification(id));
  }

  const handleOpenPopoverClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);

  return (
    <div>
      <IconButton className={classes.bellButton} onClick={handleOpenPopoverClick}>
        <Badge color="secondary" variant="dot" invisible={!notifications || notifications.length === 0}>
          <NotificationsNoneTwoToneIcon />
        </Badge>
      </IconButton>

      <Popover
        open={open}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'center',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'center',
        }}>
          <Paper className={classes.notificationPaper} elevation={0}>
            <Paper className={classes.headerContainer} elevation={4} square>
              <Typography style={{fontWeight: '300'}} variant="h5">
                Notifications
              </Typography>
            </Paper>

            {(notifications && notifications.length > 0 && (
              <List dense className={classes.notificationList}>
                { notifications.map((notification, index) => (
                  <div key={index}>
                    <ListItem className={classes.listItem} button
                      onClick={() => history.push(`/microcontroller/${notification.microcontrollerID}`)}>
                      <ListItemAvatar>
                        <Avatar src={notification.sensorTypeIcon}>
                          {notification.sensorTypeUnits}
                        </Avatar>
                      </ListItemAvatar>
                      <ListItemText primary={(
                        <Typography>
                          <b>{notification.sensorName}</b> - <b>{notification.sensorValue}&nbsp;{notification.sensorTypeUnits}</b>
                        </Typography>
                      )} secondary={notification.sensorType} />
                      <ListItemSecondaryAction>
                        <IconButton edge="end" onClick={() => handleDeleteNotification(notification.ID)}>
                          <DeleteTwoToneIcon color="secondary"/>
                        </IconButton>
                      </ListItemSecondaryAction>
                    </ListItem>
                    <Divider className={classes.listItemDivider} />
                  </div>
                )) }
              </List>
            )) || (
              <div className={classes.noNotificationsString}>
                <Typography variant="caption">
                  No notifications received!
                </Typography>
              </div>
            )}
          </Paper>
      </Popover>
    </div>
  );
}