import PagedTable from "../PagedTable";
import { Avatar } from "@material-ui/core";
import AddButton from "../AddButton";

// https://sensorsappimagestorage.blob.core.windows.net/thumbnails/0c896b43-2794-4d3e-a8eb-39dbd3afc3e6.png
const columns = [
  {field: 'Icon', headerName: 'Icon', flex: 0.1, align: 'center', headerAlign: 'center',
    renderCell: (params) => (
      <Avatar src={params.value} variant="square">{params.row.Units}</Avatar>
  )},
  { field: 'ID', headerName: 'ID', flex: 0.5 },
  { field: 'Units', headerName: 'Units', flex: 0.2, align: 'center', headerAlign: 'center' },
  { field: 'Name', headerName: 'Name', flex: 1 },
]

export default function Sensors() {
  return (
    <div>
      <PagedTable columns={columns} url={'api/SensorsTypes/GetAllSensorsTypes'} detailsUrl={'/sensorsType/'} />
      <AddButton url={'/sensorsType/create'} />
    </div>
  );
}