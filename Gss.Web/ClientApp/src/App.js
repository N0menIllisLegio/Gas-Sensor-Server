import Dashboard from './components/Administration/Dashboard';
import Navbar from './components/Navbar';
import { unstable_createMuiStrictModeTheme as createMuiTheme, ThemeProvider, Toolbar } from '@material-ui/core'
import { red, teal } from '@material-ui/core/colors'
import ClippedDrawer from './components/ClippedDrawer';
import CssBaseline from '@material-ui/core/CssBaseline';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import { makeStyles } from '@material-ui/core/styles';
import Users from './components/Administration/Users';
import User from './components/Users/User';
import Microcontrollers from "./components/Administration/Microcontrollers";
import Sensors from './components/Administration/Sensors';
import SensorTypes from './components/Administration/SensorTypes';
import Login from './components/Login';
import { useEffect } from 'react';
import { useDispatch } from 'react-redux'
import { initialize } from './redux/reducers/authSlice';
import SignUp from './components/SignUp';
import { Initialize } from './requests/Requests';
import Microcontroller from './components/Microcontrollers/Microcontroller';

export const theme = createMuiTheme({
  palette: {
    primary: teal,
    secondary: red
  },

  userDetailsCard: {
    width: 500
  }
});

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexGrow: 1,
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
  },
}));

export default function App() {
  const classes = useStyles();
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(initialize());
    Initialize(dispatch);
  }, [dispatch]);

  return (
    <ThemeProvider theme={theme}>
      <Router>
        <div className={classes.root}>
          <CssBaseline />
          <Navbar />
          <ClippedDrawer />
          <main className={classes.content}>
            <Toolbar/>
              <Switch>
                <Route exact path="/">
                  <Dashboard />
                </Route>

                {/* Auth */}
                <Route path="/login">
                  <Login />
                </Route>
                <Route path="/signup">
                  <SignUp />
                </Route>

                {/* Administration catalogs */}
                <Route path="/users">
                  <Users />
                </Route>
                <Route path="/microcontrollers">
                  <Microcontrollers />
                </Route>
                <Route path="/sensors">
                  <Sensors />
                </Route>
                <Route path="/sensorTypes">
                  <SensorTypes />
                </Route>

                {/* Details */}
                <Route path="/user/:id">
                  <User />
                </Route>
                <Route path="/microcontroller/:id">
                  <Microcontroller />
                </Route>
              </Switch>
          </main>
        </div>
      </Router>
    </ThemeProvider>
  );
}
