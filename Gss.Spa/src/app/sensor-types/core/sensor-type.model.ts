import { guid } from "../../core/guid";

export default interface SensorTypeModel {
    id: guid;
    name: string;
    icon: string | null;
    units: string | null;
}
