import { TextField, List, ListItem, ListItemAvatar, Avatar, ListItemText, Card, Typography, Grid, Button } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import { Pagination } from '@material-ui/lab';
import { useEffect, useState } from 'react';
import { usePagedPost } from '../../hooks/usePost';
import Progress from '../Progress';

const useStyles = makeStyles((theme) => ({
  sensorsList: {
    width: '100%',
    backgroundColor: theme.palette.background.paper,
    height: theme.spacing(50),
    overflow: 'auto'
  },
  inputsRow: {
    marginBottom: theme.spacing(2),
    display: 'flex',
    justifyContent: 'space-between', 
    alignItems: 'center'
  },
  formControl: {
    margin: theme.spacing(1),
    minWidth: 120,
  },
  description: {
    marginLeft: theme.spacing(2)
  },
  button: {
    marginTop: theme.spacing(2)
  }
}));

export default function SensorsPagedList(props) {
  const classes = useStyles();
  const [searchString, setSearchString] = useState('');
  const [totalPagesNumber, setTotalPagesNumber] = useState(4);
  const [currentPageNumber, setCurrentPageNumber] = useState(1);
  const [selectedSensor, setSelectedSensor] = useState(null);
  const pageSize = 10;

  const { data: pagedResponse, isPending } = usePagedPost(
    `${process.env.REACT_APP_SERVER_URL}api/Sensors/GetAllSensors`, currentPageNumber, pageSize, searchString);

  useEffect(() => {
    if (pagedResponse != null) {
      setTotalPagesNumber(Math.ceil(pagedResponse.TotalItemsCount / pagedResponse.PagedInfo.PageSize));
    }
  }, [pagedResponse]);

  useEffect(() => {
    setCurrentPageNumber(1);
  }, [searchString]);

  return (
    <div>
      { isPending && (<Progress />) }
      { pagedResponse && (
        <div>
          <div className={classes.inputsRow}>
            <TextField label="Search..." value={searchString} onChange={(e) => setSearchString(e.target.value)}/>

            {totalPagesNumber > 1 && (
                <Pagination
                  variant="outlined"
                  color="primary"
                  count={totalPagesNumber}
                  page={currentPageNumber}
                  onChange={(e, pageNumber) => setCurrentPageNumber(pageNumber)} />
              )
            }
          </div>

          <Grid container>
            <Grid item xs={6}>
              <Card variant="outlined" style={{width:'100%'}}>
                <List className={classes.sensorsList} dense>
                  { pagedResponse.Items.map(sensor => (
                    <ListItem
                      key={sensor.ID}
                      button
                      selected={selectedSensor?.ID === sensor.ID}
                      onClick={() => setSelectedSensor(sensor) }>
                      <ListItemAvatar>
                        <Avatar src={sensor.SensorType.Icon}>
                          {sensor.SensorType.Units}
                        </Avatar>
                      </ListItemAvatar>
                      <ListItemText primary={sensor.Name} secondary={sensor.SensorType.Name} />
                    </ListItem>
                  ))}
                </List>
              </Card>

              <Button
                variant="outlined"
                color="primary"
                disableElevation
                size="large"
                disabled={!props.addSensorButtonEnabled || !selectedSensor}
                className={classes.button}
                onClick={() => props.handleSensorClick(selectedSensor)}
                style={{width: '100%'}}>Add Sensor</Button>
            </Grid>

            <Grid item xs={6}>
              <div className={classes.description}>
                <Typography variant="caption" align="justify" display="block" style={{whiteSpace: 'pre-line'}}>
                  {selectedSensor?.Description}
                </Typography>
              </div>
            </Grid>
          </Grid>
        </div>
      )}
    </div>
  )
}