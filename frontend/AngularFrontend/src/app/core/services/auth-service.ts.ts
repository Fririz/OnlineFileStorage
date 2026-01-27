import { Injectable } from '@angular/core';
import { User } from '../models/User.model';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})

export class AuthService {
  constructor(private http: HttpClient) {}
  login(dto: User): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/login`, dto);
  }

  register(dto: User): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/register`, dto);
  }
  
  isLoggedIn(): boolean {
    this.http.get(`${environment.apiUrl}/auth/getcurrentuser`).subscribe({
      next: (response) => {
        console.log('User is logged in', response);
      },
      error: (err) => {
        console.error('User is not logged in', err);
        return false;
      }
    });
    return true;
  }
}