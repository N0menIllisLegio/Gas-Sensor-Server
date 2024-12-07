import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import PagedRequestModel from "../../core/paged-request.model";
import PagedResponseModel from "../../core/paged-response.model";
import SensorTypeModel from "./sensor-type.model";

@Injectable({ providedIn: 'root' })
export class SensorTypesQueryService {
    constructor(private httpClient: HttpClient) {}

    getPublicMicrocontrollers(pagedRequest: PagedRequestModel) {
        return this.httpClient.post<PagedResponseModel<SensorTypeModel>>('api/SensorsTypes/GetAllSensorsTypes', pagedRequest);
    }
}