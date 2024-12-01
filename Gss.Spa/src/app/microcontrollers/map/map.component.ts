import { Component, inject, signal } from "@angular/core";
import { latLng, tileLayer, Map, marker, Layer, icon, Icon, latLngBounds, MapOptions, LatLngBounds, tooltip } from "leaflet";
import { MatTableModule } from "@angular/material/table";
import { MatCardModule } from '@angular/material/card';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSortModule, Sort } from "@angular/material/sort";
import { LeafletModule } from '@bluehalo/ngx-leaflet';
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

@Component({
    selector: 'microcontrollers-map',
    templateUrl: './map.component.html',
    styleUrl: './map.component.scss',
    imports: [
        LeafletModule,
        DateTimePipe,
        EmptyPlaceholderPipe,
        CoordinatesPipe,
        MatTableModule,
        MatCardModule,
        MatPaginator,
        MatPaginatorModule,
        MatSortModule,
        MatInputModule,
        MatFormFieldModule,
        SpinnerComponent,
        FormsModule
    ],
})
export class MapComponent {
    title = 'Map';

    private snackBar = inject(MatSnackBar);
    private microcontrollerQueryService = inject(MicrocontrollersQueryService);
    private errorHandlingService = inject(ErrorHandlingService);
    private mapBoundsSubject = new BehaviorSubject<LatLngBounds | null>(null);
    private pagedRequestSubject = new BehaviorSubject<PagedRequestModel | null>(null);
    private markerIcon = icon({
        ...Icon.Default.prototype.options,
        iconUrl: 'assets/marker-icon.png',
        iconRetinaUrl: 'assets/marker-icon-2x.png',
        shadowUrl: 'assets/marker-shadow.png'
    });

    private map: Map | undefined;
    mapLayers = signal<Layer[]>([]);
    initMapOptions: MapOptions = {
        layers: [
            tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
            }),
        ],

        maxZoom: 18,
        center: latLng(54.5260, 15.2551),
        zoom: 4,

        // Prevent moving out of the world bounds
        minZoom: 3,
        maxBounds: latLngBounds(latLng(-90, -180), latLng(90, 180)),
        maxBoundsViscosity: 1,
    };

    displayedColumns = ['name', 'coordinates', 'lastResponseTime', 'sensorsCount'];
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

                this.mapLayers.set(microcontrollers.map(microcontroller => {
                    const mcMarker = marker([microcontroller.latitude, microcontroller.longitude], {
                        icon: this.markerIcon,
                    });

                    const markerTooltip = tooltip({
                        content: microcontroller.sensorTypes.length > 0
                            ? microcontroller.sensorTypes.map(x => x.name).join('<br/>')
                            : 'No sensors connected',
                    });

                    mcMarker.bindTooltip(markerTooltip);

                    return mcMarker;
                }));
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

    onMapReady(map: Map) {
        this.map = map;
        this.updateMapData(map);
    }

    ngOnInit() {
        const mapResizeObserver = new ResizeObserver((entry) => {
            // without it, map renders without half of tiles
            const mapElement = entry[0];

            if (mapElement.contentRect.height > 0) {
                this.map!.invalidateSize();
                mapResizeObserver.disconnect();
            }
        })

        mapResizeObserver.observe(document.querySelector('#microcontrollers-leaflet-map')!);
    }

    updateMapData(map: Map) {
        const mapBounds = map.getBounds();
        this.mapBoundsSubject.next(mapBounds);
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

    onCoordsClick(microcontroller: MicrocontrollerModel) {
        if (microcontroller.latitude && microcontroller.longitude && this.map) {
            this.map.flyTo(latLng(microcontroller.latitude, microcontroller.longitude), 8, { animate: true });
        }
    }
}