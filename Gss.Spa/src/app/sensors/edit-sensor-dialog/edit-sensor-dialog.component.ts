
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
import DialogResultModel from '../../core/dialog-result.model';
import { Observable } from 'rxjs';
import ErrorHandlingService from '../../core/error-handling.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SensorsQueryService } from '../core/sensor-query.service';
import EditSensorModel from './edit-sensor.model';
import SensorModel from '../core/sensor.model';
import { NgSelectModule } from '@ng-select/ng-select';
import SensorTypeModel from '../../sensor-types/core/sensor-type.model';
import { SensorTypesQueryService } from '../../sensor-types/core/sensor-types-query.service';
import PagedRequestModel from '../../core/paged-request.model';

@Component({
    selector: 'edit-sensor-dialog',
    templateUrl: 'edit-sensor-dialog.component.html',
    imports: [
        MatFormFieldModule,
        MatInputModule,
        FormsModule,
        MatButtonModule,
        MatDialogModule,
        MatProgressSpinnerModule,
        NgSelectModule
    ]
})
export class EditSensorDialogComponent {
    private snackBar = inject(MatSnackBar);
    private sensorsQueryService = inject(SensorsQueryService);
    private sensorTypesQueryService = inject(SensorTypesQueryService);
    private errorHandlingService = inject(ErrorHandlingService);
    readonly dialogRef = inject(MatDialogRef<EditSensorDialogComponent>);
    readonly data = inject<SensorModel | null>(MAT_DIALOG_DATA);

    readonly name = model(this.data?.name ?? '');
    readonly description = model(this.data?.description ?? '');
    readonly type = model(this.data?.type ?? null);
    readonly isBusy = signal<boolean>(false);

    sensorTypes: SensorTypeModel[] = [];
    loading = signal<boolean>(false);
    page = 1;
    size = 20;
    fetchedAllSensorTypes = false;

    constructor() {
        this.loadItems();
    }

    onScrollToEnd() {
        this.loadItems();
    }

    loadItems() {
        if (this.fetchedAllSensorTypes)
            return;

        this.loading.set(true);

        this.sensorTypesQueryService.getSensorTypes(new PagedRequestModel(this.page, this.size))
            .subscribe(data => {
                this.sensorTypes = [...this.sensorTypes, ...data.items];
                this.fetchedAllSensorTypes = this.sensorTypes.length === data.totalItemsCount;
                this.loading.set(false);
                this.page++;
            });
    }

    onDelete(): void {
        this.isBusy.set(true);
        const request = this.sensorsQueryService.deleteSensor(this.data!.id);

        this.showErrorOrCloseDialog(request);
    }

    onCancel(): void {
        this.dialogRef.close(new DialogResultModel('cancel'));
    }

    onSave(): void {
        // TODO: detailed errors from backend and form validaiton.
        this.isBusy.set(true);
        const editSensorType = new EditSensorModel(
            this.data?.id ?? null,
            this.name(),
            this.description() === '' ? null : this.description(),
            this.type()!.id
        );

        const request = this.data === null
            ? this.sensorsQueryService.createSensor(editSensorType)
            : this.sensorsQueryService.updateSensor(editSensorType);

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
}