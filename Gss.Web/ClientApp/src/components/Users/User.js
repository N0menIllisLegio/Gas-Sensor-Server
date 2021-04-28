import { makeStyles } from '@material-ui/core/styles';
import useGet from '../../hooks/useGet';
import { useParams } from 'react-router-dom';
import UserDetailsCard from './UserDetailsCard';
import UserMicrocontrollersList from './UserMicrocontrollersList';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex'
  },
  rightColumn: {
    marginLeft: theme.spacing(3) + theme.userDetailsCard.width
  },
  userDetailsCardWrapper: {
    position: 'fixed'
  }
}));

export default function User() {
  const classes = useStyles();
  const { id } = useParams();
  const { data: user, isPending: userDetailsIsPending } = useGet(`${process.env.REACT_APP_SERVER_URL}api/Users/GetExtendedUserByID/${id}`);

  return (
    <div className={classes.root}>
      <div className={classes.userDetailsCardWrapper} >
        { !userDetailsIsPending && <UserDetailsCard user={user} />}
      </div>
      <div className={classes.rightColumn}>
        <UserMicrocontrollersList />
      </div>
    </div>
  );
}