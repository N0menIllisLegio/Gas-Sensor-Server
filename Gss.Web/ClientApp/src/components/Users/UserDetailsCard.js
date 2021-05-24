import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import EditTwoToneIcon from '@material-ui/icons/EditTwoTone';
import CardContent from '@material-ui/core/CardContent';
import CardMedia from '@material-ui/core/CardMedia';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import UserDetailsList from './UserDetailsList';
import { Divider } from '@material-ui/core';
import Grid from '@material-ui/core/Grid';

const useStyles = makeStyles((theme) => ({
  root: {
    width: theme.userDetailsCard.width,
  },
  media: {
    height: 300,
  },
  namesHeader: {
    marginLeft: theme.spacing(2),
    display: 'flex',
    alignItems: 'flex-end',
    justifyContent: 'space-between'

  },
  divider: {
    marginBottom: theme.spacing(2)
  },
  cardCaptionParagraph: {
    margin: 0,
    textAlign: 'end'
  }
}));

const dateTimeOptions = {
  year: 'numeric',
  month: 'long',
  day: 'numeric',
  weekday: 'long',
  hour: 'numeric',
  minute: 'numeric',
  second: 'numeric',
  hour12: false
};

export default function UserDetailsCard(props) {
  const classes = useStyles();
  const user = props.user;

  return user && (
    <div>
      <Card className={classes.root}>
        <CardMedia
          className={classes.media}
          image={user.AvatarPath != null ? user.AvatarPath : process.env.REACT_APP_AVATAR_PLACEHOLDER_URL}
          title="Contemplative Reptile" />
        <CardContent style={{padding: '16px'}}>
          <div className={classes.namesHeader}>
            <Typography gutterBottom variant="h5" component="h2">
              <strong>
                {user.FirstName} {user.LastName}
              </strong>
            </Typography>
            <IconButton size="medium" onClick={props.handleEditClick} color="primary">
              <EditTwoToneIcon />
            </IconButton>
          </div>

          <UserDetailsList user={user} />

          <Divider light className={classes.divider} />

          <Typography variant="caption">
            {user.CreationDate != null ? (
              <Grid container spacing={0}>
                <Grid item xs={4}>
                  ID:
                </Grid>
                <Grid item xs={8}>
                  <p className={classes.cardCaptionParagraph}>
                    <i>{user.ID}</i>
                  </p>
                </Grid>
                <Grid item xs={4}>
                  Creation Date:
                </Grid>
                <Grid item xs={8}>
                  <p className={classes.cardCaptionParagraph}>
                    <i>{new Date(user.CreationDate).toLocaleString("en-US", dateTimeOptions)}</i>
                  </p>
                </Grid>
              </Grid>
            ) : (
              <Grid container spacing={0}>
                <Grid item xs={4}>
                  ID:
                </Grid>
                <Grid item xs={8}>
                  <p className={classes.cardCaptionParagraph}>
                    <i>{user.ID}</i>
                  </p>
                </Grid>
              </Grid>
            )}
          </Typography>
        </CardContent>
      </Card>
    </div>
    );
}