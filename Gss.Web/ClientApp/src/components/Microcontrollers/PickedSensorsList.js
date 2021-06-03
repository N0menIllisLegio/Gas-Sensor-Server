import { Avatar, ListItemAvatar, ListItemSecondaryAction, IconButton, List, ListItem, ListItemText } from "@material-ui/core";
import DeleteForeverTwoToneIcon from '@material-ui/icons/DeleteForeverTwoTone';

export default function PickedSensorsList(props) {
  const pickedSensors = props.pickedSensors;

  return (
    <List dense>
      {pickedSensors.map(sensor => (
        <ListItem key={sensor.ID}>
          <ListItemAvatar>
            <Avatar src={sensor.SensorType.Icon}>
              {sensor.SensorType.Units}
            </Avatar>
          </ListItemAvatar>
          <ListItemText primary={sensor.Name} secondary={sensor.SensorType.Name} />
          <ListItemSecondaryAction>
            <IconButton edge="end" onClick={() => props.handleDeletePickedSensor(sensor)}>
              <DeleteForeverTwoToneIcon color="secondary" />
            </IconButton>
          </ListItemSecondaryAction>
        </ListItem>
        )
      )}
    </List>
  );
}