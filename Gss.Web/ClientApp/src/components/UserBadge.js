import { Avatar, Typography } from '@material-ui/core';
import Button from '@material-ui/core/Button';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import { useEffect, useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { MakeAuthorizedRequest, GetRequest } from '../requests/Post';
import { selectUser, logout } from '../redux/reducers/authSlice';
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
  }
}));

export default function UserBadge() {
  const classes = useStyles();
  const history = useHistory();
  const [anchorElement, setAnchorElement] = useState(null);
  const [userInfo, setUserInfo] = useState(null);
  const dispatch = useDispatch();
  const user = useSelector(selectUser);

  const handleClick = (event) => {
    console.log(event)
    setAnchorElement(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorElement(null);
  };

  const handleLogout = () => {
    dispatch(logout());
  };

  useEffect(() => {
    if (user != null) {
      const getUserRequestFactory = () => GetRequest(`${process.env.REACT_APP_SERVER_URL}api/Users/GetUserByID/${user.UserID}`, user.AccessToken);
      MakeAuthorizedRequest(getUserRequestFactory, dispatch, user.AccessToken, user.RefreshToken)
        .then(response => {
          if (response.status === 200) {
            console.log(response.data);
            setUserInfo(response.data);
          }
        });
    } else {
      setUserInfo(null);
    }
  }, [user?.UserID]);

  return userInfo ? (
      <div>
        <Button aria-haspopup="true" variant="contained" color="primary" onClick={handleClick} disableElevation>
          <Avatar src={userInfo?.AvatarPath} className={classes.avatar}/>
          <Typography className={classes.name}>{userInfo?.FirstName}</Typography>
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