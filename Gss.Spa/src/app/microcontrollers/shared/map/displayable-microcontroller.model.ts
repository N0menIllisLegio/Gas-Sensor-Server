export default class DisplayableMicrocontrollerModel {
    latitude: number;
    longitude: number;
    sensorTypeNames?: string[];

    constructor(lat: number, lng: number, sensorTypeNames?: string[]) {
        this.latitude = lat;
        this.longitude = lng;
        this.sensorTypeNames = sensorTypeNames;
    }
}