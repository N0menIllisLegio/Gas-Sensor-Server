import { Component, effect, inject, signal } from '@angular/core';
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
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { marker as _ } from '@colsen1991/ngx-translate-extract-marker';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { MaterialPaginatorIntl } from './core/material-paginator-internalization.service';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';

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
    MatMenuModule,
    TranslateModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule
  ],
  providers: [{provide: MatPaginatorIntl, useClass: MaterialPaginatorIntl}],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  readonly authService = inject(AuthService);
  readonly dialog = inject(MatDialog);
  readonly screenSizeWatcher = inject(ScreenSizeWatcherService);

  currentLocale = signal('en');

  public get firstName() : string | undefined {
    return this.authService.firstName;
  }

  constructor(private translate: TranslateService) {
    this.translate.addLangs(['ru', 'en']);
    this.translate.setDefaultLang('en');
    this.translate.use('en');

    effect(() => {
      this.translate.use(this.currentLocale());
    })
  }

  onLogout() {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      panelClass: 'gss-dialog',
      disableClose: true,
      data: {
        title: this.translate.instant(_('common.dialog.confirmation-title')),
        message: this.translate.instant(_('dialog.log-out.message')),
        okButtonText: this.translate.instant(_('common.yes')),
        cancelButtonText: this.translate.instant(_('common.no')),
      },
    });

    dialogRef.afterClosed().subscribe((x) => {
        if (x) {
          this.authService.logout();
        }
    });
  }

  changeLanguage(locale: string) {
    this.currentLocale.set(locale);
  }
}
