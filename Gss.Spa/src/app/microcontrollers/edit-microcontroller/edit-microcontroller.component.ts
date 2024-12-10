import { Component, effect, inject, input, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MapComponent } from '../shared/map/map.component';
import { BehaviorSubject, merge } from 'rxjs';
import FlyToTargetModel from '../shared/map/fly-to-target.model';
import { latLng, LeafletMouseEvent } from 'leaflet';
import DisplayableMicrocontrollerModel from '../shared/map/displayable-microcontroller.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { MicrocontrollersQueryService } from '../core/microcontrollers-query.service';
import { guid } from '../../core/guid';
import ErrorHandlingService from '../../core/error-handling.service';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import EditMicrocontrollerModel from './edit-microcontroller.model';
import { SensorsTableComponent } from '../../sensors/sensors-table/sensor-table.component';
import { SelectionModel } from '@angular/cdk/collections';
import { MatButtonModule } from '@angular/material/button';
import MicrocontrollerModel from '../core/microcontroller.model';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import dictionary from '../../core/dictionary.type';

@Component({
    selector: 'edit-microcontroller',
    templateUrl: 'edit-microcontroller.component.html',
    imports: [
        MatCardModule,
        MapComponent,
        MatInputModule,
        MatFormFieldModule,
        FormsModule,
        MatSlideToggleModule,
        SensorsTableComponent,
        MatButtonModule,
        MatProgressSpinnerModule,
        ReactiveFormsModule
    ]
})

export class EditMicrocontrollerComponent {
    private router = inject(Router);
    private snackBar = inject(MatSnackBar);
    private microcontrollerQueryService = inject(MicrocontrollersQueryService);
    private errorHandlingService = inject(ErrorHandlingService);

    addMicrocontroller = new BehaviorSubject<DisplayableMicrocontrollerModel | null>(null);
    flyToTarget = new BehaviorSubject<FlyToTargetModel | null>(null);
    mapZoom = 4;
    mapCenter = latLng(54.5260, 15.2551);

    isFetching = signal<boolean>(false);
    isSaving = signal<boolean>(false);
    microcontrollerId = input<guid>();
    originalMicrocontroller: MicrocontrollerModel | undefined;

    readonly name = new FormControl('', [Validators.required, Validators.maxLength(200)]);
    readonly isPublic = new FormControl(false);
    readonly lat = new FormControl('', [Validators.min(-90), Validators.max(90)]);
    readonly lng = new FormControl('', [Validators.min(-180), Validators.max(180)]);
    readonly key = new FormControl('', [Validators.required, Validators.maxLength(200)]);
    readonly selectedSensors = new SelectionModel<guid>(true, []);
    readonly errorMessage = signal<dictionary<{ message: string } | undefined>>({});

