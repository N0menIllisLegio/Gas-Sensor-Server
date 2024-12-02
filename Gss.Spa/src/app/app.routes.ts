import { MatSlideToggle } from '@angular/material/slide-toggle';

import { NotFoundComponent } from './not-found-page/not-found.component';
import { MicrocontrollersMapComponent } from './microcontrollers/microcontrollers-map/microcontrollers-map.component';
import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: 'map', component: MicrocontrollersMapComponent },
    { path: 'configuration-generator', component: MatSlideToggle },
    { path: '',   redirectTo: '/map', pathMatch: 'full' },
    { path: 'not-found', component: NotFoundComponent },
    { path: '**', redirectTo: '/not-found' },
];
