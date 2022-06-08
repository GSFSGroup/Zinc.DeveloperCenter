export interface Results<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalRows: number;
}
