import { makeStyles } from '@material-ui/core/styles';
import { AppBar, Avatar, Toolbar, Typography } from '@material-ui/core';
import { Link } from 'react-router-dom';
import UserBadge from './UserBadge';
import NotificationCenter from './NotificationCenter';

const useStyles = makeStyles((theme) => ({
  icon: {
    marginRight: theme.spacing(2),
  },
  title: {
    flexGrow: 1,
  },
  appBar: {
    zIndex: theme.zIndex.drawer + 1,
    height: theme.spacing(theme.mainContent.marginTop)
  },
  link: {
    color: 'inherit',
    textDecoration: 'none',
  }
}));

export default function Navbar() {
  const classes = useStyles();

  return (
    <AppBar position="fixed" className={classes.appBar}>
      <Toolbar>
        <Link to="/" className={classes.link}>
          <Avatar variant="square" src={`${process.env.PUBLIC_URL}/favicon.ico`} edge="start" className={classes.icon} />
        </Link>
        <Typography variant="h6" className={classes.title}>
          Sensors System
        </Typography>

        <NotificationCenter />
        <UserBadge />
      </Toolbar>
    </AppBar>
  );
}