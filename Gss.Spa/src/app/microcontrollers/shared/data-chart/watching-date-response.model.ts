export default class WatchingDateResponseModel {
    averageValue: number;
    readTime: string;
    watchingDate: string;

    constructor(averageValue: number, readTime: string, watchingDate: string) {
        this.averageValue = averageValue;
        this.readTime = readTime;
        this.watchingDate = watchingDate;
    }
}