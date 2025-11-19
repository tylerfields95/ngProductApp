import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../../services/product.service';
import { ProductSearchHeaderComponent } from '../product-search-header/product-search-header.component';
import { ProductPaginationFooterComponent } from '../product-pagination-footer/product-pagination-footer.component';
import { CategoryDetailModalComponent } from '../category-detail-modal/category-detail-modal.component';
import { Category } from '../../../models';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule, ProductSearchHeaderComponent, ProductPaginationFooterComponent, CategoryDetailModalComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent {
  selectedCategory: Category | null = null;
  isModalOpen: boolean = false;

  constructor(public productService: ProductService) {}

  onPageChange(page: number): void {
    this.productService.searchProducts({ page });
  }

  onCategoryClick(category: Category | undefined): void {
    if (category) {
      this.selectedCategory = category;
      this.isModalOpen = true;
    }
  }

  onModalClose(): void {
    this.isModalOpen = false;
    this.selectedCategory = null;
  }
}
