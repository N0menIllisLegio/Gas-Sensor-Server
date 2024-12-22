import { Component, inject } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { marker } from '@colsen1991/ngx-translate-extract-marker';

export interface ConfirmationDialogData {
    title: string | undefined;
    message: string;
    okButtonText: string | undefined;
    cancelButtonText: string | undefined;
}

@Component({
    selector: 'confirmation-dialog',
    templateUrl: 'confirmation.dialog.html',
    imports: [
      MatFormFieldModule,
      MatInputModule,
      FormsModule,
      ReactiveFormsModule,
      MatButtonModule,
      MatDialogContent,
      MatDialogActions,
      MatDialogTitle,
      MatDialogClose,
      TranslateModule,
    ],
})

export class ConfirmationDialogComponent {
    readonly dialogRef = inject(MatDialogRef<ConfirmationDialogComponent>);
    readonly data = inject<ConfirmationDialogData>(MAT_DIALOG_DATA);
    readonly translate = inject(TranslateService);

    public get _() {
      return marker;
    }
}