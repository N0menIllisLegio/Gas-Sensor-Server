import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import PagedRequestModel from "../../core/paged-request.model";
import PagedResponseModel from "../../core/paged-response.model";
import SensorTypeModel from "./sensor-type.model";
import EditSensorTypeModel from "../edit-sensor-type-dialog/edit-sensor-type.model";
import { guid } from "../../core/guid";

@Injectable({ providedIn: 'root' })
export class SensorTypesQueryService {
    constructor(private httpClient: HttpClient) {}

    getSensorTypes(pagedRequest: PagedRequestModel) {
        return this.httpClient.post<PagedResponseModel<SensorTypeModel>>('api/SensorsTypes/GetAllSensorsTypes', pagedRequest);
    }

    updateSensorType(sensorType: EditSensorTypeModel) {
        return this.httpClient.put(`api/SensorsTypes/Update/${sensorType.id}`, {
            name: sensorType.name,
            icon: sensorType.icon,
            units: sensorType.units
        });
    }

    createSensorType(sensorType: EditSensorTypeModel) {
        return this.httpClient.post<SensorTypeModel>(`api/SensorsTypes/Create`, {
            name: sensorType.name,
            icon: sensorType.icon,
            units: sensorType.units
        });
    }

    deleteSensorType(sensorTypeId: guid) {
        return this.httpClient.delete(`api/SensorsTypes/Delete/${sensorTypeId}`);
    }
}