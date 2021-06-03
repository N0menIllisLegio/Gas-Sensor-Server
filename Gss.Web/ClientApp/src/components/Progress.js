import { makeStyles } from '@material-ui/core/styles';
import { CircularProgress } from "@material-ui/core";

const useStyles = makeStyles((theme) => ({
  progressContainer: {
    marginTop: theme.spacing(4),
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center'
  }
}));

export default function Progress() {
  const classes = useStyles();

  return (
    <div className={classes.progressContainer}>
      <CircularProgress color="secondary" />
    </div>
  );
}