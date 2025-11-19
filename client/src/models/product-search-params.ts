export interface ProductSearchParams {
  searchTerm?: string;
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
  inStock?: boolean;
  sortBy?: string;
  sortOrder?: string;
  page?: number;
  pageSize?: number;
}
