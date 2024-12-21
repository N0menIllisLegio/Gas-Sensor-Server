import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import AuthService from './core/auth.service';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from './shared/confirmation-dialog/confirmation.dialog';
import { GssTitleComponent } from "./shared/gss-title/gss-title.component";
import { MatMenuModule } from '@angular/material/menu';
import { ScreenSizeWatcherService } from './core/screen-size-watcher.service';

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
    MatDividerModule,
    GssTitleComponent,
    GssTitleComponent,
    MatMenuModule
],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  readonly authService = inject(AuthService);
  readonly dialog = inject(MatDialog);
  readonly screenSizeWatcher = inject(ScreenSizeWatcherService);

  onLogout() {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      panelClass: 'w-2/5',
      disableClose: true,
      data: {
          title: 'Confirmation Dialog',
          message: 'Are you sure you want to logout?',
          okButtonText: 'Yes',
          cancelButtonText: 'No'
      },
    });

    dialogRef.afterClosed().subscribe((x) => {
        if (x) {
          this.authService.logout();
        }
    });
  }
}
