import { Component, effect, inject, input, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MapComponent } from '../shared/map/map.component';
import { BehaviorSubject } from 'rxjs';
import FlyToTargetModel from '../shared/map/fly-to-target.model';
import { latLng, LeafletMouseEvent } from 'leaflet';
import DisplayableMicrocontrollerModel from '../shared/map/displayable-microcontroller.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { MicrocontrollersQueryService } from '../core/microcontrollers-query.service';
import { SensorsQueryService } from '../../sensors/core/sensor-query.service';
import { guid } from '../../core/guid';
import ErrorHandlingService from '../../core/error-handling.service';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import EditMicrocontrollerModel from './edit-microcontroller.model';
import { SensorsTableComponent } from '../../sensors/sensors-table/sensor-table.component';
import { SelectionModel } from '@angular/cdk/collections';
import { MatButtonModule } from '@angular/material/button';
import MicrocontrollerModel from '../core/microcontroller.model';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

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
        MatProgressSpinnerModule
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
    editingMicrocontroller = signal<EditMicrocontrollerModel>(
        new EditMicrocontrollerModel('', false, '', '', '', []));

    selectedSensors = new SelectionModel<guid>(true, []);

    ngOnInit() {
        if (this.microcontrollerId()) {
            this.isFetching.set(true);

            this.microcontrollerQueryService.getMicrocontroller(this.microcontrollerId()!)
                .subscribe({
                    next: x => {
                        this.originalMicrocontroller = x;

                        this.editingMicrocontroller.set(new EditMicrocontrollerModel(
                            x.name,
                            x.public,
                            x.latitude?.toString() ?? '',
                            x.longitude?.toString() ?? '',
                            '',
                            x.sensors
                        ));

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
        this.editingMicrocontroller().latitude = event.latlng.lat.toString();
        this.editingMicrocontroller().longitude = event.latlng.lng.toString();

        this.onLatLngChange();
    }

    onLatLngChange() {
        const lat = Number(this.editingMicrocontroller().latitude);
        const lng = Number(this.editingMicrocontroller().longitude);

        if (isNaN(lat) || isNaN(lng)) {
            return;
        }

        this.addMicrocontroller.next(new DisplayableMicrocontrollerModel(lat, lng));
    }

    onSave() {
        if (this.selectedSensors.selected.length > 5) {
            this.snackBar.open('We support no more then 5 sensors per microcontroller', undefined, {
                horizontalPosition: 'right',
                verticalPosition: 'bottom',
                duration: 5000,
            });

            return;
        }


        this.isSaving.set(true);

        if (this.microcontrollerId()) {
            this.microcontrollerQueryService.updateMicrocontroller(
                this.editingMicrocontroller(),
                this.selectedSensors.selected,
                this.originalMicrocontroller!)
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
            this.microcontrollerQueryService.createMicrocontroller(
                this.editingMicrocontroller(), this.selectedSensors.selected)
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
