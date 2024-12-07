import { Component, ElementRef, inject, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { provideNativeDateAdapter } from '@angular/material/core';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';

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
        MatSelectModule
    ]
})

export class ConfigGeneratorComponent {
    private snackBar = inject(MatSnackBar);
    private configTextElement = viewChild<ElementRef<HTMLDivElement>>('configurationText');

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
        dataRequestSeconds: '900',
        // TODO: remove. it is no longer used on the backend, and it is just random guid to follow protocol
        ownerId: 'a8e9315b-ae0d-4ced-bd3f-e80f2af4a7d5',
        microcontrollerId: '',
        microcontrollerKey: '',
        microcontrollerSensorId: ''
    };

    onCopy() {
        if (!navigator.clipboard || !this.configTextElement()?.nativeElement) {
            return;
        }

        navigator.clipboard.writeText(this.configTextElement()!.nativeElement.innerText).then(
            () => this.snackBar.open('Config copied to clipboard!', undefined, {
                verticalPosition: 'bottom',
                horizontalPosition: 'right'
            }),
            () => this.snackBar.open('Failed to copy config!', undefined, {
                verticalPosition: 'bottom',
                horizontalPosition: 'right'
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
}

