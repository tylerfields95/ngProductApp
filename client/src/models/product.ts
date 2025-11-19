import { Category } from './category';

export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  category?: Category;
  stockQuantity: number;
  createdDate: Date;
  isActive: boolean;
}
