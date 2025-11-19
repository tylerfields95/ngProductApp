import { Injectable, signal } from '@angular/core';
import { tap } from 'rxjs/operators';
import { Product, PaginatedResult, ProductSearchParams } from '../models';
import { ProductHttpService } from './product-http.service';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly DEFAULT_PAGE = 1;
  private readonly DEFAULT_PAGE_SIZE = 50;

  // Minimal signal-based state management
  private resultSignal = signal<PaginatedResult<Product> | null>(null);
  private loadingSignal = signal<boolean>(false);

  // Store current search parameters to preserve them across page changes
  private currentSearchParams: ProductSearchParams = {};

  // Public readonly signals
  readonly result = this.resultSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();

  constructor(private productHttp: ProductHttpService) {
    // Load initial data
    this.searchProducts({});
  }

  /**
   * Load product by ID
   */
  loadProductById(id: number): void {
    this.loadingSignal.set(true);

    this.productHttp
      .getById(id)
      .pipe(
        tap({
          next: () => {
            this.loadingSignal.set(false);
          },
          error: () => {
            this.loadingSignal.set(false);
          },
        })
      )
      .subscribe();
  }

  /**
   * Create a new product
   */
  createProduct(product: Product): void {
    this.loadingSignal.set(true);

    this.productHttp
      .create(product)
      .pipe(
        tap({
          next: (newProduct) => {
            const currentResult = this.resultSignal();
            if (currentResult) {
              this.resultSignal.set({
                ...currentResult,
                items: [...currentResult.items, newProduct],
                totalCount: currentResult.totalCount + 1,
              });
            }
            this.loadingSignal.set(false);
          },
          error: () => {
            this.loadingSignal.set(false);
          },
        })
      )
      .subscribe();
  }

  /**
   * Update an existing product
   */
  updateProduct(product: Product): void {
    this.loadingSignal.set(true);

    this.productHttp
      .update(product)
      .pipe(
        tap({
          next: (updatedProduct) => {
            const currentResult = this.resultSignal();
            if (currentResult) {
              this.resultSignal.set({
                ...currentResult,
                items: currentResult.items.map((p) =>
                  p.id === updatedProduct.id ? updatedProduct : p
                ),
              });
            }
            this.loadingSignal.set(false);
          },
          error: () => {
            this.loadingSignal.set(false);
          },
        })
      )
      .subscribe();
  }

  /**
   * Delete a product
   */
  deleteProduct(id: number): void {
    this.loadingSignal.set(true);

    this.productHttp
      .delete(id)
      .pipe(
        tap({
          next: () => {
            const currentResult = this.resultSignal();
            if (currentResult) {
              this.resultSignal.set({
                ...currentResult,
                items: currentResult.items.filter((p) => p.id !== id),
                totalCount: currentResult.totalCount - 1,
              });
            }
            this.loadingSignal.set(false);
          },
          error: () => {
            this.loadingSignal.set(false);
          },
        })
      )
      .subscribe();
  }

  /**
   * Search products with optional filters
   */
  searchProducts(searchParams: ProductSearchParams = {}): void {
    this.loadingSignal.set(true);

    // Merge with current search params to preserve filters across page changes
    this.currentSearchParams = {
      ...this.currentSearchParams,
      ...searchParams,
    };

    // Apply defaults only if not provided
    const params: ProductSearchParams = {
      ...this.currentSearchParams,
      page: this.currentSearchParams.page ?? this.DEFAULT_PAGE,
      pageSize: this.currentSearchParams.pageSize ?? this.DEFAULT_PAGE_SIZE,
    };

    this.productHttp
      .search(params)
      .pipe(
        tap({
          next: (result) => {
            this.resultSignal.set(result);
            this.loadingSignal.set(false);
          },
          error: () => {
            this.loadingSignal.set(false);
          },
        })
      )
      .subscribe();
  }
}
