import { guid } from "../../core/guid";
import SensorModel from "../../sensors/core/sensor.model";

export default interface MicrocontrollerSensorModel extends SensorModel {
    criticalValue: number | null;
    microcontrollerSensorId: guid;
}