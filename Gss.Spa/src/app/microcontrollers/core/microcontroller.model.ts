import { guid } from "../../core/guid";
import MicrocontrollerSensorModel from "./microcontroller-sensor.model";

export default interface MicrocontrollerModel {
    id: guid;
    lastResponseTime: Date | null;
    latitude: number | null;
    longitude: number | null;
    name: string;
    ownerId: guid;
    public: boolean;
    requestedSensorId: guid | null;
    sensors: MicrocontrollerSensorModel[];
}