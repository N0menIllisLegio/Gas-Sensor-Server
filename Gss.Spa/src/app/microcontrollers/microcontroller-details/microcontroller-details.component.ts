import { Component, computed, inject, input, signal } from "@angular/core";
import { MicrocontrollersQueryService } from "../core/microcontrollers-query.service";
import { guid } from "../../core/guid";
import { Router } from "@angular/router";
import { BehaviorSubject, catchError, of } from "rxjs";
import MicrocontrollerModel from "../core/microcontroller.model";
import { MapComponent } from "../shared/map/map.component";
import DisplayableMicrocontrollerModel from "../shared/map/displayable-microcontroller.model";
import FlyToTargetModel from "../shared/map/fly-to-target.model";
import { MatCardModule } from "@angular/material/card";
import { CoordinatesPipe } from "../../shared/coordinates.pipe";
import { EmptyPlaceholderPipe } from "../../shared/empty-placeholder.pipe";
import { DateTimePipe } from "../../shared/date-time.pipe";
import { SpinnerComponent } from "../../shared/spinner/spinner.component";
import { BoolYesNoPipe } from "../../shared/bool-yes-no.pipe";
import { MatExpansionModule } from "@angular/material/expansion";
import { DataChartComponent } from "../shared/data-chart/data-chart.component";
import { MatDivider } from "@angular/material/divider";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import AuthService from "../../core/auth.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatInputModule } from "@angular/material/input";
import { MatFormFieldModule } from "@angular/material/form-field";
import { FormsModule } from "@angular/forms";
import MicrocontrollerSensorModel from "../core/microcontroller-sensor.model";
import { MatTooltipModule } from "@angular/material/tooltip";

@Component({
    selector: 'microcontroller-details',
    templateUrl: './microcontroller-details.component.html',
    imports: [
        MapComponent,
        MatCardModule,
        CoordinatesPipe,
        EmptyPlaceholderPipe,
        DateTimePipe,
        SpinnerComponent,
        BoolYesNoPipe,
        MatExpansionModule,
        DataChartComponent,
        MatDivider,
        MatButtonModule,
        MatIconModule,
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatTooltipModule
    ]
})
export class MicrocontrollerDetailsComponent {
    private snackBar = inject(MatSnackBar);
    private microcontrollerQueryService = inject(MicrocontrollersQueryService);
    private router = inject(Router);
    authService = inject(AuthService);

    loading = signal(true);
    microcontrollerId = input<guid>();
    microcontroller = signal<MicrocontrollerModel | undefined> (undefined);
    requestedSensorId = signal<guid | null> (null);

    addMicrocontroller = new BehaviorSubject<DisplayableMicrocontrollerModel | null>(null);
    flyToTarget = new BehaviorSubject<FlyToTargetModel | null>(null);

    ngOnInit() {
        const guidCheckRegex = new RegExp(/^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i);

        if (!this.microcontrollerId() || !guidCheckRegex.test(this.microcontrollerId()!)) {
            this.router.navigateByUrl('/not-found');
            return;
        }

        this.microcontrollerQueryService
            .getMicrocontroller(this.microcontrollerId()!)
            .pipe(catchError((err) => {
                if (err.status === 404)
                    this.router.navigateByUrl('/not-found');

                return of(null);
            }))
            .subscribe(data => {
                if (!data)
                    return;

                data.sensors.forEach(x => x.enteredCriticalValue = x.criticalValue?.toString());

                this.loading.set(false);
                this.microcontroller.set(data);
                this.requestedSensorId.set(data.requestedSensorId);

                if (data.latitude === null || data.longitude === null)
                    return;

                this.addMicrocontroller.next(new DisplayableMicrocontrollerModel(data.latitude, data.longitude));
                this.flyToTarget.next(new FlyToTargetModel(data.latitude, data.longitude, 8));
            });
    }

    onDelete() {
        this.microcontrollerQueryService.deleteMicrocontroller(this.microcontrollerId()!)
            .subscribe(() => this.router.navigateByUrl('/'));
    }

    onEdit() {
        this.router.navigateByUrl(`/microcontrollers/${this.microcontrollerId()!}/edit`)
    }

    onSync(event: Event, microcontrollerSensorId: guid) {
        event.stopPropagation();

        this.microcontrollerQueryService.requestSensorValue(this.microcontrollerId()!, microcontrollerSensorId)
            .subscribe(() => {
                this.requestedSensorId.set(microcontrollerSensorId);

                this.snackBar.open('Sensor\'s data requested successfully!', undefined, {
                    horizontalPosition: 'right',
                    verticalPosition: 'bottom',
                    duration: 5000,
                });
            });
    }

    onSetCriticalValue(sensor: MicrocontrollerSensorModel) {
        const enteredValue = Number(sensor.enteredCriticalValue);

        if (isNaN(enteredValue)) {
            this.snackBar.open('Entered value is not a number!', undefined, {
                horizontalPosition: 'right',
                verticalPosition: 'bottom',
                duration: 5000,
            });

            return;
        }

        this.microcontrollerQueryService.setTreshold(sensor.microcontrollerSensorId, sensor.enteredCriticalValue === '' ? null : enteredValue)
            .subscribe(() => {
                this.snackBar.open('Critical value updated successfuly!', undefined, {
                    horizontalPosition: 'right',
                    verticalPosition: 'bottom',
                    duration: 5000,
                });
            });
    }
}