import { NotFoundComponent } from './not-found-page/not-found.component';
import { MicrocontrollersMapComponent } from './microcontrollers/microcontrollers-map/microcontrollers-map.component';
import { Routes } from '@angular/router';
import { MicrocontrollerDetailsComponent } from './microcontrollers/details/microcontroller-details.component';
import { ConfigGeneratorComponent } from './config-generator/config-generator.component';
import { SensorTypesTableComponent } from './sensor-types/sensor-types-table/sensor-types-table.component';
import { SensorsTableComponent } from './sensors/sensors-table/sensor-table.component';
import { UserMicrocontrollersComponent } from './microcontrollers/user-microcontrollers/user-microcontrollers.component';
import { EditMicrocontrollerComponent } from './microcontrollers/edit-microcontroller/edit-microcontroller.component';

export const routes: Routes = [
    { path: 'map', component: MicrocontrollersMapComponent },
    { path: 'microcontrollers/create', component: EditMicrocontrollerComponent },
    { path: 'microcontrollers/:microcontrollerId', component: MicrocontrollerDetailsComponent },
    { path: 'microcontrollers/:microcontrollerId/edit', component: EditMicrocontrollerComponent },
    { path: 'configuration-generator', component: ConfigGeneratorComponent },
    { path: 'user-microcontrollers/:userId', component: UserMicrocontrollersComponent },
    { path: 'sensors', component: SensorsTableComponent },
    { path: 'sensor-types', component: SensorTypesTableComponent },

    { path: '',   redirectTo: '/map', pathMatch: 'full' },
    { path: 'not-found', component: NotFoundComponent },
    { path: '**', redirectTo: '/not-found' },
];
