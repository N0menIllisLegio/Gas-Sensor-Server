import { guid } from "../../../core/guid"
import { watchingPeriod } from "./watching-period.type";

export default class WatchingDateRequestModel {
    microcontrollerSensorId: guid;
    period: watchingPeriod;
    watchingDates: string[];

    constructor(microcontrollerSensorId: guid, period: watchingPeriod, watchingDates: string[]) {
        this.microcontrollerSensorId = microcontrollerSensorId;
        this.period = period;
        this.watchingDates = watchingDates;
    }

    equals(comparingModel: WatchingDateRequestModel) {
        return this.microcontrollerSensorId === comparingModel.microcontrollerSensorId
            && this.period === comparingModel.period
            && this.watchingDates.every(x => comparingModel.watchingDates.includes(x));
    }
}