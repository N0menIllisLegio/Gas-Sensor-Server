import { useState, useEffect } from 'react';
import { MakeAuthorizedRequest, Request } from '../requests/Requests';
import { useSelector } from 'react-redux';
import { selectUser } from '../redux/reducers/authSlice';

export default function useGet(url) {
  const [data, setData] = useState(null);
  const [isPending, setIsPending] = useState(true);
  const [error, setError] = useState(null);
  const user = useSelector(selectUser);
  
  useEffect(() => {
    const abortCont = new AbortController();
    const getRequestFactory = (token) =>
      Request(url, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token ?? ''}`
        },
        signal: abortCont.signal
      });
    
    MakeAuthorizedRequest(getRequestFactory, user).then(response => {
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
  }, [url, user]);

  return { data, isPending, error };
}