import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import TextField from '@material-ui/core/TextField';
import Typography from '@material-ui/core/Typography';
import { Button, Grid } from '@material-ui/core';
import { Link } from 'react-router-dom';
import { useState, useEffect } from 'react';


const emailRegex = /(?:^(?:.+)@(?:.+)\.(?:.+)$)|(?:^$)/;

const useStyles = makeStyles((theme) => ({
  card: {
    width: 500,
    position: 'fixed',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)'
  },
  bullet: {
    display: 'inline-block',
    marginLeft: theme.spacing(1),
    marginRight: theme.spacing(1),
  },
  cardHeader: {
    textAlign: 'center',
    marginTop: theme.spacing(2),
    marginBottom: theme.spacing(4)
  },
  formElement: {
    marginLeft: theme.spacing(2),
    marginRight: theme.spacing(2),
  },
  link: {
    textDecoration: 'none'
  }
}));

export default function Login() {
  const classes = useStyles();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [invalidEmail, setInvalidEmail] = useState(true);

  const handleFormSubmit = (e) => {
    e.preventDefault();

    // TODO send request, save response, show errors
    console.log({
      email: email,
      password: password
    });
  };

  useEffect(() => setInvalidEmail(email.match(emailRegex) === null), [email]);

  return (
    <Card className={classes.card}>
      <CardContent className={classes.cardContent}>
        <Typography gutterBottom variant="h4" component="h2" className={classes.cardHeader}>
          <strong>Log in</strong>
        </Typography>

        <form autoComplete="off" onSubmit={handleFormSubmit}>
          <Grid container spacing={3}>
            <Grid item xs={12} className={classes.formElement}>
              <TextField
                type="email"
                required
                fullWidth
                label="Email"
                variant="outlined"
                value={email}
                onChange={(e) => setEmail(e.target.value)} error={invalidEmail} />
            </Grid>
            <Grid item xs={12} className={classes.formElement}>
              <TextField
                type="password"
                required
                fullWidth
                label="Password"
                variant="outlined"
                value={password}
                onChange={(e) => setPassword(e.target.value)} />
            </Grid>
            <Grid item xs={12} className={classes.formElement}>
              <Button
                type="submit"
                variant="contained"
                color="primary"
                size="medium"
                style={{width: '100%'}}>
                  Log in
              </Button>
            </Grid>
            <Grid item xs={12}>
              <Typography style={{textAlign: 'center'}}>
                <Link to="/" className={classes.link}>Forgot password?</Link>
                <span className={classes.bullet}>•</span>
                <Link to="/" className={classes.link}>Sign up</Link>
              </Typography>
            </Grid>
          </Grid>
        </form>
      </CardContent>
    </Card>
  );
}