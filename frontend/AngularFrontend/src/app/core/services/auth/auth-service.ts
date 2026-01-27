import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, of } from 'rxjs'; 
import { environment } from '../../../../environments/environment';
import { User } from '../../models/User.model';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = environment.apiUrl;

  isLoggedIn = signal<boolean>(!!localStorage.getItem('token'));

  constructor() {
    const token = localStorage.getItem('token');
    
    if (token) {
      this.getCurrentUser().subscribe({
        next: () => {
          console.log('Token is valid');
        },
        error: (err) => {
          console.error('Token expired or invalid', err);
          this.logout(); 
        }
      });
    }
  }

  login(dto: User): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/sessions`, dto).pipe(
      tap((response) => {
        localStorage.setItem('token', response.token); 
        this.isLoggedIn.set(true);
      })
    );
  }

  register(dto: User): Observable<any> {
    return this.http.post(`${this.apiUrl}/users`, dto);
  }

  logout() {
    localStorage.removeItem('token');
    this.isLoggedIn.set(false);
    this.router.navigate(['/login']);
  }

  getCurrentUser(): Observable<any> {
    return this.http.get(`${this.apiUrl}/users/me`);
  }
}