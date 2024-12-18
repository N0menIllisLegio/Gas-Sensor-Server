import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import MicrocontrollersMapMicrocontrollerModel from "../microcontrollers-map/microcontrollers-map-microcontroller.model";
import PagedRequestModel from "../../core/paged-request.model";
import PagedResponseModel from "../../core/paged-response.model";
import MicrocontrollerModel from "./microcontroller.model";
import { map } from "rxjs";
import { guid } from "../../core/guid";
import EditMicrocontrollerModel from "../edit-microcontroller/edit-microcontroller.model";

@Injectable({ providedIn: 'root' })
export class MicrocontrollersQueryService {
    constructor(private httpClient: HttpClient) {}

    getMicrocontrollersMap(southWestLat: Number, southWestLong: Number, northEastLat: Number, northEastLong: Number) {
        return this.httpClient.get<MicrocontrollersMapMicrocontrollerModel[]>(`api/Microcontrollers/GetPublicMicrocontrollersMap?SouthWestLatitude=${southWestLat}&SouthWestLongitude=${southWestLong}&NorthEastLatitude=${northEastLat}&NorthEastLongitude=${northEastLong}`);
    }

    getPublicMicrocontrollers(pagedRequest: PagedRequestModel) {
        return this.httpClient.post<PagedResponseModel<MicrocontrollerModel>>('api/Microcontrollers/GetPublicMicrocontrollers', pagedRequest);
    }

    getMicrocontroller(id: guid) {
        return this.httpClient.get<MicrocontrollerModel>(`/api/Microcontrollers/GetMicrocontroller/${id}`);
    }

    getUserPublicMicrocontrollers(userId: guid, pagedRequest: PagedRequestModel) {
        return this.httpClient.post<PagedResponseModel<MicrocontrollerModel>>(`api/Microcontrollers/GetUserMicrocontrollers/${userId}`, pagedRequest);
    }

    deleteMicrocontroller(id: guid) {
        return this.httpClient.delete(`api/Microcontrollers/Delete/${id}`);
    }

    setTreshold(microcontrollerSensorId: guid, threshold: number | null) {
        return this.httpClient.patch('api/Microcontrollers/SetSensorsCriticalValue', {
            microcontrollerSensorId,
            criticalValue: threshold
        });
    }

    createMicrocontroller(newMicrocontroller: EditMicrocontrollerModel, newSensors: guid[]) {
        return this.httpClient.post<{ id: guid }>('api/Microcontrollers/Create', {
            name: newMicrocontroller.name,
            public: newMicrocontroller.isPublic,
            latitude: newMicrocontroller.latitude === '' ? null : newMicrocontroller.latitude,
            longitude: newMicrocontroller.longitude === '' ? null : newMicrocontroller.longitude,
            key: newMicrocontroller.key === '' ? null : newMicrocontroller.key,
            sensorIDs: newSensors,
        });
    }

    updateMicrocontroller(newMicrocontroller: EditMicrocontrollerModel, newSensors: guid[], originalMicrocontroller: MicrocontrollerModel) {
        return this.httpClient.put(`api/Microcontrollers/Update/${originalMicrocontroller.id}`, {
            name: newMicrocontroller.name,
            public: newMicrocontroller.isPublic,
            latitude: newMicrocontroller.latitude === '' ? null : newMicrocontroller.latitude,
            longitude: newMicrocontroller.longitude === '' ? null : newMicrocontroller.longitude,
            key: newMicrocontroller.key === '' ? null : newMicrocontroller.key,
            addSensorIds: newSensors.filter(x => originalMicrocontroller.sensors.find(y => y.id === x) === undefined),
            removeMicrocontrollerSensorIds: originalMicrocontroller.sensors.filter(x => !newSensors.includes(x.id)).map(x => x.microcontrollerSensorId)
        });
    }
}