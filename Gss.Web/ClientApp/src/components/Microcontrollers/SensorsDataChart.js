import { Button, Chip, FormControl, InputLabel, MenuItem, Select, Typography } from '@material-ui/core';
import { AreaChart, Area, CartesianGrid, XAxis, YAxis, ResponsiveContainer, Tooltip } from 'recharts';
import { makeStyles } from '@material-ui/core/styles';
import { useEffect, useState } from 'react';
import DateFnsUtils from '@date-io/date-fns';
import { MuiPickersUtilsProvider, KeyboardDatePicker } from '@material-ui/pickers';
import { useSensorDataPost } from '../../hooks/usePost';
import Progress from '../Progress';

const useStyles = makeStyles((theme) => ({
  periodFormControl: {
    width: '200px',
  },
  controlsRow: {
    marginTop: theme.spacing(4),
    marginBottom: theme.spacing(4),
    display: 'flex',
    justifyContent: 'space-evenly'
  },
  chipRow: {
    display: 'flex',
    flexWrap: 'wrap',
    justifyContent: 'center'
  },
  chip: {
    margin: theme.spacing(1),
  },
  noDataCapation: {
    marginTop: theme.spacing(2),
    marginBottom: theme.spacing(2),
    display: 'flex',
    justifyContent: 'center'
  }
}));

const dateTimeOptions = {
  year: 'numeric',
  month: 'numeric',
  day: 'numeric'
};

export default function SensorsDataChart(props) {
  const classes = useStyles();
  const [ period, setPeriod ] = useState('Day');
  const [ watchingDate, setWatchingDate ] = useState(new Date());
  const [ watchingDates, setWatchingDates ] = useState([]);
  const [ colors, setColors ] = useState([]);
  const [ data, setData ] = useState([]);

  const handleDelete = (date) => {
    const index = watchingDates.indexOf(date);
    setWatchingDates(watchingDates.filter(wDate => wDate !== date));

    const color = colors[index];
    setColors(colors.filter(c => c !== color));
  };

  const handleAddWatchingDate = () => {
    if (watchingDate != null && watchingDates.length < 5 && !watchingDates.includes(watchingDate)) {
      setWatchingDates([...watchingDates, watchingDate]);
      setColors([...colors, getRandomColor()]);
    }
  };

  const { data: sensorData, isPending } = useSensorDataPost(`${process.env.REACT_APP_SERVER_URL}api/SensorsData/GetSensorData`, props.microcontrollerID, props.sensor.ID, period, watchingDates);

  useEffect(() => {
    if (sensorData != null) {
      const retriever = currentValueReadTimeRetrieverPicker(period);
      let processedSensorData = sensorData.reduce((acc, el) => groupByReadTime(acc, el, retriever), []);
      processedSensorData = processedSensorData.sort((d1, d2) => d1.RawValueReadTime - d2.RawValueReadTime);
      setData(processedSensorData);
    } else {
      setData(null);
    }
  }, [sensorData, period]);
  
  return (
    <div>
      <div className={classes.controlsRow}>
        <FormControl variant="outlined" className={classes.periodFormControl}>
          <InputLabel>Period</InputLabel>
          <Select
            value={period}
            onChange={(e) => setPeriod(e.target.value)}
            label="Period">
            <MenuItem value="Day">Day</MenuItem>
            <MenuItem value="Month">Month</MenuItem>
            <MenuItem value="Year">Year</MenuItem>
          </Select>
        </FormControl>

        <MuiPickersUtilsProvider utils={DateFnsUtils}>
          <KeyboardDatePicker
            value={watchingDate}
            onChange={(date) => setWatchingDate(date || null)}
            label="Watching Date"
            format="dd.MM.yyyy"
            inputVariant="outlined" />
        </MuiPickersUtilsProvider>

        <Button
          variant="outlined"
          onClick={handleAddWatchingDate}
          disabled={watchingDates.length >= 5}>
            Add Watching Date
        </Button>
      </div>
      {
        watchingDates && watchingDates.length > 0 && (
          <div className={classes.chipRow}>
            {
              watchingDates.map(date => (
                <Chip
                  className={classes.chip}
                  key={date}
                  label={date.toLocaleString("ru-RU", dateTimeOptions)}
                  onDelete={() => handleDelete(date)}
                  variant="outlined" />
              ))
            }
          </div>
        )
      }

      {
        (data && data.length > 0) || (
          <div>
            { (isPending && (<Progress />)) || (
            <Typography variant="caption" className={classes.noDataCapation}>
              No data to display...
            </Typography>) }
          </div>          
        )
      }

      {
        data && data.length > 0 && (
          <ResponsiveContainer width="99%" height={600}>
            <AreaChart data={data} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
              <defs>
                {
                  watchingDates.map(date => {
                    const color = colors[watchingDates.indexOf(date)];

                    return (
                      <linearGradient key={date} id={date.toLocaleString("ru-RU", dateTimeOptions)} x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor={color} stopOpacity={0.8}/>
                        <stop offset="95%" stopColor={color} stopOpacity={0}/>
                      </linearGradient>
                    )
                  })
                }
              </defs>
    
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="ValueReadTime" />
              <YAxis />
              <Tooltip isAnimationActive={false} />
              {
                watchingDates.map(date => {
                  const strDate = date.toLocaleString("ru-RU", dateTimeOptions);
                  const color = colors[watchingDates.indexOf(date)];

                  return (
                    <Area
                      key={date}
                      connectNulls
                      dataKey={strDate}

                      unit={props.sensor.SensorType.Units}
                      type="monotone"
                      stroke={color}
                      fillOpacity={1}
                      fill={`url(#${strDate})`} />
                  )
                })
              }
            </AreaChart>
          </ResponsiveContainer>
        )
      }
    </div>
  )
}

