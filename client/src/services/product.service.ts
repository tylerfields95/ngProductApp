import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, PaginatedResult, ProductSearchParams } from '../models';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  // targetting http for now
  private apiUrl = 'http://localhost:5258/api/product'; // Update with your API URL

  constructor(private http: HttpClient) {}

  /**
   * Get all active products with pagination
   */
  getAll(
    page: number = 1,
    pageSize: number = 50
  ): Observable<PaginatedResult<Product>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PaginatedResult<Product>>(this.apiUrl, { params });
  }

  /**
   * Get product by ID
   */
  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new product
   */
  create(product: Product): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }

  /**
   * Update an existing product
   */
  update(product: Product): Observable<Product> {
    return this.http.put<Product>(this.apiUrl, product);
  }

  /**
   * Soft delete a product
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Search products with optional filters
   */
  search(
    searchParams: ProductSearchParams
  ): Observable<PaginatedResult<Product>> {
    let params = new HttpParams();

    if (searchParams.searchTerm)
      params = params.set('searchTerm', searchParams.searchTerm);
    if (searchParams.categoryId !== undefined)
      params = params.set('categoryId', searchParams.categoryId.toString());
    if (searchParams.minPrice !== undefined)
      params = params.set('minPrice', searchParams.minPrice.toString());
    if (searchParams.maxPrice !== undefined)
      params = params.set('maxPrice', searchParams.maxPrice.toString());
    if (searchParams.inStock !== undefined)
      params = params.set('inStock', searchParams.inStock.toString());
    if (searchParams.sortBy) params = params.set('sortBy', searchParams.sortBy);
    if (searchParams.sortOrder)
      params = params.set('sortOrder', searchParams.sortOrder);
    if (searchParams.page !== undefined)
      params = params.set('page', searchParams.page.toString());
    if (searchParams.pageSize !== undefined)
      params = params.set('pageSize', searchParams.pageSize.toString());

    return this.http.get<PaginatedResult<Product>>(`${this.apiUrl}/search`, {
      params,
    });
  }
}
