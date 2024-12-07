import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import AuthService from './core/auth.service';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatButtonModule,
    MatListModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  authService = inject(AuthService);
  publicNavbarLinks = [
    {
      url: '/map',
      title: 'Map'
    },
    {
      url: '/configuration-generator',
      title: 'Config Generator'
    },
  ];

  authorizedNavbarLinks = [
    {
      url: '/sensor-types',
      title: 'Sensor Types'
    },
  ];
}
