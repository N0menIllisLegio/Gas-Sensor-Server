import { guid } from "../../core/guid";

export default interface SensorTypeModel {
    id: guid;
    name: string;
    Icon: string | null;
    Units: string | null;
}
