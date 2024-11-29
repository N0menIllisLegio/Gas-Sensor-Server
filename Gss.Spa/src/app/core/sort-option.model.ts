export default class SortOptionModel {
    isAscending: boolean;
    propertyName: string;

    constructor(isAscending: boolean, propertyName: string) {
        this.isAscending = isAscending;
        this.propertyName = propertyName;
    }
}