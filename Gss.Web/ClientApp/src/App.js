import Dashboard from './components/Administration/Dashboard';
import Navbar from './components/Navbar';
import { unstable_createMuiStrictModeTheme as createMuiTheme, ThemeProvider, Toolbar } from '@material-ui/core'
import { red, teal } from '@material-ui/core/colors'
import ClippedDrawer from './components/ClippedDrawer';
import CssBaseline from '@material-ui/core/CssBaseline';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import { makeStyles } from '@material-ui/core/styles';
import Users from './components/Administration/Users';
import User from './components/User';
import Microcontrollers from "./components/Microcontrollers";
import Sensors from './components/Administration/Sensors';

const theme = createMuiTheme({
  palette: {
    primary: teal,
    secondary: red
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
                <Route path="/users">
                  <Users />
                </Route>
                <Route path="/user/:id">
                  <User />
                </Route>
                <Route path="/microcontrollers">
                  <Microcontrollers />
                </Route>
                <Route path="/sensors">
                  <Sensors />
                </Route>
              </Switch>
          </main>
        </div>
      </Router>
    </ThemeProvider>
  );
}
