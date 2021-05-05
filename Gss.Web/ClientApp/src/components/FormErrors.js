import { List, ListItem, ListItemText } from "@material-ui/core";
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles((theme) => ({
  list: {
    paddingTop: '0',
    paddingBottom: '0'
  },
  listItem: {
    color: theme.palette.secondary.main,
    padding: '0'
  }
}));

export default function FormErrors(props) {
  const classes = useStyles();

  return (
  <List className={classes.list}>
    {(props.errors.map(error => (
      <ListItem key={error} className={classes.listItem}>
        <ListItemText primary={error} />
      </ListItem>
    )))}
  </List>
  );
}