import { guid } from "../../core/guid";
import SensorTypeModel from "../../sensor-types/core/sensor-type.model";

export default interface MapMicrocontrollerModel {
    microcontrollerId: guid;
    latitude: number;
    longitude: number;
    sensorTypes: SensorTypeModel[];
}