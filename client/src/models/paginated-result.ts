/**
 * Wraps API responses for large datasets, returning a subset of items with navigation metadata.
 * Reduces payload size and improves performance by loading data in chunks.
 */
export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
