export default class SortOptionModel {
    isAscending: boolean;
    propertyName: string;

    constructor(isAscending: boolean, propertyName: string) {
        this.isAscending = isAscending;
        this.propertyName = propertyName;
    }

    equals(comparingModel: SortOptionModel) {
        return this.isAscending === comparingModel.isAscending
            && this.propertyName === comparingModel.propertyName;
    }
}