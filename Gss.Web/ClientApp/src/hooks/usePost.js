import { useState, useEffect } from 'react';
import { MakeAuthorizedRequest, Request } from '../requests/Requests';
import { useSelector } from 'react-redux';
import { selectUser } from '../redux/reducers/authSlice';

// SortOptions: [
//   {
//     Order: "None",
//     PropertyName: "string"
//   }
// ],
// Filters: [
//   {
//     PropertyName: "string",
//     Value: "string",
//     OperatorType: "Contains"
//   }
// ]

export function usePagedPost(url, pageNumber, pageSize,
  searchString = "", sortOptions = null, filters = null) {

  const [data, setData] = useState(null);
  const [isPending, setIsPending] = useState(true);
  const [error, setError] = useState(null);
  const user = useSelector(selectUser);
  
  useEffect(() => {
    const abortCont = new AbortController();
    const postRequestFactory = (token) =>
      Request(url, {
        signal: abortCont.signal,
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token ?? ''}`
        },
        body: JSON.stringify({
          PageNumber: pageNumber,
          PageSize: pageSize,
          SearchString: searchString,
          SortOptions: sortOptions,
          Filters: filters
        })
      });
    
    MakeAuthorizedRequest(postRequestFactory, user)
      .then(response => {
        if (response.status === 200) {
          setData(response.data);
          setError(null);
        } else if (response.errors[0] === 'AbortError') {
          console.log(`Fetch from ${url} aborted.`);
        } else {
          setError(response.errors);
        }
      
        setIsPending(false);
      });

    return () => abortCont.abort();
  }, [url, user, pageNumber, pageSize, searchString, sortOptions, filters]);

  return { data, isPending, error };
}

export function useSensorDataPost(url, microcontrollerID, sensorID, period, watchingDates) {
  const [data, setData] = useState(null);
  const [isPending, setIsPending] = useState(true);
  const [error, setError] = useState(null);
  const user = useSelector(selectUser);

  useEffect(() => {
    const abortCont = new AbortController();
    const postRequestFactory = (token) =>
      Request(url, {
        signal: abortCont.signal,
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token ?? ''}`
        },
        body: JSON.stringify({
          MicrocontrollerID: microcontrollerID,
          SensorID: sensorID,
          Period: period,
          WatchingDates: watchingDates
        })
      });

    if (watchingDates == null || watchingDates.length === 0) {
      setIsPending(false);
    } else {
    
      MakeAuthorizedRequest(postRequestFactory, user)
        .then(response => {
          if (response.status === 200) {
            setData(response.data);
            setError(null);
          } else if (response.errors[0] === 'AbortError') {
            console.log(`Fetch from ${url} aborted.`);
          } else {
            setError(response.errors);
          }
        
          setIsPending(false);
        });
    }

    return () => abortCont.abort();
  }, [url, user, microcontrollerID, sensorID, period, watchingDates]);
  
  return { data, isPending, error };
}

export function useVisibleMicrocontrollers(url, neLat, neLong, swLat, swLong) {
  const [data, setData] = useState(null);
  const [isPending, setIsPending] = useState(true);
  const [error, setError] = useState(null);
  const user = useSelector(selectUser);

  useEffect(() => {
    const abortCont = new AbortController();
    const postRequestFactory = (token) =>
      Request(url, {
        signal: abortCont.signal,
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token ?? ''}`
        },
        body: JSON.stringify({
          SouthWestLatitude: swLat,
          SouthWestLongitude: swLong,
          NorthEastLatitude: neLat,
          NorthEastLongitude: neLong
        })
      });

    if (neLat == null || neLong == null || swLat == null || swLong == null) {
      setIsPending(false);
    } else {
    
      MakeAuthorizedRequest(postRequestFactory, user)
        .then(response => {
          if (response.status === 200) {
            setData(response.data);
            setError(null);
          } else if (response.errors[0] === 'AbortError') {
            console.log(`Fetch from ${url} aborted.`);
          } else {
            setError(response.errors);
          }
        
          setIsPending(false);
        });
    }

    return () => abortCont.abort();
  }, [url, user, neLat, neLong, swLat, swLong]);
  
  return { data, isPending, error };
}
