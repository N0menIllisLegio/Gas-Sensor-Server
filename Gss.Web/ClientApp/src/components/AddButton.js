import { Button } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import AddIcon from '@material-ui/icons/Add';

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
    <Button variant="contained" color="secondary" className={classes.button} onClick={props.handleClick}>
      <AddIcon fontSize="large" />
    </Button>
  );
}