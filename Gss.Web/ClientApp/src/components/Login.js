import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import TextField from '@material-ui/core/TextField';
import Typography from '@material-ui/core/Typography';
import { Button, Grid } from '@material-ui/core';
import { Link } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { PostRequest } from '../requests/Post';
import { useForm } from "react-hook-form";
import FormErrors from './FormErrors';
import CircularProgress from '@material-ui/core/CircularProgress';
import { useHistory } from "react-router-dom";
import { useDispatch } from 'react-redux';
import { authorize } from '../redux/reducers/authSlice';

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
  },
  footer: {
    display: 'flex',
    justifyContent: 'center'
  },
  progress: {
    color: theme.palette.grey[400],
    marginRight: theme.spacing(1)
  },
  loginButton: {
    width: '100%',
    display: 'flex',
    alignItems: 'center'
  }
}));

export default function Login() {
  const classes = useStyles();
  const history = useHistory();
  const dispatch = useDispatch();
  const { register, formState: { errors }, handleSubmit } = useForm();
  const [serverErrors, setServerErrors] = useState(null);
  const [isPending, setIsPending] = useState(false);
  const [isPasswordError, setIsPasswordError] = useState(false);
  const [isEmailError, setIsEmailError] = useState(false);

  const handleFormSubmit = (authRequestBody) => {
    setIsPending(true);

    PostRequest(`${process.env.REACT_APP_SERVER_URL}api/Authorization/LogIn`, authRequestBody)
      .then(response => {
        setServerErrors(response.errors);
        setIsPending(false);

        if (response.data != null) {
          dispatch(authorize(response.data))
          if (history.length > 1) {
            history.goBack();
          } else {
            history.push('/');
          }
        }
      });
  };

  useEffect(() => {    
    setIsEmailError(errors?.Login?.message != null);
    setIsPasswordError(errors?.Password?.message != null);
  }, [errors, errors?.Login, errors?.Password]);

  return (
    <Card className={classes.card}>
      <CardContent className={classes.cardContent}>
        <Typography gutterBottom variant="h4" component="h2" className={classes.cardHeader}>
          <strong>Log in</strong>
        </Typography>

        <form autoComplete="off" onSubmit={handleSubmit(handleFormSubmit)}>
          <Grid container spacing={3}>
            <Grid item xs={12} className={classes.formElement}>

              <TextField
                {...register('Login', {
                  required: { value: true, message: "Email is required" },
                  minLength: { value: 4, message: "Minimum Email length is 4 characters" },
                  pattern: { value: /(?:^(?:.+)@(?:.+)\.(?:.+)$)|(?:^$)/, message: "Email is invalid" }
                })}
                fullWidth
                label="Email"
                variant="outlined"
                error={isEmailError}
                helperText={errors.Login?.message}  />
            </Grid>

            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('Password', {
                  required: { value: true, message: "Password is required" },
                  maxLength: { value: 20, message: "Maximum Password length is 20 characters" },
                  minLength: { value: 4, message: "Minimum Password length is 4 characters" }
                })}
                type="password"
                fullWidth
                label="Password"
                variant="outlined"
                error={isPasswordError}
                helperText={errors.Password?.message} />
            </Grid>

            {serverErrors && (
              <Grid item xs={12} className={classes.formElement}>
                <FormErrors errors={serverErrors} />
              </Grid>
            )}

            <Grid item xs={12} className={classes.formElement}>
              <Button
                type="submit"
                variant="contained"
                color="primary"
                size="medium"
                className={classes.loginButton}>
                  {isPending && (<CircularProgress size={18} className={classes.progress} />)} Log in
              </Button>
            </Grid>

            <Grid item xs={12}>
              <Typography className={classes.footer}>
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