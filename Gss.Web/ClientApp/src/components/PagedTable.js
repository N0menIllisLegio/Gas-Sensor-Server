import { DataGrid } from "@material-ui/data-grid";
import { usePagedPost } from "../hooks/usePost";
import Progress from "./Progress";
import { useState, useEffect } from 'react';
import TextField from '@material-ui/core/TextField';
import { makeStyles } from '@material-ui/core/styles';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import InputLabel from '@material-ui/core/InputLabel';
import { useHistory } from "react-router-dom";
import { Pagination } from "@material-ui/lab";

const useStyles = makeStyles((theme) => ({
  inputsRow: {
    margin: theme.spacing(2),
    display: 'flex',
    justifyContent: 'space-between', 
    alignItems: 'center'
  },
  formControl: {
    margin: theme.spacing(1),
    minWidth: 120,
  }
}));

export default function PagedTable(props) {
  const classes = useStyles();
  const history = useHistory();
  const [rows, setRows] = useState([]);
  const [searchString, setSearchString] = useState('');
  const [totalPagesNumber, setTotalPagesNumber] = useState(1);
  const [currentPageNumber, setCurrentPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(20);

  const { data: pagedResponse, isPending } = usePagedPost(
    `${process.env.REACT_APP_SERVER_URL}${props.url}`, currentPageNumber, pageSize, searchString);

  useEffect(() => {
    if (pagedResponse !== null) {
      setRows(pagedResponse.Items.map((item) => {
        item.id = item.ID;
        return item;
      }));
      
      setTotalPagesNumber(Math.ceil(pagedResponse.TotalItemsCount / pagedResponse.PagedInfo.PageSize));
    }
  }, [pagedResponse]);

  useEffect(() => {
    setCurrentPageNumber(1);
  }, [searchString]);

  return (
    <div>
      { isPending && (<Progress />) }
      {
        pagedResponse && (
          <div>
            <div className={classes.inputsRow}>
              <TextField label="Search..." value={searchString} onChange={(e) => setSearchString(e.target.value)}/>

              {totalPagesNumber > 1 &&
                (
                  <Pagination
                    variant="outlined"
                    color="primary"
                    count={totalPagesNumber}
                    page={currentPageNumber}
                    onChange={(e, pageNumber) => setCurrentPageNumber(pageNumber)} />
                )
              }

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
              <DataGrid
                disableColumnMenu
                hideFooter
                disableMultipleSelection
                rows={rows}
                columns={props.columns}
                pageSize={pageSize}
                onRowDoubleClick={(e) => {
                  if (props.detailsUrl != null) {
                    history.push(`${props.detailsUrl}${e.id}`)
                  }
                }} />
            </div>
          </div>
        )
      }
    </div>
  );
}