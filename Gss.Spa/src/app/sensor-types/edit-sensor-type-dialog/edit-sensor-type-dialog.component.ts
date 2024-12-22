
import { Component, inject, model, signal } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {
    MAT_DIALOG_DATA,
    MatDialog,
    MatDialogModule,
    MatDialogRef,
  } from '@angular/material/dialog';
import SensorTypeModel from '../core/sensor-type.model';
import EditSensorTypeModel from './edit-sensor-type.model';
import DialogResultModel from '../../core/dialog-result.model';
import { SensorTypesQueryService } from '../core/sensor-types-query.service';
import { merge, Observable } from 'rxjs';
import ErrorHandlingService from '../../core/error-handling.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmationDialogComponent } from '../../shared/confirmation-dialog/confirmation.dialog';
import dictionary from '../../core/dictionary.type';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { marker as _ } from '@colsen1991/ngx-translate-extract-marker';

@Component({
    selector: 'edit-sensor-type-dialog',
    templateUrl: 'edit-sensor-type-dialog.component.html',
    imports: [
        MatFormFieldModule,
        MatInputModule,
        FormsModule,
        MatButtonModule,
        MatDialogModule,
        MatProgressSpinnerModule,
        ReactiveFormsModule,
        TranslateModule
    ]
})
export class EditSensorTypeDialogComponent {
    private snackBar = inject(MatSnackBar);
    private sensorTypesQueryService = inject(SensorTypesQueryService);
    private errorHandlingService = inject(ErrorHandlingService);
    private translate = inject(TranslateService);
    readonly dialogRef = inject(MatDialogRef<EditSensorTypeDialogComponent>);
    readonly data = inject<SensorTypeModel | null>(MAT_DIALOG_DATA);
    readonly dialog = inject(MatDialog);

    readonly icon = new FormControl(this.data?.icon ?? '', [Validators.maxLength(8000)]);
    readonly name = new FormControl(this.data?.name ?? '', [Validators.required, Validators.maxLength(200)]);
    readonly units = new FormControl(this.data?.units ?? '', [Validators.maxLength(20)]);
    readonly errorMessage = signal<dictionary<{ message: string } | undefined>>({});

    readonly isBusy = signal<boolean>(false);

    constructor() {
        merge(
            this.icon.statusChanges, this.icon.valueChanges,
            this.name.statusChanges, this.name.valueChanges,
            this.units.statusChanges, this.units.valueChanges,
        )
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.updateErrorMessage());
    }

    updateErrorMessage() {
        let isValid = true;
        const errors: dictionary<{ message: string }> = {};

        if (this.name.hasError('required')) {
            errors['name'] = {
                message: this.translate.instant(_('common.error.required'))
            };

            isValid = false;
        }
        else if (this.name.hasError('maxlength')) {
            errors['name'] = {
                message: this.translate.instant(_('common.error.max-length'),
                {
                    maxLength: this.name.errors!['maxlength'].requiredLength
                })
            };

            isValid = false;
        }

        if (this.icon.hasError('maxlength')) {
            errors['icon'] = {
                message: this.translate.instant(_('common.error.large-icon'))
            };

            isValid = false;
        }

        if (this.units.hasError('maxlength')) {
            errors['units'] = {
                message: this.translate.instant(_('common.error.max-length'),
                {
                    maxLength: this.units.errors!['maxlength'].requiredLength
                })
            };

            isValid = false;
        }

        this.errorMessage.set(errors);

        return isValid;
    }

    onDelete(): void {
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
          panelClass: 'gss-dialog',
          disableClose: true,
          data: {
              title: this.translate.instant(_('common.dialog.confirmation-title')),
              message: this.translate.instant(_('edit-sensor-type.delete-dialog.message')),
              okButtonText: this.translate.instant(_('common.yes')),
              cancelButtonText: this.translate.instant(_('common.no'))
          },
        });

        dialogRef.afterClosed().subscribe((x) => {
            if (x) {
              this.isBusy.set(true);
              const request = this.sensorTypesQueryService.deleteSensorType(this.data!.id);

              this.showErrorOrCloseDialog(request);
            }
        });
    }

    onCancel(): void {
        this.dialogRef.close(new DialogResultModel('cancel'));
    }

    onSave(): void {
        if (!this.updateErrorMessage()) {
            return;
        }

        this.isBusy.set(true);
        const editSensorType = new EditSensorTypeModel(
            this.data?.id ?? null,
            this.icon.value === '' ? null : this.icon.value,
            this.name.value!,
            this.units.value === '' ? null : this.units.value,
        );

        const request = this.data === null
            ? this.sensorTypesQueryService.createSensorType(editSensorType)
            : this.sensorTypesQueryService.updateSensorType(editSensorType);

        this.showErrorOrCloseDialog(request);
    }

    showErrorOrCloseDialog(request: Observable<Object>) {
        request
        .subscribe({
            error: (err) => {
                const error = this.errorHandlingService.convertError(err);
                this.isBusy.set(false);

                this.snackBar.open(error.message, undefined, {
                    horizontalPosition: 'right',
                    verticalPosition: 'bottom',
                    duration: 5000,
                    panelClass: 'whitespace-pre'
                });
            },
            next: () => {
                this.dialogRef.close(new DialogResultModel('success'));
                this.isBusy.set(false);
            }
        });
    }

    encodeImageFileAsURL(event: Event) {
        if ((event.target as HTMLInputElement).files && (event.target as HTMLInputElement).files!.length) {
            const [file] = (event.target as HTMLInputElement).files!;
            var reader = new FileReader();

            reader.onloadend = () => {
                const imageBase64 = reader.result as string;

                if (imageBase64) {
                    this.icon.setValue(imageBase64);
                } else {
                    this.icon.setValue('');
                }
            }

            reader.readAsDataURL(file);
        }
    }
}