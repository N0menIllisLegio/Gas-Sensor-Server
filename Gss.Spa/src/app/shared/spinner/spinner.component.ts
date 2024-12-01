
import { Component } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

// Set parent component's position to relative.
@Component({
    selector: 'spinner',
    templateUrl: 'spinner.component.html',
    imports: [MatProgressSpinnerModule],
})
export class SpinnerComponent {
    title = "spinner"
}