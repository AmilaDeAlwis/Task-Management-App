// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Tasks`;
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) { }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  private hasToken(): boolean {
    // Check if a token (username:password base64) exists in session storage
    return !!sessionStorage.getItem('basicAuthToken');
  }

  login(username: string, password: string): Observable<any> {
    const authString = btoa(`${username}:${password}`);
    const headers = new HttpHeaders({
      'Authorization': `Basic ${authString}`
    });

    // Make a request to a protected endpoint to verify credentials
    return this.http.get(this.apiUrl, { headers: headers, observe: 'response' })
      .pipe(
        tap(response => {
          if (response.status === 200) {
            sessionStorage.setItem('basicAuthToken', authString);
            this.loggedIn.next(true);
            console.log('Login successful');
          }
        }),
        catchError(error => {
          this.logout();
          console.error('Login failed:', error);
          return throwError(() => new Error('Login failed. Please check your credentials.'));
        })
      );
  }

  logout(): void {
    sessionStorage.removeItem('basicAuthToken');
    this.loggedIn.next(false);
    console.log('Logged out');
  }

  getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('basicAuthToken');
    if (token) {
      return new HttpHeaders({
        'Authorization': `Basic ${token}`
      });
    }
    return new HttpHeaders();
  }
}