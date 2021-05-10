import PagedTable from "../PagedTable";
import { Link } from 'react-router-dom'
import { Button } from "@material-ui/core";
import AddButton from "../AddButton";
import { useHistory } from 'react-router-dom';

const dateTimeOptions = {
  year: 'numeric',
  month: 'long',
  day: 'numeric',
  weekday: 'long',
  hour: 'numeric',
  minute: 'numeric',
  second: 'numeric',
  hour12: false
};

const columns = [
  { field: 'ID', headerName: 'ID', flex: 1 },
  { field: 'Name', headerName: 'Name', flex: 1 },
  { field: 'IPAddress', headerName: 'IP', flex: 1 },
  { field: 'LastResponseTime', headerName: 'LRT', flex: 1, type: 'dateTime', description: 'Last Response Time', valueFormatter: (params) => new Date(params.value).toLocaleString("en-US", dateTimeOptions) },
  { field: 'Public', headerName: 'Public', flex: 0.2, type: 'boolean' },
  { field: 'Sensors', headerName: 'Sensors Connected', flex: 0.4, valueFormatter: (params) => params.value.length, type: 'number', sortComparator: (v1, v2, cellParams1, cellParams2) => v1.length - v2.length},
  { field: 'UserInfo', headerName: 'Owner Email', flex: 0.6, align: 'center', headerAlign: 'center',
    sortComparator: (v1, v2, cellParams1, cellParams2) => {
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

export default function Users() {
  const history = useHistory();

  return (
    <div>
      <PagedTable columns={columns} url={'api/Microcontrollers/GetAllMicrocontrollers'} detailsUrl={'/microcontroller/'} />
      <AddButton handleClick={() => history.push('/microcontroller/create')} />
    </div>
  );
}