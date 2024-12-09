import SensorModel from "../../sensors/core/sensor.model";

export default class EditMicrocontrollerModel {
    constructor(
        public name: string,
        public isPublic: boolean,
        public latitude: string,
        public longitude: string,
        public key: string,
        public sensors: SensorModel[]
    ) {}
}
