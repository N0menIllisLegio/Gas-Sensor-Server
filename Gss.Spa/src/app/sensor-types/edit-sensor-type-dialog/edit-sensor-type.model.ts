import { guid } from "../../core/guid";

export default class EditSensorTypeModel {
    constructor(
        public id: guid | null,
        public icon: string | null,
        public name: string,
        public units: string | null
    ) {}
}