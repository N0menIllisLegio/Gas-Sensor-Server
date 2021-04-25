import { DataGrid } from "@material-ui/data-grid";
import usePagedPost from "../../hooks/usePost";
import Progress from "../Progress";
import { useState, useEffect } from 'react';
import TextField from '@material-ui/core/TextField';
import { makeStyles } from '@material-ui/core/styles';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import InputLabel from '@material-ui/core/InputLabel';
import { useHistory } from "react-router-dom";

const useStyles = makeStyles((theme) => ({
  inputsRow: {
    margin: theme.spacing(2),
    display: 'flex',
    justifyContent: 'space-between'
  },
  formControl: {
    margin: theme.spacing(1),
    minWidth: 120,
  }
}));

const columns = [
  { field: 'ID', headerName: 'ID', flex: 1 },
  { field: 'Email', headerName: 'Email', flex: 1 },
  { field: 'FirstName', headerName: 'First name', flex: 1 },
  { field: 'LastName', headerName: 'Last name', flex: 1 },
  { field: 'AccessFailedCount', headerName: 'Access failed times', flex: 0.4, type: 'number' }
];

export default function Users() {
  const classes = useStyles();
  const history = useHistory();
  const [rows, setRows] = useState([]);
  const [searchString, setSearchString] = useState('');
  const [pageSize, setPageSize] = useState(20);

  const { data: pagedResponse, isPending } = usePagedPost(
    `${process.env.REACT_APP_SERVER_URL}api/Users/GetAllUsers`, 0, pageSize, searchString);

  useEffect(() => {
    if (pagedResponse !== null) {
      setRows(pagedResponse.Items.map((user) => {
        user.id = user.ID;
        return user;
      }));
    }
  }, [pagedResponse]);

  return (
    <div>
      { isPending && (<Progress />) }
      {
        pagedResponse && (
          <div>
            <div className={classes.inputsRow}>
              <TextField label="Search..." value={searchString} onChange={(e) => setSearchString(e.target.value)}/>

              <FormControl variant="outlined" className={classes.formControl}>
                <InputLabel>Elements</InputLabel>
                <Select label="Elements" 
                  value={pageSize}
                  onChange={(e) => setPageSize(e.target.value)}>
                  <MenuItem value={10}>10</MenuItem>
                  <MenuItem value={20}>20</MenuItem>
                  <MenuItem value={30}>30</MenuItem>
                </Select>
              </FormControl>
            </div>
            <div style={{ height: 800, width: '100%' }}>
              <DataGrid rows={rows} columns={columns} pageSize={pageSize}
                disableColumnMenu hideFooter disableMultipleSelection onRowSelected={(e) => history.push(`/user/${e.data.id}`)} />
            </div>
          </div>
        )
      }
    </div>
  );
}