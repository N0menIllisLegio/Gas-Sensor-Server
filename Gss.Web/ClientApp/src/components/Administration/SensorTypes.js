import PagedTable from "../PagedTable";
import { Avatar } from "@material-ui/core";
import { Button } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import AddIcon from '@material-ui/icons/Add';
import CreateSensorType from "../SensorTypes/CreateSensorType";
import { useState } from 'react';

const margin = 6;

const useStyles = makeStyles((theme) => ({
  button: {
    position: 'fixed',
    bottom: theme.spacing(margin),
    right: theme.spacing(margin),
    borderRadius: '100%',
    padding: '15px'
  }
}));

// https://sensorsappimagestorage.blob.core.windows.net/thumbnails/0c896b43-2794-4d3e-a8eb-39dbd3afc3e6.png
const columns = [
  {field: 'Icon', headerName: 'Icon', flex: 0.1, align: 'center', headerAlign: 'center',
    renderCell: (params) => (
      <Avatar src={params.value} variant="square">{params.row.Units}</Avatar>
  )},
  { field: 'ID', headerName: 'ID', flex: 0.5 },
  { field: 'Units', headerName: 'Units', flex: 0.2, align: 'center', headerAlign: 'center' },
  { field: 'Name', headerName: 'Name', flex: 1 },
];

// detailsUrl={'/sensorsType/'}
// url={'/sensorsType/create'} 

export default function Sensors() {
  const classes = useStyles();
  const [openDialog, setOpenDialog] = useState(false);

  const handleClickOpen = () => {
    setOpenDialog(true);
  };

  return (
    <div>
      <PagedTable columns={columns} url={'api/SensorsTypes/GetAllSensorsTypes'} />
      <Button variant="contained" color="secondary" className={classes.button} onClick={handleClickOpen}>
        <AddIcon fontSize="large" />
      </Button>
      
      <CreateSensorType openDialog={openDialog} setOpenDialog={setOpenDialog} />
    </div>
  );
}