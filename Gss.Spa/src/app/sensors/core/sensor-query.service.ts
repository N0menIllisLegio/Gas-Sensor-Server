import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import PagedRequestModel from "../../core/paged-request.model";
import PagedResponseModel from "../../core/paged-response.model";
import { guid } from "../../core/guid";
import EditSensorModel from "../edit-sensor-dialog/edit-sensor.model";
import SensorModel from "./sensor.model";

@Injectable({ providedIn: 'root' })
export class SensorsQueryService {
    constructor(private httpClient: HttpClient) {}

    getSensors(pagedRequest: PagedRequestModel) {
        return this.httpClient.post<PagedResponseModel<SensorModel>>('api/Sensors/GetAllSensors', pagedRequest);
    }

    updateSensor(sensor: EditSensorModel) {
        return this.httpClient.put(`api/Sensors/Update/${sensor.id}`, {
            name: sensor.name,
            description: sensor.description,
            typeId: sensor.typeId
        });
    }

    createSensor(sensor: EditSensorModel) {
        return this.httpClient.post<SensorModel>(`api/Sensors/Create`, {
            name: sensor.name,
            description: sensor.description,
            typeId: sensor.typeId
        });
    }

    deleteSensor(sensorId: guid) {
        return this.httpClient.delete(`api/Sensors/Delete/${sensorId}`);
    }
}