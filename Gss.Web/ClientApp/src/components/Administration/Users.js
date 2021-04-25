import Table from "../Table";

const columns = [
  { field: 'ID', headerName: 'ID', flex: 1 },
  { field: 'Email', headerName: 'Email', flex: 1 },
  { field: 'FirstName', headerName: 'First name', flex: 1 },
  { field: 'LastName', headerName: 'Last name', flex: 1 },
  { field: 'AccessFailedCount', headerName: 'Access failed times', flex: 0.4, type: 'number' }
];

export default function Users() {
  return (
    <div>
      <Table columns={columns} url={'api/Users/GetAllUsers'} detailsUrl={'/user/'} />
    </div>
  );
}