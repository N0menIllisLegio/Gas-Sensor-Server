import MicrocontrollerModel from "./microcontroller.model";

export default interface ExtendedMicrocontrollerModel extends MicrocontrollerModel {
    latestResponse: Date | null;
}