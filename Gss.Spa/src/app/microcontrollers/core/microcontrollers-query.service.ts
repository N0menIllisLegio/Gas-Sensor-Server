import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import MapMicrocontrollerModel from "../map/map-microcontroller.model";
import PagedRequestModel from "../../core/paged-request.model";
import PagedResponseModel from "../../core/paged-response.model";
import MicrocontrollerModel from "./microcontroller.model";

@Injectable({ providedIn: 'root' })
export class MicrocontrollersQueryService {
    constructor(private httpClient: HttpClient) {}

    getMicrocontrollersMap(southWestLat: Number, southWestLong: Number, northEastLat: Number, northEastLong: Number) {
        return this.httpClient.get<MapMicrocontrollerModel[]>(`api/Microcontrollers/GetPublicMicrocontrollersMap?SouthWestLatitude=${southWestLat}&SouthWestLongitude=${southWestLong}&NorthEastLatitude=${northEastLat}&NorthEastLongitude=${northEastLong}`);
    }

    getPublicMicrocontrollers(pagedRequest: PagedRequestModel) {
        return this.httpClient.post<PagedResponseModel<MicrocontrollerModel>>('api/Microcontrollers/GetPublicMicrocontrollers', pagedRequest);
    }
}