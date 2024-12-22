import { Component, inject, input, signal } from "@angular/core";
import { MicrocontrollersQueryService } from "../core/microcontrollers-query.service";
import { checkGuid, guid } from "../../core/guid";
import { Router } from "@angular/router";
import { BehaviorSubject, catchError, of } from "rxjs";
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
import { MatInputModule } from "@angular/material/input";
import { MatFormFieldModule } from "@angular/material/form-field";
import { FormsModule } from "@angular/forms";
import MicrocontrollerSensorModel from "../core/microcontroller-sensor.model";
import { MatTooltipModule } from "@angular/material/tooltip";
import { EditThresholdDialog } from "./edit-threshold-dialog/edit-threshold.dialog";
import { MatDialog } from "@angular/material/dialog";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { ConfirmationDialogComponent } from "../../shared/confirmation-dialog/confirmation.dialog";
import ExtendedMicrocontrollerModel from "../core/extended-microcontroller.model";
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { marker as _ } from '@colsen1991/ngx-translate-extract-marker';

@Component({
    selector: 'microcontroller-details',
    templateUrl: './microcontroller-details.component.html',
    imports: [
        MapComponent,
        MatCardModule,
        CoordinatesPipe,
        EmptyPlaceholderPipe,
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
        MatTooltipModule,
        MatProgressSpinnerModule,
        DateTimePipe,
        TranslateModule
    ]
})
export class MicrocontrollerDetailsComponent {
    private translate = inject(TranslateService);
    private microcontrollerQueryService = inject(MicrocontrollersQueryService);
    private router = inject(Router);
    readonly dialog = inject(MatDialog);
    authService = inject(AuthService);

    loading = signal(true);
    microcontrollerId = input.required<guid>();
    microcontroller = signal<ExtendedMicrocontrollerModel | undefined> (undefined);
    requestedSensorId = signal<guid | null> (null);

    addMicrocontroller = new BehaviorSubject<DisplayableMicrocontrollerModel | null>(null);
    flyToTarget = new BehaviorSubject<FlyToTargetModel | null>(null);

    ngOnInit() {
        if (!checkGuid(this.microcontrollerId()!)) {
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
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            panelClass: 'gss-dialog',
            disableClose: true,
            data: {
                title: this.translate.instant(_('common.dialog.confirmation-title')),
                message: this.translate.instant(_('microcontroller-details.delete-dialog.message')),
                okButtonText: this.translate.instant(_('common.yes')),
                cancelButtonText: this.translate.instant(_('common.no'))
            },
        });

        dialogRef.afterClosed().subscribe((x) => {
            if (x) {
                this.microcontrollerQueryService.deleteMicrocontroller(this.microcontrollerId()!)
                    .subscribe(() => this.router.navigateByUrl('/'));
            }
        });
    }

    onEdit() {
        this.router.navigateByUrl(`/microcontrollers/${this.microcontrollerId()!}/edit`)
    }

    onSetCriticalValue(sensor: MicrocontrollerSensorModel) {
        const dialogRef = this.dialog.open(EditThresholdDialog, {
            panelClass: 'gss-dialog',
            data: {
                microcontrollerSensorId: sensor.microcontrollerSensorId,
                criticalValue: sensor.criticalValue
            },
        });

        dialogRef.afterClosed().subscribe((changedValue) => {
            if (changedValue) {
                sensor.criticalValue = changedValue.criticalValue;
            }
        });
    }

    toConfigGenerator() {
        this.router.navigateByUrl(`/configuration-generator/${this.microcontrollerId()}`);
    }
}
