import { MatButton } from '@angular/material/button';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { MatSlider } from '@angular/material/slider';

import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: 'map', component: MatButton },
    { path: 'microcontrollers/public', component: MatSlider },
    { path: 'configuration-generator', component: MatSlideToggle }
];
