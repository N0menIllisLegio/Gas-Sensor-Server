import { useState, useEffect } from 'react';

export default function useGet(url) {
  const [data, setData] = useState(null);
  const [isPending, setIsPending] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const abortCont = new AbortController();

    fetch(url, { signal: abortCont.signal })
    .then(res => {
      if (!res.ok) {
        throw Error(`Failed to fetch data from ${url}`);
      }

      return res.json();
    })
    .then(data => {
      console.log(data);
      setData(data);
      setIsPending(false);
      setError(null);
    })
    .catch(err => {
      if (err.Name === 'AbortError') {
        console.log(`Fetch from ${url} aborted.`);
      } else {
        setIsPending(false);
        setError(err.Message);
      }
    });

    return () => abortCont.abort();
  }, [url]);

  return { data, isPending, error };
}