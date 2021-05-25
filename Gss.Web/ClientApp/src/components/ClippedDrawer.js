import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import Toolbar from '@material-ui/core/Toolbar';
import List from '@material-ui/core/List';
import Divider from '@material-ui/core/Divider';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import { Link } from "react-router-dom";
import PeopleAltTwoToneIcon from '@material-ui/icons/PeopleAltTwoTone';
import RouterTwoToneIcon from '@material-ui/icons/RouterTwoTone';
import MemoryTwoToneIcon from '@material-ui/icons/MemoryTwoTone';
import AccountTreeTwoToneIcon from '@material-ui/icons/AccountTreeTwoTone';
import { useSelector } from 'react-redux';
import { selectUser } from '../redux/reducers/authSlice';
import { useState, useEffect } from 'react';
import SettingsTwoToneIcon from '@material-ui/icons/SettingsTwoTone';
import MapTwoToneIcon from '@material-ui/icons/MapTwoTone';
import NotesTwoToneIcon from '@material-ui/icons/NotesTwoTone';

const drawerWidth = '17vw';

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
  },
  text: {
    whiteSpace: 'nowrap',
    overflow: 'hidden',
    textOverflow: 'ellipsis',
  }
}));

export default function ClippedDrawer() {
  const classes = useStyles();
  const user = useSelector(selectUser);
  const [displayAdminPanel, setDisplayAdminPanel] = useState(false);

  useEffect(() => {
    setDisplayAdminPanel(user?.Administrator === true);
  }, [user?.Administrator])

  return (
    <Drawer
      className={classes.drawer}
      variant="permanent"
      classes={{ paper: classes.drawerPaper }}>
        <Toolbar />
        <div className={classes.drawerContainer}>
          <List>
            <Link to="/" key="Map" className={classes.link}>
              <ListItem button>
                <ListItemIcon><MapTwoToneIcon /></ListItemIcon>
                <ListItemText primary="Map" className={classes.text} />
              </ListItem>
            </Link>
            <Link to="/public/microcontrollers" key="Public MCs" className={classes.link}>
              <ListItem button>
                <ListItemIcon><NotesTwoToneIcon /></ListItemIcon>
                <ListItemText primary="Public microcontrollers" className={classes.text} />
              </ListItem>
            </Link>
            <Link to="/configFileGenerator" key="Config file" className={classes.link}>
              <ListItem button>
                <ListItemIcon><SettingsTwoToneIcon /></ListItemIcon>
                <ListItemText primary="Config file generator" className={classes.text} />
              </ListItem>
            </Link>
          </List>

          <Divider />
          
          <List hidden={!displayAdminPanel}>
            <Link to="/users" key="users" className={classes.link}>
              <ListItem button>
                <ListItemIcon><PeopleAltTwoToneIcon /></ListItemIcon>
                <ListItemText primary="Users" className={classes.text} />
              </ListItem>
            </Link>
            <Link to="/microcontrollers" key="microcontrollers" className={classes.link}>
              <ListItem button>
                <ListItemIcon><RouterTwoToneIcon /></ListItemIcon>
                <ListItemText primary="Microcontrollers" className={classes.text} />
              </ListItem>
            </Link>
            <Link to="/sensors" key="sensors" className={classes.link}>
              <ListItem button>
                <ListItemIcon><MemoryTwoToneIcon /></ListItemIcon>
                <ListItemText primary="Sensors" className={classes.text} />
              </ListItem>
            </Link>
            <Link to="/sensorTypes" key="sensorTypes" className={classes.link}>
              <ListItem button>
                <ListItemIcon><AccountTreeTwoToneIcon /></ListItemIcon>
                <ListItemText primary="Sensor Types" className={classes.text} />
              </ListItem>
            </Link>
          </List>
        </div>
    </Drawer>
  );
}