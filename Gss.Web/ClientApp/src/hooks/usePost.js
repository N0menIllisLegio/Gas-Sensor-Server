import { useState, useEffect } from 'react';

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

  useEffect(() => {
    const abortCont = new AbortController();
    
    fetch(url,
      {
        signal: abortCont.signal,
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          PageNumber: pageNumber,
          PageSize: pageSize,
          SearchString: searchString,
          SortOptions: sortOptions,
          Filters: filters
        })
      })
    .then(res => {
      if (!res.ok) {
        throw Error(`Failed to fetch data from ${url}`);
      }

      return res.json();
    })
    .then(data => {
      console.log(data);
      setData(data.Data);
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
  }, [url, pageNumber, pageSize, searchString, sortOptions, filters]);

  return { data, isPending, error };
}