import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Button from '@material-ui/core/Button';
import Grid from '@material-ui/core/Grid';
import AvatarButton from '../AvatarButton';
import { CircularProgress, TextField } from '@material-ui/core';
import InputLabel from '@material-ui/core/InputLabel';
import MenuItem from '@material-ui/core/MenuItem';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import { MuiPickersUtilsProvider, KeyboardDatePicker } from '@material-ui/pickers';
import DateFnsUtils from '@date-io/date-fns';
import { theme as appTheme } from '../../App';
import { useForm } from "react-hook-form";
import { useEffect, useState } from 'react';
import FormErrors from '../FormErrors';
import { MakeAuthorizedRequest, PutRequest, PostImageRequest } from '../../requests/Requests';
import { useSelector, useDispatch } from 'react-redux';
import { selectUser, saveBadgeData } from '../../redux/reducers/authSlice';
import { useHistory } from 'react-router-dom';

const useStyles = makeStyles((theme) => ({
  root: {
    width: theme.userDetailsCard.width,
  },
  genderSelector: {
    width: '100%',
    marginBottom: theme.spacing(2),
    marginTop: theme.spacing(1),
  },
  centerGridItemContent: {
    marginLeft: theme.spacing(2),
    marginRight: theme.spacing(2),
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end'
  },
  formElement: {
    marginLeft: theme.spacing(2),
    marginRight: theme.spacing(2),
  },
  actionButtonsContainer: {
    display: 'flex',
    justifyContent: 'space-around'
  },
  progress: {
    color: theme.palette.grey[400],
    marginRight: theme.spacing(1)
  },
}));

