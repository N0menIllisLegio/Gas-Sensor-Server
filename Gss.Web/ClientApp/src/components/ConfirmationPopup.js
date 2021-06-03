import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';

export default function ConfirmationPopup(props) {
  return (
    <Dialog
      open={props.open}
      onClose={props.handleDisagree}>
      <DialogTitle>{props.title}</DialogTitle>
      <DialogContent>
        <DialogContentText>
          {props.content}
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={props.handleDisagree} color="secondary">
          Disagree
        </Button>
        <Button onClick={props.handleAgree} color="primary" autoFocus>
          Agree
        </Button>
      </DialogActions>
    </Dialog>
  );
}