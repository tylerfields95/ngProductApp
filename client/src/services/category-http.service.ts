import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../models';

@Injectable({
  providedIn: 'root',
})
export class CategoryHttpService {
  private apiUrl = 'http://localhost:5258/api/category';

  constructor(private http: HttpClient) {}

  /**
   * Get all categories
   */
  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  /**
   * Get category by ID
   */
  getById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new category
   */
  create(category: Category): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, category);
  }
}
