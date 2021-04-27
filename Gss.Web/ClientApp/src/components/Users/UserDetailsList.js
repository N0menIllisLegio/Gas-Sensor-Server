import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import ListItemAvatar from '@material-ui/core/ListItemAvatar';
import Avatar from '@material-ui/core/Avatar';
import EmailTwoToneIcon from '@material-ui/icons/EmailTwoTone';
import WcTwoToneIcon from '@material-ui/icons/WcTwoTone';
import CakeTwoToneIcon from '@material-ui/icons/CakeTwoTone';
import amber from '@material-ui/core/colors/amber';
import lightGreen from '@material-ui/core/colors/lightGreen';
import red from '@material-ui/core/colors/red';
import yellow from '@material-ui/core/colors/yellow';
import indigo from '@material-ui/core/colors/indigo';
import TimerOffTwoToneIcon from '@material-ui/icons/TimerOffTwoTone';

const useStyles = makeStyles((theme) => ({
  root: {
    width: '100%',
    maxWidth: 'inherit',
    backgroundColor: theme.palette.background.paper,
  },
  green: {
    color: theme.palette.getContrastText(lightGreen[500]),
    backgroundColor: lightGreen[500],
  },
  amber: {
    color: theme.palette.getContrastText(amber[500]),
    backgroundColor: amber[500],
  },
  red: {
    color: theme.palette.getContrastText(red[500]),
    backgroundColor: red[500],
  },
  yellow: {
    color: theme.palette.getContrastText(yellow[500]),
    backgroundColor: yellow[500],
  },
  indigo: {
    color: theme.palette.getContrastText(indigo[500]),
    backgroundColor: indigo[500],
  },
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

export default function UserDetailsList(props) {
  const classes = useStyles();
  const user = props.user;

  return (
    <List className={classes.root}>
      <ListItem>
        <ListItemAvatar>
          <Avatar className={
            user.EmailConfirmed != null
            ? user.EmailConfirmed ? classes.green : classes.amber
            : null
          }>
            <EmailTwoToneIcon />
          </Avatar>
        </ListItemAvatar>
        <ListItemText primary="Email" secondary={user.Email} />
      </ListItem>
      <ListItem>
        <ListItemAvatar>
          <Avatar className={classes.indigo}>
            <WcTwoToneIcon />
          </Avatar>
        </ListItemAvatar>
        <ListItemText primary="Gender" secondary={user.Gender} />
      </ListItem>
      <ListItem>
        <ListItemAvatar>
          <Avatar className={classes.yellow}>
            <CakeTwoToneIcon />
          </Avatar>
        </ListItemAvatar>
        <ListItemText primary="Birthday" secondary={new Date(user.Birthday).toLocaleString("en-US", dateTimeOptions)} />
      </ListItem>
      { user.LockoutEnabled != null && user.LockoutEnabled && new Date(user.LockoutEnd) > Date.now() && (
        <ListItem>
          <ListItemAvatar>
            <Avatar className={classes.red}>
              <TimerOffTwoToneIcon />
            </Avatar>
          </ListItemAvatar>
          <ListItemText primary="Lockout Ends" secondary={new Date(user.LockoutEnd).toLocaleString("en-US", dateTimeOptions)} />
        </ListItem>
      )}
    </List>
  );
}


  /* 
  {user.CreationDate}

  {user.LockoutEnabled}
  {user.LockoutEnd} */