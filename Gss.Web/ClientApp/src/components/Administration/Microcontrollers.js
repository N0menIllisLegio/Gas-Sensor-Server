import PagedTable from "../PagedTable";
import { Link } from 'react-router-dom'
import { Button } from "@material-ui/core";
import AddButton from "../AddButton";
import { useHistory } from 'react-router-dom';

const columns = [
  { field: 'ID', headerName: 'ID', flex: 1 },
  { field: 'Name', headerName: 'Name', flex: 0.8 },
  { field: 'IPAddress', headerName: 'IP', flex: 0.8 },
  { field: 'LastResponseTime', headerName: 'LRT', flex: 0.8, type: 'dateTime', description: 'Last Response Time', valueFormatter: (params) => new Date(params.value).toUTCString() },
  { field: 'Public', headerName: 'Public', flex: 0.3, type: 'boolean' },
  { field: 'Sensors', headerName: 'Sensors Connected', flex: 0.5, valueFormatter: (params) => params.value.length, type: 'number', sortComparator: (v1, v2) => v1.length - v2.length},
  { field: 'UserInfo', headerName: 'Owner Email', flex: 1, align: 'center', headerAlign: 'center',
    sortComparator: (v1, v2) => {
      if (v1 !== null && v2 !== null) {
        return v1.Email.localeCompare(v2.Email);
      } else if (v1 === v2) {
        return 0;
      } else if (v1 === null) {
        return -1;
      } else {
        return 1;
      }
    },
    renderCell: (params) => params.value === null ? (<div>—</div>) : (
    <Link to={`/user/${params.value.ID}`} style={{color: 'inherit', textDecoration: 'none'}}>
      <Button variant="contained" color="primary" disableElevation>
       {params.value.Email}
      </Button>
    </Link>
  )}
];

export default function Microcontrollers() {
  const history = useHistory();

  return (
    <div>
      <PagedTable columns={columns} url={'api/Microcontrollers/GetAllMicrocontrollers'} detailsUrl={'/microcontroller/'} />
      <AddButton handleClick={() => history.push('/edit/microcontroller')} />
    </div>
  );
}