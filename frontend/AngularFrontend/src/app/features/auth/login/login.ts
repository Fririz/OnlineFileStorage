import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth/auth-service';
import { User } from '../../../core/models/User.model';
import { Router } from '@angular/router';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card'; 
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-login',
  imports: [CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly _authService: AuthService;
  private readonly _router: Router;
  private readonly _cdr: ChangeDetectorRef

  constructor(authService: AuthService, router: Router, cdr: ChangeDetectorRef) {
    this._authService = authService;
    this._router = router;
    this._cdr = cdr;
  }
  hidePassword = true;
  errorMessage: string = '';
  loginForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

onSubmit() {
  this.errorMessage = '';
  if (this.loginForm.valid) {
    let userDto: User = {
      Username: this.loginForm.value.username!,
      Password: this.loginForm.value.password!
    };

    this._authService.login(userDto).subscribe({
      next: (response) => {
        this._router.navigate(['/items']);
      },
      error: (err) => {
        console.error('Login error:', err);

        if (err.error && err.error.errors) {
          const validationMessages = Object.values(err.error.errors).flat() as string[];
          
          this.errorMessage = validationMessages.join('. ');
        } 
        else if (err.error && err.error.title) {
          this.errorMessage = err.error.title;
        }
        else {
          this.errorMessage = 'Login failed. Please check your credentials.';
        }
        this._cdr.detectChanges();
      }
    });
  }
}
}
