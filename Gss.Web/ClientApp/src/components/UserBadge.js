import { Avatar, Typography } from '@material-ui/core';
import Button from '@material-ui/core/Button';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import { useEffect, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { MakeAuthorizedRequest, GetRequest } from '../requests/Requests';
import { selectUser, selectUserBadgeName, selectUserBadgeAvatarSrc, logout, saveBadgeData } from '../redux/reducers/authSlice';
import { makeStyles } from '@material-ui/core/styles';
import { Link } from 'react-router-dom';
import { useHistory } from "react-router-dom";

const useStyles = makeStyles((theme) => ({
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
    height: theme.spacing(theme.mainContent.marginTop)
  }
}));

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

  const handleLogout = () => {
    dispatch(logout());
    history.push('/');
  };

  useEffect(() => {
    if (user != null) {
      const getUserRequestFactory = (token) => GetRequest(`${process.env.REACT_APP_SERVER_URL}api/Users/GetUserByID/${user.UserID}`, token);

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
  }, [user?.UserID]);

  return userName ? (
      <div>
        <Button aria-haspopup="true" variant="contained" color="primary" onClick={handleClick} disableElevation>
          <Avatar src={userAvatarSrc} className={classes.avatar}/>
          <Typography className={classes.name}>{userName}</Typography>
        </Button>

        <Menu
          getContentAnchorEl={null}
          anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
          transformOrigin={{ vertical: "top", horizontal: "center" }}
          anchorEl={anchorElement}
          keepMounted
          open={Boolean(anchorElement)}
          onClose={handleClose}>
          <MenuItem onClick={() => history.push(`/user/${user.UserID}`)}>Profile</MenuItem>
          <MenuItem onClick={handleLogout} className={classes.logout}>Logout</MenuItem>
        </Menu>
      </div>
    ) : (
    <Link to="/login" className={classes.link}>
      <Button color="inherit">Login</Button>
    </Link>
  );
}