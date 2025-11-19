import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Category } from '../../../models';

@Component({
  selector: 'app-category-detail-modal',
  imports: [CommonModule],
  templateUrl: './category-detail-modal.component.html',
  styleUrl: './category-detail-modal.component.scss'
})
export class CategoryDetailModalComponent {
  @Input() category: Category | null = null;
  @Input() isOpen: boolean = false;
  @Output() close = new EventEmitter<void>();

  onClose(): void {
    this.close.emit();
  }

  onBackdropClick(event: MouseEvent): void {
    // Only close if clicking the backdrop itself, not the modal content
    if (event.target === event.currentTarget) {
      this.onClose();
    }
  }
}