export default function UserEdit(props) {
  const classes = useStyles();
  const history = useHistory();
  const dispatch = useDispatch();
  const user = useSelector(selectUser);
  const { register, formState: { errors }, handleSubmit, getValues, setValue } = useForm();

  const [serverErrors, setServerErrors] = useState(null);
  const [isPending, setIsPending] = useState(false);

  const [avatarSrc, setAvatarSrc] = useState(null);
  const [avatar, setAvatar] = useState(null);
  
  const [email, setEmail] = useState('');
  let emailValue = getValues('Email') ?? '';

  const [firstName, setFirstName] = useState('');
  let firstNameValue = getValues('FirstName') ?? '';

  const [lastName, setLastName] = useState('');
  let lastNameValue = getValues('LastName') ?? '';

  const [phoneNumber, setPhoneNumber] = useState('');
  let phoneNumberValue = getValues('PhoneNumber') ?? '';

  const [gender, setGender] = useState('');
  let genderValue = getValues('Gender') ?? '';

  const [birthday, setBirthday] = useState(null);
  let birthdayValue = getValues('Birthday');
  
  const [isFirstNameError, setIsFirstNameError] = useState(false);
  const [isLastNameError, setIsLastNameError] = useState(false);
  const [isBirthdayError, setIsBirthdayError] = useState(false);
  const [isPhoneNumberError, setIsPhoneNumberError] = useState(false);

  const handleFormSubmit = async (body) => {
    
    let uploadedAvatarUrl = null;

    if (avatar != null) {
      const saveImageRequestFactory = (token) =>
        PostImageRequest(`${process.env.REACT_APP_SERVER_URL}api/Files/AvatarUpload`, avatar, token);
  
      const imageResponse = await MakeAuthorizedRequest(saveImageRequestFactory, user);

      if (imageResponse.status === 200) {
        uploadedAvatarUrl = imageResponse.data.FileUrl;
      } else if (imageResponse.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      }
    }

    body.LastName = body.LastName || null;
    body.Gender = body.Gender || null;
    body.PhoneNumber = body.PhoneNumber || null;
    body.AvatarPath = uploadedAvatarUrl !== null
      ? uploadedAvatarUrl
      : avatarSrc || null;

    let updateRequestFactory = null;
    
    setIsPending(true);

    if (user.Administrator) {
      updateRequestFactory = (token) =>
        PutRequest(`${process.env.REACT_APP_SERVER_URL}api/Users/Update/${props.user.ID}`, body, token);
    } else {
      delete body.Email;
      updateRequestFactory = (token) =>
        PutRequest(`${process.env.REACT_APP_SERVER_URL}api/Users/UpdateUserInfo`, body, token);
    }

    const updateResponse = await MakeAuthorizedRequest(updateRequestFactory, user);
    
    setIsPending(false);

    if (updateResponse.status !== 200) {
      if (updateResponse.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (updateResponse.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setServerErrors(updateResponse.errors);
      }
    } else {
      
      if (!(user.Administrator && user.UserID !== props.user.ID)) {
        dispatch(saveBadgeData({
          userBadgeName: body.FirstName,
          userBadgeAvatarSrc: body.AvatarPath
        }));
      }

      props.setUserDetailsChanged(!props.userDetailsChanged);
      props.handleCloseClick();
    }
  };

  useEffect(() => {
    if (props.user != null) {
      setAvatarSrc(props.user.AvatarPath || null);
      
      setEmail(props.user.Email ?? '');
      setValue('Email', props.user.Email ?? '');

      setFirstName(props.user.FirstName ?? '');
      setValue('FirstName', props.user.FirstName ?? '');

      setLastName(props.user.LastName ?? '');
      setValue('LastName', props.user.LastName ?? '');

      setPhoneNumber(props.user.PhoneNumber ?? '');
      setValue('PhoneNumber', props.user.PhoneNumber ?? '');

      setGender(props.user.Gender ?? '');
      setValue('Gender', props.user.Gender ?? '');

      birthdayValue = new Date(props.user.Birthday);
      setValue('Birthday', birthdayValue);
    }
  }, [props.user]);
  
  useEffect(() => {
    setIsFirstNameError(errors?.FirstName?.message != null);
    setIsLastNameError(errors?.LastName?.message != null);
    setIsBirthdayError(errors?.Birthday?.message != null);
    setIsPhoneNumberError(errors?.PhoneNumber?.message != null);
  }, [
    errors,
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
    setEmail(emailValue);
  }, [setEmail, emailValue]);

  useEffect(() => {
    setFirstName(firstNameValue);
  }, [setFirstName, firstNameValue]);

  useEffect(() => {
    setLastName(lastNameValue);
  }, [setLastName, lastNameValue]);

  useEffect(() => {
    setPhoneNumber(phoneNumberValue);
  }, [setPhoneNumber, phoneNumberValue]);

  useEffect(() => {
    setGender(genderValue);
  }, [setGender, genderValue]);

  useEffect(() => {
    setBirthday(birthdayValue || null);
  }, [setBirthday, birthdayValue]);

  return (
    <form autoComplete="off" onSubmit={handleSubmit(handleFormSubmit)}>
      <Card className={classes.root}>
        <CardContent>
          <Grid container spacing={1}>
            
            <Grid item xs={12} className={classes.formElement}>
              <AvatarButton
                imageSrc={avatarSrc}
                setImageSrc={setAvatarSrc}
                setImage={setAvatar}
                avatarWidth={appTheme.spacing(36)}
                avatarHeight={appTheme.spacing(36)} />
            </Grid>

            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('Email', {
                  required: { value: true, message: "Email is required" },
                  minLength: { value: 4, message: "Minimum Email length is 4 characters" },
                  pattern: { value: /(?:^(?:.+)@(?:.+)\.(?:.+)$)|(?:^$)/, message: "Email is invalid" }
                })}
                value={email}
                onChange={(e) => setValue('Email', e.target.value, { shouldValidate: true, shouldDirty: true })}

                fullWidth
                disabled
                label="Email"
                margin="normal"
                variant="outlined" />
            </Grid>

            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('FirstName', {
                  required: { value: true, message: "First name is required" },
                  minLength: { value: 2, message: "Minimum first name length is 2 characters" },
                  maxLength: { value: 255, message: "Maximum first name length is 255 characters" },
                  pattern: { value: /(?:^[a-zA-Z]{2,255}$)|(?:^$)/, message: "First name should consist of english letters" }
                })}
                value={firstName}
                onChange={(e) => setValue('FirstName', e.target.value, { shouldValidate: true, shouldDirty: true })}

                fullWidth
                label="First Name"
                className={classes.textField}
                margin="normal"
                variant="outlined"
                error={isFirstNameError}
                helperText={errors.FirstName?.message}  />
            </Grid>

            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('LastName', {
                  minLength: { value: 2, message: "Minimum last name length is 2 characters" },
                  maxLength: { value: 255, message: "Maximum last name length is 255 characters" },
                  pattern: { value: /(?:^[a-zA-Z]{2,255}$)|(?:^$)/, message: "Last name should consist of english letters" }
                })}
                value={lastName}
                onChange={(e) => setValue('LastName', e.target.value, { shouldValidate: true, shouldDirty: true })}
                
                fullWidth
                label="Last Name"
                className={classes.textField}
                margin="normal"
                variant="outlined"
                error={isLastNameError}
                helperText={errors.LastName?.message} />
            </Grid>

            <Grid item xs={12} className={classes.formElement}>
              <FormControl variant="outlined" className={classes.genderSelector}>
                <InputLabel>Gender</InputLabel>
                <Select
                  {...register('Gender')}
                  value={gender}
                  onChange={(e) => setValue('Gender', e.target.value, { shouldValidate: true, shouldDirty: true })}
                  label="Gender">
                  <MenuItem value=""><em>None</em></MenuItem>
                  <MenuItem value="Male">Male</MenuItem>
                  <MenuItem value="Female">Female</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} className={classes.formElement}>
              <MuiPickersUtilsProvider utils={DateFnsUtils}>
                <KeyboardDatePicker
                  fullWidth
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

            <Grid item xs={12} className={classes.formElement}>
              <TextField
                {...register('PhoneNumber', {
                  minLength: { value: 7, message: "Minimum phone number length is 7 characters" },
                  maxLength: { value: 12, message: "Maximum phone number length is 12 characters" },
                  pattern: { value: /(?:^\+?[0-9]{7,12}$)|(?:^$)/, message: "Phone number is invalid" }
                })}
                value={phoneNumber}
                onChange={(e) => setValue('PhoneNumber', e.target.value, { shouldValidate: true, shouldDirty: true })}

                fullWidth
                label="Phone Number"
                className={classes.textField}
                margin="normal"
                variant="outlined"
                error={isPhoneNumberError}
                helperText={errors.PhoneNumber?.message} />
            </Grid>
            
            {serverErrors && (
              <Grid item xs={12} className={classes.formElement}>
                <FormErrors errors={serverErrors} />
              </Grid>
            )}

          </Grid>
        </CardContent>
        <CardActions className={classes.actionButtonsContainer}>
          <Button size="small" color="secondary" type="button" onClick={props.handleCloseClick}>Cancel</Button>
          <Button size="small" color="primary" type="submit">
            {isPending && (<CircularProgress className={classes.progress} size={15} />)}
            Save
          </Button>
        </CardActions>
      </Card>
    </form>
  );
}

function getAge(birthDate) {
  var today = new Date();
  var age = today.getFullYear() - birthDate.getFullYear();
  var monthDiff = today.getMonth() - birthDate.getMonth();

  return monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate()) ? age - 1 : age;
}