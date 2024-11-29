import SortOptionModel from "./sort-option.model";

export default class PagedRequestModel {
    pageNumber: number;
    pageSize: number;
    searchString: string;
    sortOptions: SortOptionModel[];

    constructor(pageNumber: number, pageSize: number, searchString?: string, sortOptions?: SortOptionModel[]) {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
        this.searchString = searchString ?? '';
        this.sortOptions = sortOptions ?? [];
    }
}