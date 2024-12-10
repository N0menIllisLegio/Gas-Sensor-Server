import { Component, inject, model, signal } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
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
    ],
  })
export class EditThresholdDialog {
  private microcontrollerQueryService = inject(MicrocontrollersQueryService);
  readonly dialogRef = inject(MatDialogRef<EditThresholdDialog>);
  readonly data = inject<EditThresholdDialogData>(MAT_DIALOG_DATA);
  private errorHandlingService = inject(ErrorHandlingService);

  readonly threshold = new FormControl(this.data.criticalValue?.toString() ?? '', [Validators.min(-10000), Validators.max(10000)]);
  readonly errorMessage = signal<string>('');

  constructor() {
    merge(this.threshold.statusChanges, this.threshold.valueChanges)
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.updateErrorMessage());
  }

  updateErrorMessage() {
    if (this.threshold.hasError('min')) {
      this.errorMessage.set(`Min value: ${this.threshold.errors!['min'].min}`);
    }
    else if (this.threshold.hasError('max')) {
      this.errorMessage.set(`Max value: ${this.threshold.errors!['max'].max}`);
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

    this.microcontrollerQueryService
      .setTreshold(this.data.microcontrollerSensorId, this.threshold.value === '' ? null : enteredValue)
      .subscribe({
        next: () => {
          this.dialogRef.close({
            criticalValue: this.threshold.value
          });
        },
        error: (err) => {
          const error = this.errorHandlingService.convertError(err);

          this.threshold.setErrors({
            server: error.message
          });
        }
      });
  }
}