    constructor() {
        merge(
            this.name.statusChanges, this.name.valueChanges,
            this.isPublic.statusChanges, this.isPublic.valueChanges,
            this.lat.statusChanges, this.lat.valueChanges,
            this.lng.statusChanges, this.lng.valueChanges,
            this.key.statusChanges, this.key.valueChanges,
        )
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.updateErrorMessage());
    }

    updateErrorMessage() {
        let isValid = true;
        const errors: dictionary<{ message: string }> = {};

        if (this.name.hasError('required')) {
            errors['name'] = {
                message: 'You must enter a value'
            };

            isValid = false;
        }
        else if (this.name.hasError('maxlength')) {
            errors['name'] = {
                message: `Max value length: ${this.name.errors!['maxlength'].requiredLength}`
            };

            isValid = false;
        }

        if (this.lat.hasError('min')) {
            errors['lat'] = {
                message: `Min value: ${this.lat.errors!['min'].min}`
            };

            isValid = false;
        }
        else if (this.lat.hasError('max')) {
            errors['lat'] = {
                message: `Max value: ${this.lat.errors!['max'].max}`
            };

            isValid = false;
        }

        if (this.lng.hasError('min')) {
            errors['lng'] = {
                message: `Min value: ${this.lng.errors!['min'].min}`
            };

            isValid = false;
        }
        else if (this.lng.hasError('max')) {
            errors['lng'] = {
                message: `Max value: ${this.lng.errors!['max'].max}`
            };

            isValid = false;
        }

        if (this.key.hasError('required')) {
            errors['key'] = {
                message: 'You must enter a value'
            };

            isValid = false;
        }

        if (this.selectedSensors.selected.length > 5) {
            errors['sensors'] = {
                message: 'Maximum allowed sensors: 5'
            };

            isValid = false;
        }

        this.errorMessage.set(errors);

        return isValid;
    }

    ngOnInit() {
        if (this.microcontrollerId()) {
            this.isFetching.set(true);

            this.microcontrollerQueryService.getMicrocontroller(this.microcontrollerId()!)
                .subscribe({
                    next: x => {
                        this.originalMicrocontroller = x;

                        this.name.setValue(x.name);
                        this.isPublic.setValue(x.public);
                        this.lat.setValue(x.latitude?.toString() ?? '');
                        this.lng.setValue(x.longitude?.toString() ?? '');

                        this.isFetching.set(false);

                        if (x.latitude && x.longitude) {
                            this.addMicrocontroller.next(new DisplayableMicrocontrollerModel(x.latitude, x.longitude));
                            this.flyToTarget.next(new FlyToTargetModel(x.latitude, x.longitude, this.mapZoom));
                        }

                        if (x.sensors) {
                            x.sensors.map(y => this.selectedSensors.select(y.id));
                        }
                    },
                    error: (err) => {
                        this.isFetching.set(false);
                        const error = this.errorHandlingService.convertError(err);

                        this.snackBar.open(error.message, undefined, {
                            horizontalPosition: 'right',
                            verticalPosition: 'bottom',
                            duration: 5000,
                        });
                    }
                });
        }
    }

    mapDoubleClick(event: LeafletMouseEvent) {
        this.lat.setValue(event.latlng.lat.toString());
        this.lng.setValue(event.latlng.lng.toString());

        this.onLatLngChange();
    }

    onLatLngChange() {
        const lat = Number(this.lat);
        const lng = Number(this.lng);

        if (isNaN(lat) || isNaN(lng)) {
            return;
        }

        this.addMicrocontroller.next(new DisplayableMicrocontrollerModel(lat, lng));
    }

    onSave() {
        if (!this.updateErrorMessage()) {
            return;
        }

        this.isSaving.set(true);

        const newMicrocontroller = new EditMicrocontrollerModel(
            this.name.value!,
            this.isPublic.value!,
            this.lat.value ?? '',
            this.lng.value ?? '',
            this.key.value!,
            []);

        if (this.microcontrollerId()) {
            this.microcontrollerQueryService.updateMicrocontroller(
                newMicrocontroller, this.selectedSensors.selected, this.originalMicrocontroller!)
                .subscribe({
                    next: () => {
                        this.isSaving.set(false);

                        this.router.navigateByUrl(`/microcontrollers/${this.microcontrollerId()}`);
                    },
                    error: (err) => {
                        this.isSaving.set(false);
                        const error = this.errorHandlingService.convertError(err);

                        this.snackBar.open(error.message, undefined, {
                            horizontalPosition: 'right',
                            verticalPosition: 'bottom',
                            duration: 5000,
                        });
                    }
                });
        } else {
            this.microcontrollerQueryService.createMicrocontroller(newMicrocontroller, this.selectedSensors.selected)
                .subscribe({
                    next: (x) => {
                        this.isSaving.set(false);

                        this.router.navigateByUrl(`/microcontrollers/${x.id}`);
                    },
                    error: (err) => {
                        this.isSaving.set(false);
                        const error = this.errorHandlingService.convertError(err);

                        this.snackBar.open(error.message, undefined, {
                            horizontalPosition: 'right',
                            verticalPosition: 'bottom',
                            duration: 5000,
                        });
                    }
                });
        }
    }
}
