
import { Component, inject, model, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {
    MAT_DIALOG_DATA,
    MatDialogModule,
    MatDialogRef,
  } from '@angular/material/dialog';
import SensorTypeModel from '../core/sensor-type.model';
import EditSensorTypeModel from './edit-sensor-type.model';
import DialogResultModel from '../../core/dialog-result.model';
import { SensorTypesQueryService } from '../core/sensor-types-query.service';
import { Observable } from 'rxjs';
import ErrorHandlingService from '../../core/error-handling.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
    selector: 'edit-sensor-type-dialog',
    templateUrl: 'edit-sensor-type-dialog.component.html',
    imports: [
        MatFormFieldModule,
        MatInputModule,
        FormsModule,
        MatButtonModule,
        MatDialogModule,
        MatProgressSpinnerModule
    ]
})
export class EditSensorTypeDialogComponent {
    private snackBar = inject(MatSnackBar);
    private sensorTypesQueryService = inject(SensorTypesQueryService);
    private errorHandlingService = inject(ErrorHandlingService);
    readonly dialogRef = inject(MatDialogRef<EditSensorTypeDialogComponent>);
    readonly data = inject<SensorTypeModel | null>(MAT_DIALOG_DATA);

    readonly units = model(this.data?.units ?? '');
    readonly name = model(this.data?.name ?? '');
    readonly icon = model(this.data?.icon ?? '');
    readonly isBusy = signal<boolean>(false);

    onDelete(): void {
        this.isBusy.set(true);
        const request = this.sensorTypesQueryService.deleteSensorType(this.data!.id);

        this.showErrorOrCloseDialog(request);
    }

    onCancel(): void {
        this.dialogRef.close(new DialogResultModel('cancel'));
    }

    onSave(): void {
        // TODO: detailed errors from backend and form validaiton.
        this.isBusy.set(true);
        const editSensorType = new EditSensorTypeModel(
            this.data?.id ?? null,
            this.icon() === '' ? null : this.icon(),
            this.name(),
            this.units() === '' ? null : this.units(),
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
                    this.icon.set(imageBase64);
                } else {
                    this.icon.set('');
                }
            }

            reader.readAsDataURL(file);
        }
    }
}