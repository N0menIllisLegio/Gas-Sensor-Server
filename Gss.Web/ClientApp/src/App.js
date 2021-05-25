import Navbar from './components/Navbar';
import { unstable_createMuiStrictModeTheme as createMuiTheme, ThemeProvider } from '@material-ui/core'
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
import ConfigurationFileGenerator from './components/Microcontrollers/ConfigurationFileGenerator';
import EditMicrocontroller from './components/Microcontrollers/EditMicrocontroller';
import MicrocontrollersMap from './components/MicrocontrollersMap';
import MicrocontrollersPublicTable from './components/MicrocontrollersPublicTable';

const mainContentPadding = 3;
const mainContentMarginTop = 8;

export const theme = createMuiTheme({
  palette: {
    primary: teal,
    secondary: red
  },

  userDetailsCard: {
    width: 500
  },

  mainContent: {
    marginTop: mainContentMarginTop,
    padding: mainContentPadding    
  }
});

const useStyles = makeStyles((them) => ({
  root: {
    display: 'flex',
    flexGrow: 1,
  },
  content: {
    flexGrow: 1,
    marginTop: them.spacing(mainContentMarginTop),
    paddingTop: them.spacing(mainContentPadding),
    paddingRight: them.spacing(mainContentPadding),
    paddingLeft: them.spacing(mainContentPadding),
  },
}));

export default function App() {
  const classes = useStyles();
  const dispatch = useDispatch();

  theme.mainContent.height = `calc(100vh - (${theme.spacing(mainContentMarginTop)}px + ${theme.spacing(mainContentPadding)}px))`;

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
            <Switch>
              <Route exact path="/">
                <MicrocontrollersMap />
              </Route>

              <Route path="/public/microcontrollers">
                <MicrocontrollersPublicTable />
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
              <Route exact path="/user/:id">
                <User />
              </Route>
              <Route exact path="/microcontroller/:id">
                <Microcontroller />
              </Route>

              <Route exact path="/configFileGenerator/:microcontrollerID?">
                <ConfigurationFileGenerator />
              </Route>

              <Route exact path="/edit/microcontroller/:id?">
                <EditMicrocontroller />
              </Route>
            </Switch>
          </main>
        </div>
      </Router>
    </ThemeProvider>
  );
}
