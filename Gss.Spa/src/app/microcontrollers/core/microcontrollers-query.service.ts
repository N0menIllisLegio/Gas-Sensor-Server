import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import MicrocontrollersMapMicrocontrollerModel from "../microcontrollers-map/microcontrollers-map-microcontroller.model";
import PagedRequestModel from "../../core/paged-request.model";
import PagedResponseModel from "../../core/paged-response.model";
import MicrocontrollerModel from "./microcontroller.model";
import { map } from "rxjs";
import { guid } from "../../core/guid";

@Injectable({ providedIn: 'root' })
export class MicrocontrollersQueryService {
    constructor(private httpClient: HttpClient) {}

    getMicrocontrollersMap(southWestLat: Number, southWestLong: Number, northEastLat: Number, northEastLong: Number) {
        return this.httpClient.get<MicrocontrollersMapMicrocontrollerModel[]>(`api/Microcontrollers/GetPublicMicrocontrollersMap?SouthWestLatitude=${southWestLat}&SouthWestLongitude=${southWestLong}&NorthEastLatitude=${northEastLat}&NorthEastLongitude=${northEastLong}`);
    }

    getPublicMicrocontrollers(pagedRequest: PagedRequestModel) {
        return this.httpClient.post<PagedResponseModel<MicrocontrollerModel>>('api/Microcontrollers/GetPublicMicrocontrollers', pagedRequest)
            .pipe(
                map(response => {
                    response.items.forEach(x => {
                        if (x.lastResponseTime)
                            x.lastResponseTime = new Date(x.lastResponseTime)
                    });

                    return response;
                }
            ));
    }

    getMicrocontroller(id: guid) {
        return this.httpClient.get<MicrocontrollerModel>(`/api/Microcontrollers/GetMicrocontroller/${id}`)
            .pipe(map(x => {
                if (x.lastResponseTime)
                    x.lastResponseTime = new Date(x.lastResponseTime);

                return x;
            }));
    }

    getUserPublicMicrocontrollers(userId: guid, pagedRequest: PagedRequestModel) {
        return this.httpClient.post<PagedResponseModel<MicrocontrollerModel>>(`api/Microcontrollers/GetUserMicrocontrollers/${userId}`, pagedRequest)
            .pipe(
                map(response => {
                    response.items.forEach(x => {
                        if (x.lastResponseTime)
                            x.lastResponseTime = new Date(x.lastResponseTime)
                    });

                    return response;
                }
            ));
    }
}