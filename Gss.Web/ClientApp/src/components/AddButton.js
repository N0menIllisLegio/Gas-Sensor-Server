import { Button } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import AddIcon from '@material-ui/icons/Add';
import { Link } from 'react-router-dom';

const margin = 6;

const useStyles = makeStyles((theme) => ({
  button: {
    position: 'fixed',
    bottom: theme.spacing(margin),
    right: theme.spacing(margin),
    borderRadius: '100%',
    padding: '15px'
  }
}));

export default function AddButton(props) {
  const classes = useStyles();

  return (
    <Link to={props.url}>
      <Button variant="contained" color="secondary" className={classes.button}>
        <AddIcon fontSize="large" />
      </Button>
    </Link>
  );
}