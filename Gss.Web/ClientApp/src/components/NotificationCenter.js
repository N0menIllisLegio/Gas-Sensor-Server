import { Avatar, Badge, Divider, IconButton, List, ListItem, ListItemAvatar, ListItemSecondaryAction, ListItemText, makeStyles, Paper, Popover, Typography } from '@material-ui/core';
import NotificationsNoneTwoToneIcon from '@material-ui/icons/NotificationsNoneTwoTone';
import { useState } from 'react';
import '@fontsource/roboto/300.css';
import DeleteTwoToneIcon from '@material-ui/icons/DeleteTwoTone';

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
  const [ anchorEl, setAnchorEl ] = useState(null);
  const [ notifications, setNotifications ] = useState([ 1,2,3,4 ]);

  const handleOpenPopoverClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const open = Boolean(anchorEl);

  const handleDeleteNotification = (id) => {
    setNotifications(notifications.filter(notification => notification != id));
  }

  return (
    <div>
      <IconButton className={classes.bellButton} onClick={handleOpenPopoverClick}>
        <Badge color="secondary" variant="dot" invisible={notifications.length === 0}>
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
                { notifications.map(notification => (
                  <div key={notification}>
                    <ListItem className={classes.listItem} >
                      <ListItemAvatar>
                        <Avatar>
                          K
                        </Avatar>
                      </ListItemAvatar>
                      <ListItemText primary={(
                        <Typography>
                          Received value: <b>1234&nbsp;ooc</b> from sensor: <i>OMEGALUL</i>
                        </Typography>
                      )} secondary="Air Quality sensor" />
                      <ListItemSecondaryAction>
                        <IconButton edge="end" onClick={() => handleDeleteNotification(notification)}>
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