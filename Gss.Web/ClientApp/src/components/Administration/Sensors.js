import PagedTable from "../PagedTable";
import AddButton from "../AddButton";
import Sensor from "./Sensor";
import { useState, useEffect } from 'react';

const columns = [
  { field: 'ID', headerName: 'ID', flex: 1 },
  { field: 'Name', headerName: 'Name', flex: 1 },
  { field: 'SensorType', headerName: 'Sensor Type', flex: 1, align: 'center', headerAlign: 'center',
    sortComparator: (v1, v2, cellParams1, cellParams2) => {
      if (v1 !== null && v2 !== null) {
        return v1.Name.localeCompare(v2.Name);
      } else if (v1 === v2) {
        return 0;
      } else if (v1 === null) {
        return -1;
      } else {
        return 1;
      }
    },
    renderCell: (params) => params.value === null ? (<div>—</div>) : (<div>{params.value.Name}</div>)
  },
]

export default function Sensors() {
  const [openDialog, setOpenDialog] = useState(false);
  const [selectedSensor, setSelectedSensor] = useState(null);
  const [sensorChanged, setSensorChanged] = useState(false);
  const [sensorUrl, setSensorUrl] = useState('api/Sensors/GetAllSensors');

  const handleDetailsAction = (e) => {
    if (e?.row != null) {
      setSelectedSensor(e.row);
      setOpenDialog(true);
    }
  };

  useEffect(() => {
    if (sensorChanged) {
      setSensorUrl('api/Sensors/GetAllSensors/');
    } else {
      setSensorUrl('api/Sensors/GetAllSensors');
    }
  }, [sensorChanged]);

  return (
    <div>
      <PagedTable columns={columns} url={sensorUrl} detailsAction={handleDetailsAction} />
      <AddButton handleClick={() => setOpenDialog(true)} />
      <Sensor
        openDialog={openDialog}
        setOpenDialog={setOpenDialog}
        selectedSensor={selectedSensor}
        setSelectedSensor={setSelectedSensor}
        sensorChanged={sensorChanged}
        setSensorChanged={setSensorChanged} />
    </div>
  );
}