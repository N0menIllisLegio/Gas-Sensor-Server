import { Map, Marker } from 'pigeon-maps';
import { makeStyles } from '@material-ui/core/styles';
import { useState } from 'react';
import { useVisibleMicrocontrollers } from '../hooks/usePost';
import { useHistory } from 'react-router-dom';

const useStyles = makeStyles((theme) => ({
  root: {
    height: `calc(100vh - (${theme.spacing(theme.mainContent.marginTop)}px + ${theme.spacing(theme.mainContent.padding)}px * 2))`,
  },
}));

export default function MicrocontrollersMap() {
  const classes = useStyles();
  const history = useHistory();
  const [neLatitude, setNeLatitude ] = useState(null);
  const [neLongitude, setNeLongitude ] = useState(null);
  const [swLatitude, setSwLatitude ] = useState(null);
  const [swLongitude, setSwLongitude ] = useState(null);

  const { data: visibleMicrocontrollers, isPending } = useVisibleMicrocontrollers(`${process.env.REACT_APP_SERVER_URL}api/Microcontrollers/GetPublicMicrocontrollersMap`,
    neLatitude, neLongitude, swLatitude, swLongitude);

  return (
    <div className={classes.root}>
      <Map
        center={[30, 0]}
        zoom={3}
        onBoundsChanged={e => {
          setNeLatitude(e.bounds.ne[0]);
          setNeLongitude(e.bounds.ne[1]);

          setSwLatitude(e.bounds.sw[0]);
          setSwLongitude(e.bounds.sw[1]);
        }}
        onClick={e => console.log(e)}>
          { visibleMicrocontrollers && visibleMicrocontrollers.length > 0 && (
            visibleMicrocontrollers.map(mc => (
              <Marker
                key={mc.MicrocontrollerID}
                width={50}
                anchor={[mc.Latitude, mc.Longitude]}
                onClick={() => history.push(`/microcontroller/${mc.MicrocontrollerID}`)} />
            ))
          )}
      </Map>
    </div>
  );
}