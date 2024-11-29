import { guid } from "../../core/guid";
import SensorTypeModel from "../../sensor-types/core/sensor-type.model";

export default interface SensorModel {
    id: guid;
    name: string;
    description: string | null;
    type: SensorTypeModel;
}