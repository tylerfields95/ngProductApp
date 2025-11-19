import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaginatedResult } from '../../../models';

@Component({
  selector: 'app-product-pagination-footer',
  imports: [CommonModule],
  templateUrl: './product-pagination-footer.component.html',
  styleUrl: './product-pagination-footer.component.scss'
})
export class ProductPaginationFooterComponent {
  @Input() result: PaginatedResult<any> | null = null;
  @Output() pageChange = new EventEmitter<number>();

  // Expose Math for template usage
  Math = Math;

  onPageChange(page: number): void {
    if (this.result && page >= 1 && page <= this.result.totalPages) {
      this.pageChange.emit(page);
    }
  }

  getPageNumbers(): number[] {
    if (!this.result) return [];

    const currentPage = this.result.page;
    const totalPages = this.result.totalPages;
    const pages: number[] = [];

    // Show max 7 page numbers with current page in middle when possible
    const maxVisible = 7;
    let startPage = Math.max(1, currentPage - Math.floor(maxVisible / 2));
    let endPage = Math.min(totalPages, startPage + maxVisible - 1);

    // Adjust start if we're near the end
    if (endPage - startPage < maxVisible - 1) {
      startPage = Math.max(1, endPage - maxVisible + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }
}
