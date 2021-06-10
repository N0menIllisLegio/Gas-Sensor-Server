import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import TextField from '@material-ui/core/TextField';
import Typography from '@material-ui/core/Typography';
import { Button, Grid } from '@material-ui/core';
import { useState, useEffect } from 'react';
import { PostRequest } from '../requests/Requests';
import { useForm } from "react-hook-form";
import FormErrors from './FormErrors';
import CircularProgress from '@material-ui/core/CircularProgress';
import { useHistory } from "react-router-dom";
import InputLabel from '@material-ui/core/InputLabel';
import MenuItem from '@material-ui/core/MenuItem';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import DateFnsUtils from '@date-io/date-fns';
import { MuiPickersUtilsProvider, KeyboardDatePicker } from '@material-ui/pickers';

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
  },
  formControl: {
    width: '100%',
  },
  centerGridItemContent: {
    marginLeft: theme.spacing(2),
    marginRight: theme.spacing(2),
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end'
  }
}));

export default function SignUp() {
  const classes = useStyles();
  const history = useHistory();
  const { register, formState: { errors }, handleSubmit, watch, getValues, setValue } = useForm();

  const [serverErrors, setServerErrors] = useState(null);
  const [isPending, setIsPending] = useState(false);
  
  const [birthday, setBirthday] = useState(null);
  const birthdayValue = getValues('Birthday');

  const [isEmailError, setIsEmailError] = useState(false);
  const [isPasswordError, setIsPasswordError] = useState(false);
  const [isFirstNameError, setIsFirstNameError] = useState(false);
  const [isLastNameError, setIsLastNameError] = useState(false);
  const [isBirthdayError, setIsBirthdayError] = useState(false);
  const [isPhoneNumberError, setIsPhoneNumberError] = useState(false);

  const handleFormSubmit = (signupRequestBody) => {
    setIsPending(true);

    signupRequestBody.LastName = signupRequestBody.LastName === '' ? null : signupRequestBody.LastName;
    signupRequestBody.Gender = signupRequestBody.Gender === '' ? null : signupRequestBody.Gender;
    signupRequestBody.PhoneNumber = signupRequestBody.PhoneNumber === '' ? null : signupRequestBody.PhoneNumber;

    PostRequest(`${process.env.REACT_APP_SERVER_URL}api/Authorization/Register`, signupRequestBody)
      .then(response => {
        setServerErrors(response.errors);
        setIsPending(false);

        if (response.status === 200) {
          history.push('/login');
        }
      });
  };

  useEffect(() => {    
    setIsEmailError(errors?.Email?.message != null);
    setIsPasswordError(errors?.Password?.message != null || errors?.ConfirmPassword?.message != null);
    setIsFirstNameError(errors?.FirstName?.message != null);
    setIsLastNameError(errors?.LastName?.message != null);
    setIsBirthdayError(errors?.Birthday?.message != null);
    setIsPhoneNumberError(errors?.PhoneNumber?.message != null);
  }, [
    errors,
    errors?.Email,
    errors?.Password,
    errors?.ConfirmPassword,
    errors?.FirstName,
    errors?.LastName,
    errors?.Birthday,
    errors?.PhoneNumber
  ]);

  useEffect(() => {
    register('Birthday', {
      validate: date => date == null || getAge(date) >= 15 || 'You must be older than 15'
    });
  }, [register]);
  
  useEffect(() => {
    setBirthday(birthdayValue || null);
  }, [setBirthday, birthdayValue]);

  return (
    <Card className={classes.card}>
      <CardContent className={classes.cardContent}>
        <Typography gutterBottom variant="h4" component="h2" className={classes.cardHeader}>
          <strong>Sign Up</strong>
        </Typography>

        <form autoComplete="off" onSubmit={handleSubmit(handleFormSubmit)}>
          <Grid container spacing={3}>
            {/* Email */}
            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('Email', {
                  required: { value: true, message: "Email is required" },
                  minLength: { value: 4, message: "Minimum Email length is 4 characters" },
                  pattern: { value: /(?:^(?:.+)@(?:.+)\.(?:.+)$)|(?:^$)/, message: "Email is invalid" }
                })}
                fullWidth
                label="Email"
                variant="outlined"
                error={isEmailError}
                helperText={errors.Email?.message} />
            </Grid>
            
            {/* Password */}
            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('Password', {
                  required: { value: true, message: "Password is required" },
                  maxLength: { value: 20, message: "Maximum Password length is 20 characters" },
                  minLength: { value: 4, message: "Minimum Password length is 4 characters" }
                })}
                name="Password"
                type="password"
                fullWidth
                label="Password"
                variant="outlined"
                error={isPasswordError}
                helperText={errors.Password?.message} />
            </Grid>

            {/* Confirm password */}
            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('ConfirmPassword', {
                  required: { value: true, message: "Password is required" },
                  maxLength: { value: 20, message: "Maximum Password length is 20 characters" },
                  minLength: { value: 4, message: "Minimum Password length is 4 characters" },
                  validate: value => value === watch('Password') || "The passwords do not match"
                })}
                type="password"
                fullWidth
                label="Confirm Password"
                variant="outlined"
                error={isPasswordError}
                helperText={errors.ConfirmPassword?.message} />
            </Grid>
              
            {/* First name */}
            <Grid item xs className={classes.formElement} style={{ marginRight: '0' }}>
              <TextField
                {...register('FirstName', {
                  required: { value: true, message: "First name is required" },
                  minLength: { value: 2, message: "Minimum first name length is 2 characters" },
                  maxLength: { value: 255, message: "Maximum first name length is 255 characters" },
                  pattern: { value: /(?:^[a-zA-Z]{2,255}$)|(?:^$)/, message: "First name should consist of english letters" }
                })}
                fullWidth
                label="First Name"
                variant="outlined"
                error={isFirstNameError}
                helperText={errors.FirstName?.message}  />
            </Grid>

            {/* Last Name */}
            <Grid item xs className={classes.formElement} style={{ marginLeft: '0' }}>
              <TextField
                {...register('LastName', {
                  minLength: { value: 2, message: "Minimum last name length is 2 characters" },
                  maxLength: { value: 255, message: "Maximum last name length is 255 characters" },
                  pattern: { value: /(?:^[a-zA-Z]{2,255}$)|(?:^$)/, message: "Last name should consist of english letters" }
                })}
                fullWidth
                label="Last Name"
                variant="outlined"
                error={isLastNameError}
                helperText={errors.LastName?.message} />
            </Grid>

            <Grid item xs={12} style={{ padding: '0' }}/>
            
            {/* Gender */}
            <Grid item xs className={classes.centerGridItemContent} style={{ marginRight: '0' }}>
              <FormControl variant="outlined" className={classes.formControl}>
                <InputLabel>Gender</InputLabel>
                <Select
                  {...register("Gender")}
                  label="Gender"
                  defaultValue="">
                  <MenuItem value="">
                    <em>None</em>
                  </MenuItem>
                  <MenuItem value="Male">Male</MenuItem>
                  <MenuItem value="Female">Female</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            
            {/* BirthDay */}
            <Grid item xs={6} className={classes.centerGridItemContent} style={{ marginLeft: '0' }}>
              <MuiPickersUtilsProvider utils={DateFnsUtils}>
                <KeyboardDatePicker
                  clearable
                  value={birthday}
                  onChange={(date) => setValue('Birthday', date, { shouldValidate: true, shouldDirty: true })}
                  label="Birthday"
                  format="dd.MM.yyyy"
                  inputVariant="outlined"
                  error={isBirthdayError}
                  helperText={errors.Birthday?.message} />
              </MuiPickersUtilsProvider>
            </Grid>
            
            {/* Phone number */}
            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('PhoneNumber', {
                  minLength: { value: 7, message: "Minimum phone number length is 7 characters" },
                  maxLength: { value: 12, message: "Maximum phone number length is 12 characters" },
                  pattern: { value: /(?:^\+?[0-9]{7,12}$)|(?:^$)/, message: "Phone number is invalid" }
                })}
                fullWidth
                label="Phone Number"
                variant="outlined"
                error={isPhoneNumberError}
                helperText={errors.PhoneNumber?.message} />
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
                {(isPending && (<CircularProgress className={classes.progress} size={15} />)) || (<span>Sign Up</span>)}
              </Button>
            </Grid>

            <Grid item xs={12}>
              {/* TODO External auth buttons google, twitter, facebook */}
            </Grid>
          </Grid>
        </form>
      </CardContent>
    </Card>
  );
}

function getAge(birthDate) {
  var today = new Date();
  var age = today.getFullYear() - birthDate.getFullYear();
  var monthDiff = today.getMonth() - birthDate.getMonth();

  return monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate()) ? age - 1 : age;
}