import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import Toolbar from '@material-ui/core/Toolbar';
import List from '@material-ui/core/List';
import Divider from '@material-ui/core/Divider';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import InboxIcon from '@material-ui/icons/MoveToInbox';
import { Link } from "react-router-dom";
import PeopleAltTwoToneIcon from '@material-ui/icons/PeopleAltTwoTone';
import RouterIcon from '@material-ui/icons/Router';
import MemoryIcon from '@material-ui/icons/Memory';

const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  drawerContainer: {
    overflow: 'auto',
  },
  link: {
    color: 'inherit',
    textDecoration: 'none',
  }
}));

export default function ClippedDrawer() {
  const classes = useStyles();

  return (
    <Drawer
      className={classes.drawer}
      variant="permanent"
      classes={{ paper: classes.drawerPaper }}>
        <Toolbar />
        <div className={classes.drawerContainer}>
          <List>
            <Link to="/" key="123" className={classes.link}>
              <ListItem button>
                <ListItemIcon><InboxIcon /></ListItemIcon>
                <ListItemText primary="Main" />
              </ListItem>
            </Link>
          </List>
          <Divider />
          <List>
            <Link to="/users" key="users" className={classes.link}>
              <ListItem button>
                <ListItemIcon><PeopleAltTwoToneIcon /></ListItemIcon>
                <ListItemText primary="Users" />
              </ListItem>
            </Link>
            <Link to="/microcontrollers" key="microcontrollers" className={classes.link}>
              <ListItem button>
                <ListItemIcon><RouterIcon /></ListItemIcon>
                <ListItemText primary="Microcontrollers" />
              </ListItem>
            </Link>
            <Link to="/sensors" key="sensors" className={classes.link}>
              <ListItem button>
                <ListItemIcon><MemoryIcon /></ListItemIcon>
                <ListItemText primary="Sensors" />
              </ListItem>
            </Link>
          </List>
        </div>
    </Drawer>
  );
}