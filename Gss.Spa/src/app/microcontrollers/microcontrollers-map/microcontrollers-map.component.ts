import { Component, inject, signal } from "@angular/core";
import { latLng, Map, LatLngBounds } from "leaflet";
import { MatTableModule } from "@angular/material/table";
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSortModule, Sort } from "@angular/material/sort";
import { MicrocontrollersQueryService } from "../core/microcontrollers-query.service";
import { BehaviorSubject, catchError, debounceTime, distinctUntilChanged, filter, of, switchMap } from "rxjs";
import { DateTimePipe } from "../../shared/date-time.pipe";
import { EmptyPlaceholderPipe } from "../../shared/empty-placeholder.pipe";
import { CoordinatesPipe } from "../../shared/coordinates.pipe";
import { FormsModule } from '@angular/forms';
import PagedRequestModel from "../../core/paged-request.model";
import MicrocontrollerModel from "../core/microcontroller.model";
import SortOptionModel from "../../core/sort-option.model";
import { SpinnerComponent } from "../../shared/spinner/spinner.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import ErrorHandlingService from "../../core/error-handling.service";
import { MapComponent } from "../shared/map/map.component";
import FlyToTargetModel from "../shared/map/fly-to-target.model";
import DisplayableMicrocontrollerModel from "../shared/map/displayable-microcontroller.model";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { Router } from "@angular/router";

@Component({
    selector: 'microcontrollers-map',
    templateUrl: './microcontrollers-map.component.html',
    styleUrl: './microcontrollers-map.component.scss',
    imports: [
        DateTimePipe,
        EmptyPlaceholderPipe,
        CoordinatesPipe,
        MatTableModule,
        MatCardModule,
        MatPaginatorModule,
        MatSortModule,
        MatInputModule,
        MatFormFieldModule,
        MatButtonModule,
        MatIconModule,
        SpinnerComponent,
        FormsModule,
        MapComponent
    ],
})
export class MicrocontrollersMapComponent {
    private snackBar = inject(MatSnackBar);
    private router = inject(Router);
    private microcontrollerQueryService = inject(MicrocontrollersQueryService);
    private errorHandlingService = inject(ErrorHandlingService);
    private mapBoundsSubject = new BehaviorSubject<LatLngBounds | null>(null);
    private pagedRequestSubject = new BehaviorSubject<PagedRequestModel | null>(null);

    addMicrocontrollers = new BehaviorSubject<DisplayableMicrocontrollerModel[] | null>(null);
    flyToTarget = new BehaviorSubject<FlyToTargetModel | null>(null);
    mapZoom = 4;
    mapCenter = latLng(54.5260, 15.2551);

    displayedColumns = ['name', 'coordinates', 'lastResponseTime', 'sensorsCount', 'action'];
    isTableLoading = signal<boolean>(false);
    dataSource = signal<MicrocontrollerModel[]>([]);
    totalMicrocontrollers = signal<number>(0);
    pageNumber = 0;
    pageSize = 10;
    searchString = '';
    sortOptions: SortOptionModel[] = [];

    constructor() {
        this.mapBoundsSubject
            .pipe(
                filter(x => x !== null),
                debounceTime(500),
                distinctUntilChanged((prev, curr) => prev?.equals(curr) ?? false),
                switchMap(bounds => {
                    const southWest = bounds.getSouthWest();
                    const northEast = bounds.getNorthEast();

                    return this.microcontrollerQueryService
                        .getMicrocontrollersMap(southWest.lat, southWest.lng, northEast.lat, northEast.lng)
                        .pipe(catchError(() => of(null)));
                }))
            .subscribe(microcontrollers => {
                if (!microcontrollers)
                    return;

                this.addMicrocontrollers.next(
                    microcontrollers.map(microcontroller => new DisplayableMicrocontrollerModel(
                        microcontroller.latitude, microcontroller.longitude, microcontroller.sensorTypes.map(x => x.name))));
            });

        this.pagedRequestSubject
            .pipe(
                filter(x => x !== null),
                debounceTime(800),
                distinctUntilChanged((prev, curr) => prev?.equals(curr) ?? false),
                switchMap(x => {
                    this.isTableLoading.set(true);

                    return this.microcontrollerQueryService.getPublicMicrocontrollers(x)
                        .pipe(catchError(err => {
                            const error = this.errorHandlingService.convertError(err);

                            this.snackBar.open(error.message, undefined, {
                                horizontalPosition: 'right',
                                verticalPosition: 'bottom',
                                duration: 5000,
                                panelClass: 'whitespace-pre'
                            });

                            return of(null);
                        }));
                })
            )
            .subscribe((data) => {
                this.isTableLoading.set(false);

                if (!data)
                    return;

                this.dataSource.set(data.items);
                this.totalMicrocontrollers.set(data.totalItemsCount);
            });

        this.loadPublicMicrocontrollers();
    }

    onPageChanged(pageNumber: number, pageSize: number) {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;

        this.loadPublicMicrocontrollers();
    }

    sortData(sortOption: Sort) {
        this.sortOptions = [...this.sortOptions.filter(x => x.propertyName !== sortOption.active)];

        if (sortOption.direction !== '')
            this.sortOptions.push(new SortOptionModel(sortOption.direction === 'asc', sortOption.active));

        this.loadPublicMicrocontrollers();
    }

    loadPublicMicrocontrollers() {
        this.pagedRequestSubject.next(
            new PagedRequestModel(this.pageNumber + 1, this.pageSize, this.searchString, this.sortOptions));
    }

    updateMapData(map: Map) {
        const mapBounds = map.getBounds();
        this.mapBoundsSubject.next(mapBounds);
    }

    onCoordsClick(microcontroller: MicrocontrollerModel) {
        if (microcontroller.latitude && microcontroller.longitude) {
            this.flyToTarget.next(
                new FlyToTargetModel(microcontroller.latitude, microcontroller.longitude, 8));
        }
    }

    onNavigateToMicrocontrollerPage(microcontroller: MicrocontrollerModel) {
        this.router.navigateByUrl(`/microcontrollers/${microcontroller.id}`);
    }
}