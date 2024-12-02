export default class FlyToTargetModel {
    latitude: number;
    longitude: number;
    zoom: number;

    constructor(lat: number, lng: number, zoom: number) {
        this.latitude = lat;
        this.longitude = lng;
        this.zoom = zoom;
    }
}