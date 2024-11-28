import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import MapMicrocontrollerModel from "../map/map-microcontroller.model";

@Injectable({ providedIn: 'root' })
export class MicrocontrollersQueryService {
    constructor(private httpClient: HttpClient) {}

    getMicrocontrollersMap(southWestLat: Number, southWestLong: Number, northEastLat: Number, northEastLong: Number) {
        return this.httpClient.get<MapMicrocontrollerModel[]>(`api/Microcontrollers/GetPublicMicrocontrollersMap?SouthWestLatitude=${southWestLat}&SouthWestLongitude=${southWestLong}&NorthEastLatitude=${northEastLat}&NorthEastLongitude=${northEastLong}`);
    }
}