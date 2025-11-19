import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { CategoryService } from '../../../services/category.service';
import { ProductSearchParams } from '../../../models';

@Component({
  selector: 'app-product-search-header',
  imports: [CommonModule, FormsModule],
  templateUrl: './product-search-header.component.html',
  styleUrl: './product-search-header.component.scss'
})
export class ProductSearchHeaderComponent {
  searchTerm: string = '';
  categoryId: number | undefined;
  minPrice: number | undefined;
  maxPrice: number | undefined;
  inStock: boolean | undefined;

  constructor(
    private productService: ProductService,
    public categoryService: CategoryService
  ) {}

  onSearch(): void {
    const params: ProductSearchParams = {
      searchTerm: this.searchTerm || undefined,
      categoryId: this.categoryId,
      minPrice: this.minPrice,
      maxPrice: this.maxPrice,
      inStock: this.inStock,
    };

    this.productService.searchProducts(params);
  }

  onClear(): void {
    this.searchTerm = '';
    this.categoryId = undefined;
    this.minPrice = undefined;
    this.maxPrice = undefined;
    this.inStock = undefined;
    this.productService.searchProducts({});
  }
}