function groupByReadTime(accumulator, currentValue, currentValueReadTimeRetriever) {
  const currentValueReadTime = currentValueReadTimeRetriever(currentValue.ValueReadTime);
  let savedValue = accumulator.find((element, index, arr) => currentValueReadTime.value === element.ValueReadTime);
  const watchingDate = new Date(currentValue.WatchingDate).toLocaleString("ru-RU", dateTimeOptions);

  if (savedValue === undefined) {
    savedValue = {
      "ValueReadTime": currentValueReadTime.value,
      "RawValueReadTime": currentValueReadTime.rawValue,
    };

    savedValue[`${watchingDate}`] = currentValue.AverageSensorValue;

    return [...accumulator, savedValue];
  } else {
    savedValue[`${watchingDate}`] = currentValue.AverageSensorValue;

    return accumulator;
  }
}

function currentValueReadTimeRetrieverPicker(period) {
  switch (period) {
    case 'Day':
      return currentValueReadDay;
    case 'Month':
      return currentValueReadMonth;
    case 'Year':
      return currentValueReadYear;
    default:
        throw period;
  }
}

function currentValueReadDay(currentValueReadTime) {
  const rawValue = new Date(currentValueReadTime).getHours();
  return { rawValue: rawValue, value: rawValue };
}

function currentValueReadMonth(currentValueReadTime) {
  const rawValue = new Date(currentValueReadTime).getDate();
  return { rawValue: rawValue, value: rawValue };
}

function currentValueReadYear(currentValueReadTime) {
  const months = [
    'Jan', 'Feb', 'March', 'April', 'May',
    'June', 'July', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
  ];

  const rawValue = new Date(currentValueReadTime).getMonth();
  return { rawValue: rawValue, value: months[rawValue] };
}

function getRandomColor() {
  var letters = '0123456789ABCDEF';
  var color = '#';
  for (var i = 0; i < 6; i++) {
    color += letters[Math.floor(Math.random() * 16)];
  }
  return color;
}