import { Avatar, ListItemIcon, ListItemText, Tooltip, Typography } from '@material-ui/core';
import Button from '@material-ui/core/Button';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import { useEffect, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { MakeAuthorizedRequest, GetRequest, PostRequest } from '../requests/Requests';
import { selectUser, selectUserBadgeName, selectUserBadgeAvatarSrc, logout, saveBadgeData } from '../redux/reducers/authSlice';
import { makeStyles, withStyles } from '@material-ui/core/styles';
import { Link } from 'react-router-dom';
import { useHistory } from "react-router-dom";
import NotificationCenter from './NotificationCenter';

import PhonelinkEraseTwoToneIcon from '@material-ui/icons/PhonelinkEraseTwoTone';
import ExitToAppTwoToneIcon from '@material-ui/icons/ExitToAppTwoTone';
import AccountBoxTwoToneIcon from '@material-ui/icons/AccountBoxTwoTone';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexDirection: 'row',
    alignItems: 'center'
  },
  avatar: {
    margin: theme.spacing(1),
  },
  name: {
    marginLeft: theme.spacing(1),
    marginRight: theme.spacing(1),
    fontWeight: 'bold',
  },
  link: {
    color: 'inherit',
    textDecoration: 'none',
    margin: theme.spacing(2)
  },
  logout: {
    color: theme.palette.secondary.main
  },
  button: {
    height: theme.spacing(theme.mainContent.marginTop),
    marginLeft: theme.spacing(2)
  },
  menuIcon: {
    minWidth: '0px',
    marginRight: theme.spacing(1)
  }
}));

const StyledMenu = withStyles({
  paper: {
    border: '1px solid #d3d4d5',
    width: '200px'
  },
})((props) => (
  <Menu
    getContentAnchorEl={null}
    anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
    transformOrigin={{ vertical: "top", horizontal: "center" }}
    {...props}
  />
));

export default function UserBadge() {
  const classes = useStyles();
  const history = useHistory();
  const [anchorElement, setAnchorElement] = useState(null);
  const dispatch = useDispatch();
  const user = useSelector(selectUser);
  const userName = useSelector(selectUserBadgeName);
  const userAvatarSrc = useSelector(selectUserBadgeAvatarSrc);

  const handleClick = (event) => {
    setAnchorElement(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorElement(null);
  };

  const handleProfileClick = () => {
    handleClose();
    history.push(`/user/${user.UserID}`);
  };

  const handleLogout = () => {
    handleClose();
    dispatch(logout());

    const logoutRequestFactory = (token) =>
      PostRequest(`${process.env.REACT_APP_SERVER_URL}api/Authorization/LogOut`, {
        AccessToken: user.AccessToken,
        RefreshToken: user.RefreshToken
      }, token);

    MakeAuthorizedRequest(logoutRequestFactory, user);

    history.push('/');
  };

  const handleLogoutAll = () => {
    handleClose();
    dispatch(logout());

    const logoutRequestFactory = (token) =>
      PostRequest(`${process.env.REACT_APP_SERVER_URL}api/Authorization/LogOutFromAllDevices`, {
        AccessToken: user.AccessToken,
        RefreshToken: user.RefreshToken
      }, token);

    MakeAuthorizedRequest(logoutRequestFactory, user);

    history.push('/');
  };

  useEffect(() => {
    if (user != null) {
      const getUserRequestFactory = (token) => GetRequest(`${process.env.REACT_APP_SERVER_URL}api/Users/GetExtendedUserByID/${user.UserID}`, token);

      MakeAuthorizedRequest(getUserRequestFactory, user)
        .then(response => {
          if (response.status === 200) {
            dispatch(saveBadgeData({
              userBadgeName: response.data.FirstName,
              userBadgeAvatarSrc: response.data.AvatarPath
            }));
          }
        });
    } else {
      dispatch(saveBadgeData({
        userBadgeName: null,
        userBadgeAvatarSrc: null
      }));
    }
  }, [user, dispatch]);

  return userName ? (
      <div className={classes.root}>
        <NotificationCenter />
        
        <Button
          className={classes.button}
          aria-haspopup="true"
          variant="contained"
          color="primary"
          onClick={handleClick}
          disableElevation>
          <Avatar src={userAvatarSrc} className={classes.avatar}/>
          <Typography className={classes.name}>{userName}</Typography>
        </Button>

        <StyledMenu
          keepMounted
          anchorEl={anchorElement}
          open={Boolean(anchorElement)}
          onClose={handleClose}>
          <MenuItem onClick={handleProfileClick}>
            <ListItemIcon className={classes.menuIcon}>
              <AccountBoxTwoToneIcon fontSize="small" />
            </ListItemIcon>
            <ListItemText primary="Profile" />
          </MenuItem>
          <MenuItem onClick={handleLogout} className={classes.logout}>
            <ListItemIcon className={classes.menuIcon}>
              <ExitToAppTwoToneIcon fontSize="small" color="secondary" />
            </ListItemIcon>
            <ListItemText primary="Logout" />
          </MenuItem>
          <MenuItem onClick={handleLogoutAll} className={classes.logout}>
            <ListItemIcon className={classes.menuIcon}>
              <PhonelinkEraseTwoToneIcon fontSize="small" color="secondary" />
            </ListItemIcon>
            <Tooltip title="Logout from all devices">
              <ListItemText primary="Logout All" />
            </Tooltip>
          </MenuItem>
        </StyledMenu>
      </div>
    ) : (
    <Link to="/login" className={classes.link}>
      <Button color="inherit">Login</Button>
    </Link>
  );
}