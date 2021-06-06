import PagedTable from "../PagedTable";
import { Button } from "@material-ui/core";
import { MakeAuthorizedRequest, DeleteRequest } from '../../requests/Requests';
import { useSelector } from 'react-redux';
import { selectUser } from '../../redux/reducers/authSlice';
import Snackbar from '@material-ui/core/Snackbar';
import MuiAlert from '@material-ui/lab/Alert';
import { useState, useEffect } from 'react';
import { useHistory } from 'react-router-dom';
import ConfirmationPopup from "../ConfirmationPopup";

let deleteUserID = null;
let deleteUserEmail = null;
let openConfirmationPopupFunc = null;

const columns = [
  { field: 'ID', headerName: 'ID', flex: 1 },
  { field: 'Email', headerName: 'Email', flex: 1 },
  { field: 'FirstName', headerName: 'First name', flex: 1 },
  { field: 'LastName', headerName: 'Last name', flex: 1 },
  { field: 'AccessFailedCount', headerName: 'Access failed times', flex: 0.4, type: 'number' },
  { field: 'id', headerName: 'DELETE USER', flex: 0.6, align: 'center', headerAlign: 'center',
    sortComparator: (v1, v2) => {
      if (v1 !== null && v2 !== null) {
        return v1.localeCompare(v2);
      } else if (v1 === v2) {
        return 0;
      } else if (v1 === null) {
        return -1;
      } else {
        return 1;
      }
    },
    renderCell: (params) => (
      <Button variant="contained" color="secondary" disableElevation onClick={() => {
        deleteUserID = params.value;
        deleteUserEmail = params.row.Email;

        if (openConfirmationPopupFunc != null) {
          openConfirmationPopupFunc();
        }
      }}>
        DELETE USER
      </Button>
    )
  }
];

export default function Users() {
  const user = useSelector(selectUser);
  const history = useHistory();
  const [open, setOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const [snackbarSeverity, setSnackbarSeverity] = useState('error');
  const [userChanged, setUserChanged] = useState(false);
  const [usersUrl, setUsersUrl] = useState('api/Users/GetAllUsers');

  useEffect(() => {
    if (userChanged) {
      setUsersUrl('api/Users/GetAllUsers/');
    } else {
      setUsersUrl('api/Users/GetAllUsers');
    }
  }, [userChanged]);

  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }

    setOpen(false);
  };

  const [openConfirmationPopup, setOpenConfirmationPopup] = useState(false);

  openConfirmationPopupFunc = () => setOpenConfirmationPopup(true);

  let handleAgreeConfirmationPopupAction = async (id) => {
    setOpenConfirmationPopup(false);
    
    const deleteUserRequestFactory = (token) =>
      DeleteRequest(`${process.env.REACT_APP_SERVER_URL}api/Users/Delete/${id}`, token);
  
    const deleteResponse = await MakeAuthorizedRequest(deleteUserRequestFactory, user);

    deleteUserID = null;

    if (deleteResponse.status !== 200) {
      if (deleteResponse.status === 401) {
        history.push(process.env.REACT_APP_UNAUTHORIZED_URL);
      } else if (deleteResponse.status === 500) {
        history.push(process.env.REACT_APP_SERVER_ERROR_URL);
      } else {
        setSnackbarSeverity('error');
        setSnackbarMessage(deleteResponse.errors[0]);
        setOpen(true);
      }
    } else {
      setSnackbarSeverity('success');
      setSnackbarMessage(`${deleteResponse.data.FirstName} ${deleteResponse.data.LastName} has been deleted!`);
      setOpen(true);
      setUserChanged(!userChanged);
    }
  };

  let handleDisagreeConfirmationPopupAction = () => {
    deleteUserID = null;
    deleteUserEmail = null;

    setOpenConfirmationPopup(false);
  };

  return (
    <div>
      <PagedTable columns={columns} url={usersUrl} detailsUrl={'/user/'} />

      <Snackbar open={open} autoHideDuration={5000} onClose={handleClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <MuiAlert elevation={6} variant="filled" onClose={handleClose} severity={snackbarSeverity}>
          {snackbarMessage}
        </MuiAlert>
      </Snackbar>

      <ConfirmationPopup
        open={openConfirmationPopup}
        handleAgree={() => deleteUserID && handleAgreeConfirmationPopupAction(deleteUserID)}
        handleDisagree={handleDisagreeConfirmationPopupAction}
        title="Delete User?"
        content={<span>Do you realy want to delete <b>{deleteUserEmail}</b> user?</span>} />
    </div>
  );
}