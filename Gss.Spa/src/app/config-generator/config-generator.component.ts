import { Component, ElementRef, inject, input, signal, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { provideNativeDateAdapter } from '@angular/material/core';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatListModule } from '@angular/material/list';
import { checkGuid, guid } from '../core/guid';
import { MicrocontrollersQueryService } from '../microcontrollers/core/microcontrollers-query.service';
import MicrocontrollerSensorModel from '../microcontrollers/core/microcontroller-sensor.model';
import { SpinnerComponent } from "../shared/spinner/spinner.component";

@Component({
    selector: 'config-generator',
    templateUrl: 'config-generator.component.html',
    providers: [provideNativeDateAdapter()],
    imports: [
        MatInputModule,
        MatFormFieldModule,
        MatCardModule,
        MatButtonModule,
        MatIconModule,
        MatDatepickerModule,
        MatTimepickerModule,
        FormsModule,
        MatSelectModule,
        MatListModule,
        MatAutocompleteModule,
        SpinnerComponent
    ]
})

export class ConfigGeneratorComponent {
    private snackBar = inject(MatSnackBar);
    private configTextElement = viewChild<ElementRef<HTMLDivElement>>('configurationText');
    private microcontrollerQueryService = inject(MicrocontrollersQueryService);

    config = {
        initDateTime: new Date(),
        ssid: '',
        wifiPassword: '',
        wifiPrivacyMode: '2',
        serverAddress: '',
        serverPort: '',
        serverCommunicationProtocol: 't',
        sdWritePeriodSeconds: '60',
        transmitPeriodSeconds: '1800',
        microcontrollerId: '',
        microcontrollerKey: '',
        microcontrollerSensors: [] as guid[],
    };

    loadingMicrocontroller = signal(false);
    microcontrollerId = input<guid>();
    avialableMicrocontrollerSensors = signal<MicrocontrollerSensorModel[]>([]);

    getAvialableMicrocontrollerSensor(microcontrollerSensorId: guid) {
        return this.avialableMicrocontrollerSensors().find(x => x.microcontrollerSensorId === microcontrollerSensorId);
    }

    input = viewChild<ElementRef<HTMLInputElement>>('inputSensorId');
    options: MicrocontrollerSensorModel[] = [];
    filteredOptions: MicrocontrollerSensorModel[] =  this.options.slice();
    selectedMicrocontrollerSensorId: MicrocontrollerSensorModel | guid | undefined;

    filter(): void {
        const filterValue = this.input()!.nativeElement.value.toLowerCase();
        this.filteredOptions = this.options.filter(o =>
            o.microcontrollerSensorId.toLowerCase().includes(filterValue) ||
            o.name.toLowerCase().includes(filterValue));
    }

    displayFn(model: MicrocontrollerSensorModel): guid {
        return model ? model.microcontrollerSensorId : '';
    }

    ngOnInit() {
        if (this.microcontrollerId() && checkGuid(this.microcontrollerId()!)) {
            this.loadingMicrocontroller.set(true);
            this.microcontrollerQueryService.getMicrocontroller(this.microcontrollerId()!)
                .subscribe({
                    next: x => {
                        this.loadingMicrocontroller.set(false);
                        this.config.microcontrollerId = x.id;
                        this.config.microcontrollerSensors = x.sensors.map(y => y.microcontrollerSensorId);
                        this.avialableMicrocontrollerSensors.set(x.sensors);

                        this.options = x.sensors;
                        this.filteredOptions = this.options.slice();
                    },
                    error: () => {
                        this.loadingMicrocontroller.set(false);
                        this.snackBar.open('Failed to get microcontrollers data!', undefined, {
                            verticalPosition: 'bottom',
                            horizontalPosition: 'right',
                            duration: 3000,
                            panelClass: 'whitespace-pre'
                        })
                    }
                });
        }
    }

    onCopy() {
        if (!navigator.clipboard || !this.configTextElement()?.nativeElement) {
            return;
        }

        navigator.clipboard.writeText(this.configTextElement()!.nativeElement.innerText).then(
            () => this.snackBar.open('Config copied to clipboard!', undefined, {
                verticalPosition: 'bottom',
                horizontalPosition: 'right',
                duration: 3000,
                panelClass: 'whitespace-pre'
            }),
            () => this.snackBar.open('Failed to copy config!', undefined, {
                verticalPosition: 'bottom',
                horizontalPosition: 'right',
                duration: 3000,
                panelClass: 'whitespace-pre'
            }));
    }

    onDownload() {
        if (!this.configTextElement()?.nativeElement) {
            return;
        }

        var element = document.createElement('a');
        element.setAttribute('href', 'data:text/plain;charset=utf-8,'
            + encodeURIComponent(this.configTextElement()!.nativeElement.innerText));

        element.setAttribute('download', 'config.txt');

        element.style.display = 'none';
        document.body.appendChild(element);

        element.click();

        document.body.removeChild(element);
    }

    onAddSensor() {
        if (!this.selectedMicrocontrollerSensorId) {
            return;
        }

        if (this.config.microcontrollerSensors.length >= 5) {
            this.snackBar.open('Maximum allowed sensors per microcontroller: 5', undefined, {
                verticalPosition: 'bottom',
                horizontalPosition: 'right',
                duration: 3000,
                panelClass: 'whitespace-pre'
            });

            return;
        }

        const insertingMicrocontrollerSensorId: guid = typeof this.selectedMicrocontrollerSensorId === 'string'
            ? this.selectedMicrocontrollerSensorId
            : this.selectedMicrocontrollerSensorId.microcontrollerSensorId;

        if (checkGuid(insertingMicrocontrollerSensorId)) {
            if (this.config.microcontrollerSensors.includes(insertingMicrocontrollerSensorId)) {
                this.snackBar.open('Microcontroller Sensor Id already entered!', undefined, {
                    verticalPosition: 'bottom',
                    horizontalPosition: 'right',
                    duration: 3000,
                    panelClass: 'whitespace-pre'
                });

                return;
            }

            this.config.microcontrollerSensors = [...this.config.microcontrollerSensors, insertingMicrocontrollerSensorId];
            this.selectedMicrocontrollerSensorId = undefined;
        } else {
            this.snackBar.open('Invalid Microcontroller Sensor Id!', undefined, {
                verticalPosition: 'bottom',
                horizontalPosition: 'right',
                duration: 3000,
                panelClass: 'whitespace-pre'
            });
        }
    }

    onDeleteSensor(sensorId: guid) {
        this.config.microcontrollerSensors = this.config.microcontrollerSensors.filter(x => x !== sensorId);
    }
}

