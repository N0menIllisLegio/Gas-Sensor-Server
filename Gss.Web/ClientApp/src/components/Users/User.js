import { makeStyles } from '@material-ui/core/styles';
import useGet from '../../hooks/useGet';
import { useHistory, useParams } from 'react-router-dom';
import UserDetailsCard from './UserDetailsCard';
import UserMicrocontrollersList from './UserMicrocontrollersList';
import { useState, useEffect } from 'react';
import UserEdit from './UserEdit';
import AddButton from '../AddButton';
import { useSelector } from 'react-redux';
import { selectUser } from '../../redux/reducers/authSlice';

const extendedUserUrl = `${process.env.REACT_APP_SERVER_URL}api/Users/GetExtendedUserByID/`;
const userUrl = `${process.env.REACT_APP_SERVER_URL}api/Users/GetUserByID/`;

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex'
  },
  rightColumn: {
    marginLeft: theme.spacing(3),
    overflow: 'scroll',
    height: theme.mainContent.height,
    width: '100%',
    overflowX: 'hidden',
    marginRight: -theme.spacing(theme.mainContent.padding)
  }
}));

export default function User() {
  const classes = useStyles();
  const history = useHistory();
  const { id } = useParams();
  const authorizedUser = useSelector(selectUser);
  const hasExtendedRights = authorizedUser?.Administrator === true || authorizedUser?.UserID === id;
  const usingUrl = hasExtendedRights ? extendedUserUrl : userUrl;

  const [ isEditingUserInfo, setIsEditingUserInfo ] = useState(false);
  const [userDetailsChanged, setUserDetailsChanged] = useState(false);
  const [userDetailsUrl, setUserDetailsUrl] = useState(usingUrl + id);
  const { data: user, isPending: userDetailsIsPending } = useGet(userDetailsUrl);

  useEffect(() => {
    if (userDetailsChanged) {
      setUserDetailsUrl(usingUrl + id + '/');
    } else {
      setUserDetailsUrl(usingUrl + id);
    }
  }, [userDetailsChanged, id]);

  return (
    <div className={classes.root}>
      <div>
        { !userDetailsIsPending && !isEditingUserInfo && <UserDetailsCard user={user} handleEditClick={() => setIsEditingUserInfo(true)} />}
        { !userDetailsIsPending && isEditingUserInfo
          && <UserEdit
            user={user}
            handleCloseClick={() => setIsEditingUserInfo(false)}
            setUserDetailsChanged={setUserDetailsChanged}
            userDetailsChanged={userDetailsChanged}/>}
      </div>
      <div className={classes.rightColumn}>
        <UserMicrocontrollersList userID={id} />
      </div>
      <AddButton handleClick={() => history.push('/edit/microcontroller')}/>
    </div>
  );
}