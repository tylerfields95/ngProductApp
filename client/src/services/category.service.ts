import { Injectable, signal } from '@angular/core';
import { tap } from 'rxjs/operators';
import { Category } from '../models';
import { CategoryHttpService } from './category-http.service';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  // Minimal signal-based state management
  private categoriesSignal = signal<Category[]>([]);
  private loadingSignal = signal<boolean>(false);

  // Public readonly signals
  readonly categories = this.categoriesSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();

  constructor(private categoryHttp: CategoryHttpService) {
    // Load initial data
    this.loadCategories();
  }

  /**
   * Load all categories
   */
  loadCategories(): void {
    this.loadingSignal.set(true);

    this.categoryHttp
      .getAll()
      .pipe(
        tap({
          next: (categories) => {
            this.categoriesSignal.set(categories);
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
   * Load category by ID
   */
  loadCategoryById(id: number): void {
    this.loadingSignal.set(true);

    this.categoryHttp
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
   * Create a new category
   */
  createCategory(category: Category): void {
    this.loadingSignal.set(true);

    this.categoryHttp
      .create(category)
      .pipe(
        tap({
          next: (newCategory) => {
            this.categoriesSignal.update((categories) => [
              ...categories,
              newCategory,
            ]);
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
