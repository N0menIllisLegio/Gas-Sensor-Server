import { guid } from "../../core/guid";

export default class EditSensorModel {
    constructor(
        public id: guid | null,
        public name: string,
        public description: string | null,
        public typeId: guid
    ) {}
}