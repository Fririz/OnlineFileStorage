import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import{ AuthService } from '../../core/services/auth/auth-service';
import { inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
@Component({
  selector: 'app-navbar',
  imports: [CommonModule, MatToolbarModule, MatIconModule, MatButtonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
public authService = inject(AuthService);
  logout(){
    this.authService.logout();
  }
}
