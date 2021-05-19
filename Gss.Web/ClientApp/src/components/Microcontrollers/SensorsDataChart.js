import { Button, Chip, FormControl, Grid, InputLabel, MenuItem, Select } from '@material-ui/core';
import { AreaChart, Area, CartesianGrid, XAxis, YAxis, ResponsiveContainer, Legend, Tooltip } from 'recharts';
import { makeStyles } from '@material-ui/core/styles';
import { useEffect, useState } from 'react';
import DateFnsUtils from '@date-io/date-fns';
import { MuiPickersUtilsProvider, KeyboardDatePicker } from '@material-ui/pickers';

const data = [
  { name: 'Jan' },
  { name: 'Feb' },
  { name: 'March' },
  { name: 'April' },
  { name: 'May' },
  { name: 'June' },
  { name: 'July' },
  { name: 'Aug' },
  { name: 'Sep' },
  { name: 'Oct' },
  { name: 'Nov' },
  { name: 'Dec' },
];

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
  }
}));

export default function SensorsDataChart() {
  const classes = useStyles();
  const handleDelete = () => { };

  data.forEach(element => element.value = Math.floor(Math.random() * 1024));

  return (
    <div>
      <div className={classes.controlsRow}>
        <FormControl variant="outlined" className={classes.periodFormControl}>
          <InputLabel>Period</InputLabel>
          <Select
            label="Period"
            defaultValue="Day">
            <MenuItem value="Day">Day</MenuItem>
            <MenuItem value="Month">Month</MenuItem>
            <MenuItem value="Year">Year</MenuItem>
          </Select>
        </FormControl>

        <MuiPickersUtilsProvider utils={DateFnsUtils}>
          <KeyboardDatePicker
            clearable
            label="Watching Date"
            format="dd.MM.yyyy"
            inputVariant="outlined" />
        </MuiPickersUtilsProvider>

        <Button variant="outlined">Add Watching Date</Button>
      </div>
      <div className={classes.chipRow}>
        <Chip className={classes.chip} label="20.05.2021" onDelete={handleDelete} variant="outlined" />
      </div>

      <ResponsiveContainer width="100%" height={600}>
        <AreaChart data={data} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
          <defs>
            <linearGradient id="colorUv" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#8884d8" stopOpacity={0.8}/>
              <stop offset="95%" stopColor="#8884d8" stopOpacity={0}/>
            </linearGradient>
            <linearGradient id="colorPv" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#82ca9d" stopOpacity={0.8}/>
              <stop offset="95%" stopColor="#82ca9d" stopOpacity={0}/>
            </linearGradient>
            <linearGradient id="colorAmt" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#82ce9d" stopOpacity={0.8}/>
              <stop offset="95%" stopColor="#82ce9d" stopOpacity={0}/>
            </linearGradient>
          </defs>

          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="name" />
          <YAxis />
          <Tooltip isAnimationActive={false} formatter={(value, name, props) => `${value} ${'ppm'}`} />
          <Legend verticalAlign="top" height={36}/>
          <Area type="monotone" dataKey="value" stroke="#8884d8" fillOpacity={1} fill="url(#colorUv)" />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  )
}