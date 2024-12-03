import { MatSlideToggle } from '@angular/material/slide-toggle';

import { NotFoundComponent } from './not-found-page/not-found.component';
import { MicrocontrollersMapComponent } from './microcontrollers/microcontrollers-map/microcontrollers-map.component';
import { Routes } from '@angular/router';
import { MicrocontrollerDetailsComponent } from './microcontrollers/details/microcontroller-details.component';

export const routes: Routes = [
    { path: 'map', component: MicrocontrollersMapComponent },
    { path: 'microcontrollers/:microcontrollerId', component: MicrocontrollerDetailsComponent },
    { path: 'configuration-generator', component: MatSlideToggle },
    { path: '',   redirectTo: '/map', pathMatch: 'full' },
    { path: 'not-found', component: NotFoundComponent },
    { path: '**', redirectTo: '/not-found' },
];
