export interface Page<T> {
    hasNextPage: boolean;
    hasPreviousPage: boolean;
    isFirstPage: boolean;
    isLastPage: boolean;
    items: T[];
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
}
