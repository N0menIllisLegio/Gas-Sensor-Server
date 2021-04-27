import { makeStyles } from '@material-ui/core/styles';
import useGet from '../../hooks/useGet';
import { useParams } from 'react-router-dom';
import UserDetailsCard from './UserDetailsCard';
import { Grid, Typography } from '@material-ui/core';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex'
  },
  rightColumn: {
    marginLeft: theme.spacing(3)
  },
  userDetailsCard: {
    position: 'fixed'
  }
}));

export default function User() {
  const classes = useStyles();
  const { id } = useParams();
  const { data: user, isPending: userDetailsIsPending } = useGet(`${process.env.REACT_APP_SERVER_URL}api/Users/GetExtendedUserByID/${id}`);

  return (
    <div className={classes.root}>
      { !userDetailsIsPending && <UserDetailsCard user={user} className={classes.userDetailsCard} />}
      <div className={classes.rightColumn}>
        Lorem ipsum dolor sit amet consectetur adipisicing elit. Accusamus minus pariatur eligendi quis, commodi nemo eos magni.
        Consequuntur dolorum, est praesentium, delectus deserunt porro iusto harum magni saepe assumenda accusamus.
        Lorem ipsum dolor sit, amet consectetur adipisicing elit. Reprehenderit illum vitae voluptates sint iusto et commodi, eos rerum, inventore est sunt autem, enim nam odio debitis eveniet minus voluptatem aut!
        Lorem ipsum dolor sit amet consectetur, adipisicing elit. Laboriosam, incidunt numquam. Consectetur deserunt voluptates eius rem nisi error beatae magnam, omnis voluptatum nulla ab corrupti voluptatibus. Maiores, et laborum! Eveniet!
        Lorem ipsum dolor sit amet consectetur adipisicing elit. Ratione ullam quae, maxime reiciendis, quis amet deserunt optio earum sed impedit a soluta sit dolorem totam temporibus. Iure expedita voluptatibus explicabo!
      </div>
    </div>
  );
}