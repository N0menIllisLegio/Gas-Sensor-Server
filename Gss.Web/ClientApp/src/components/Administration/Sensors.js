import PagedTable from "../PagedTable";
import { Link } from 'react-router-dom'
import { Button } from "@material-ui/core";

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
    renderCell: (params) => params.value === null ? (<div>—</div>) : (
    <Link to={`/sensorType/${params.value.ID}`} style={{color: 'inherit', textDecoration: 'none'}}>
      <Button variant="contained" color="primary" disableElevation>
      {params.value.Name}
      </Button>
    </Link>
  )},
]

export default function Sensors() {
  return (
    <div>
      <PagedTable columns={columns} url={'api/Sensors/GetAllSensors'} detailsUrl={'/sensor/'} />
    </div>
  );
}