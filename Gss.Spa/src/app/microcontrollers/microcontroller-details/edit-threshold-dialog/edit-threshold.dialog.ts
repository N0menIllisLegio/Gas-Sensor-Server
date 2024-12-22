import { Component, inject, signal } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { guid } from '../../../core/guid';
import { MicrocontrollersQueryService } from '../../core/microcontrollers-query.service';
import { merge } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import ErrorHandlingService from '../../../core/error-handling.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { marker as _ } from '@colsen1991/ngx-translate-extract-marker';

export interface EditThresholdDialogData {
    microcontrollerSensorId: guid;
    criticalValue: number | null;
}

@Component({
    selector: 'edit-threshold',
    templateUrl: 'edit-threshold.dialog.html',
    imports: [
      MatFormFieldModule,
      MatInputModule,
      FormsModule,
      ReactiveFormsModule,
      MatButtonModule,
      MatDialogContent,
      MatDialogActions,
      MatProgressSpinnerModule,
      TranslateModule
    ],
  })
export class EditThresholdDialog {
  private translate = inject(TranslateService);
  private microcontrollerQueryService = inject(MicrocontrollersQueryService);
  readonly dialogRef = inject(MatDialogRef<EditThresholdDialog>);
  readonly data = inject<EditThresholdDialogData>(MAT_DIALOG_DATA);
  private errorHandlingService = inject(ErrorHandlingService);

  readonly threshold = new FormControl(this.data.criticalValue?.toString() ?? '', [Validators.min(-10000), Validators.max(10000)]);
  readonly errorMessage = signal<string>('');
  readonly isBusy = signal<boolean>(false);

  constructor() {
    merge(this.threshold.statusChanges, this.threshold.valueChanges)
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.updateErrorMessage());
  }

  updateErrorMessage() {
    if (this.threshold.hasError('min')) {
      this.errorMessage.set(this.translate.instant(
        _('common.error.min-value'), { minValue: this.threshold.errors!['min'].min }));
    }
    else if (this.threshold.hasError('max')) {
      this.errorMessage.set(this.translate.instant(
        _('common.error.max-value'), { maxValue: this.threshold.errors!['max'].max }));
    } else if (this.threshold.hasError('server')) {
      this.errorMessage.set(this.threshold.errors!['server']);
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  save() {
    if (this.threshold.invalid) {
      return;
    }

    const enteredValue = Number(this.threshold.value);

    if (isNaN(enteredValue)) {
        return;
    }

    this.isBusy.set(true);

    this.microcontrollerQueryService
      .setTreshold(this.data.microcontrollerSensorId, this.threshold.value === '' ? null : enteredValue)
      .subscribe({
        next: () => {
          this.isBusy.set(false);
          this.dialogRef.close({
            criticalValue: this.threshold.value
          });
        },
        error: (err) => {
          const error = this.errorHandlingService.convertError(err);

          this.threshold.setErrors({
            server: error.message
          });

          this.isBusy.set(false);
        }
      });
  }
}