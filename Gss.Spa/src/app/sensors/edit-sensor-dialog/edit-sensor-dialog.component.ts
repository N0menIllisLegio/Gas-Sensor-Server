
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {
    MAT_DIALOG_DATA,
    MatDialog,
    MatDialogModule,
    MatDialogRef,
  } from '@angular/material/dialog';
import DialogResultModel from '../../core/dialog-result.model';
import { merge, Observable } from 'rxjs';
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
import { ConfirmationDialogComponent } from '../../shared/confirmation-dialog/confirmation.dialog';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import dictionary from '../../core/dictionary.type';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { marker as _ } from '@colsen1991/ngx-translate-extract-marker';

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
        NgSelectModule,
        ReactiveFormsModule,
        TranslateModule,
    ]
})
export class EditSensorDialogComponent {
    private snackBar = inject(MatSnackBar);
    private sensorsQueryService = inject(SensorsQueryService);
    private sensorTypesQueryService = inject(SensorTypesQueryService);
    private errorHandlingService = inject(ErrorHandlingService);
    readonly dialogRef = inject(MatDialogRef<EditSensorDialogComponent>);
    readonly data = inject<SensorModel | null>(MAT_DIALOG_DATA);
    readonly dialog = inject(MatDialog);

    sensorTypes: SensorTypeModel[] = [];
    loading = signal<boolean>(false);
    page = 1;
    size = 20;
    fetchedAllSensorTypes = false;

    readonly form = new FormGroup({
        type: new FormControl(this.data?.type ?? null, [Validators.required]),
        name: new FormControl(this.data?.name ?? '', [Validators.required, Validators.maxLength(200)]),
        description: new FormControl(this.data?.description ?? '', [Validators.maxLength(1800)]),
    });

    readonly errorMessage = signal<dictionary<{ message: string } | undefined>>({});
    readonly isBusy = signal<boolean>(false);

    constructor(private translate: TranslateService) {
        merge(
            this.form.controls.name.statusChanges, this.form.controls.name.valueChanges,
            this.form.controls.description.statusChanges, this.form.controls.description.valueChanges,
            this.form.controls.type.statusChanges, this.form.controls.type.valueChanges
        )
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.updateErrorMessage());

        this.loadItems();
    }

    updateErrorMessage() {
        let isValid = true;
        const errors: dictionary<{ message: string }> = {};

        if (this.form.controls.name.hasError('required')) {
            errors['name'] = {
                message: this.translate.instant(_('common.error.required'))
            };

            isValid = false;
        }
        else if (this.form.controls.name.hasError('maxlength')) {
            errors['name'] = {
                message: this.translate.instant(_('common.error.max-length'),
                    {
                        maxLength: this.form.controls.name.errors!['maxlength'].requiredLength
                    })
            };

            isValid = false;
        }

        if (this.form.controls.description.hasError('maxlength')) {
            errors['description'] = {
                message: this.translate.instant(_('common.error.max-length'),
                    {
                        maxLength: this.form.controls.description.errors!['maxlength'].requiredLength
                    })
            };

            isValid = false;
        }

        if (this.form.controls.type.hasError('required')) {
            errors['type'] = {
                message: this.translate.instant(_('common.error.required'))
            };

            isValid = false;
        }

        this.errorMessage.set(errors);

        return isValid;
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
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
          panelClass: 'gss-dialog',
          disableClose: true,
          data: {
              title: this.translate.instant(_('common.dialog.confirmation-title')),
              message: this.translate.instant(_('edit-sensor.delete-dialog.message')),
              okButtonText: this.translate.instant(_('common.yes')),
              cancelButtonText: this.translate.instant(_('common.no')),
          },
        });

        dialogRef.afterClosed().subscribe((x) => {
            if (x) {
                this.isBusy.set(true);
                const request = this.sensorsQueryService.deleteSensor(this.data!.id);

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
        const editSensorType = new EditSensorModel(
            this.data?.id ?? null,
            this.form.controls.name.value!,
            this.form.controls.description.value === '' ? null : this.form.controls.description.value,
            this.form.controls.type.value!.id
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