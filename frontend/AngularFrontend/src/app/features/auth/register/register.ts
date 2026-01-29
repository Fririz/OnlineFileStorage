import { Component, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card'; 
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../core/services/auth/auth-service';
import { User } from '../../../core/models/User.model';

@Component({
  selector: 'app-register',
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    MatInputModule,      
    MatFormFieldModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  hidePassword = true;
  errorMessage: string = '';

  registerForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  onSubmit() {
    this.errorMessage = '';

    if (this.registerForm.valid) {
      let userDto: User = {
        Username: this.registerForm.value.username!,
        Password: this.registerForm.value.password!
      };

      this.authService.register(userDto).subscribe({
        next: (response) => {
          this.router.navigate(['/login']);
        },
        error: (err) => {
          if (err.error && err.error.errors) {
            const validationMessages = Object.values(err.error.errors).flat() as string[];
            this.errorMessage = validationMessages.join('. ');
          } 
          else if (err.error && err.error.title) {
            this.errorMessage = err.error.title;
          }
          else {
            this.errorMessage = 'Registration failed. Please try again.';
          }
          this.cdr.detectChanges();
        }
      });
    }
  }
}