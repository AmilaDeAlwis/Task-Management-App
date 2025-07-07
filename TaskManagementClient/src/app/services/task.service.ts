import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

export interface Task {
  id?: number;
  title: string;
  description: string;
  isCompleted: boolean;
  createdDate?: string;
}

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = `${environment.apiUrl}/Tasks`;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }


  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Server returned code: ${error.status}, error message: ${error.message}`;
      if (error.error && error.error.Message) {
        errorMessage = `Server Error: ${error.error.Message}`;
      } else if (error.error && typeof error.error === 'string') {
        errorMessage = `Server Error: ${error.error}`; // For simple string errors
      }
    }
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.apiUrl, { headers: this.authService.getAuthHeaders() })
      .pipe(catchError(this.handleError));
  }

  getTask(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`, { headers: this.authService.getAuthHeaders() })
      .pipe(catchError(this.handleError));
  }

  addTask(task: Task): Observable<Task> {
    return this.http.post<Task>(this.apiUrl, task, { headers: this.authService.getAuthHeaders() })
      .pipe(catchError(this.handleError));
  }

  updateTask(task: Task): Observable<any> {
    return this.http.put(`${this.apiUrl}/${task.id}`, task, { headers: this.authService.getAuthHeaders() })
      .pipe(catchError(this.handleError));
  }

  deleteTask(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.authService.getAuthHeaders() })
      .pipe(catchError(this.handleError));
  }
}