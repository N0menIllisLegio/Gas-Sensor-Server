import PagedRequestModel from "./paged-request.model";

export default interface PagedResponseModel<TModel> {
    items: TModel[];
    pagedInfo: PagedRequestModel;
    totalItemsCount: number;
}