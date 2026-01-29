import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { Navbar } from './layout/navbar/navbar';
import { Sidebar } from './layout/sidebar/sidebar';
import { AuthService } from './core/services/auth/auth-service';
@Component({
  selector: 'app-root',
  imports: [Navbar, Sidebar, MatSidenavModule, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  public readonly authService :AuthService;
  constructor(authService: AuthService) {
    this.authService = authService;
  }
  protected readonly title = signal('AngularFrontend');
}
