import { Component, inject, signal } from "@angular/core";
import { LeafletModule } from '@bluehalo/ngx-leaflet';
import { latLng, tileLayer, Map, marker, Layer, icon, Icon, latLngBounds, MapOptions, LatLngBounds, tooltip } from "leaflet";
import { MicrocontrollersQueryService } from "../core/microcontrollers-query.service";
import { BehaviorSubject, debounceTime, distinctUntilChanged, filter, switchMap } from "rxjs";

@Component({
    selector: 'microcontrollers-map',
    imports: [LeafletModule],
    templateUrl: './map.component.html',
    providers: [MicrocontrollersQueryService] // TODO: read where to inject. Scoped, single, transient
})
export class MapComponent {
    title = 'Map';
    private microcontrollerQueryService = inject(MicrocontrollersQueryService);
    private mapBoundsSubject = new BehaviorSubject<LatLngBounds | null>(null);
    private markerIcon = icon({
        ...Icon.Default.prototype.options,
        iconUrl: 'assets/marker-icon.png',
        iconRetinaUrl: 'assets/marker-icon-2x.png',
        shadowUrl: 'assets/marker-shadow.png'
    });

    mapLayers = signal<Layer[]>([]);
    initMapOptions: MapOptions = {
        layers: [
            tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
            }),
        ],
        
        maxZoom: 18,
        
        // Prevent moving out of the world bounds
        minZoom: 3,
        maxBounds: latLngBounds(latLng(-90, -180), latLng(90, 180)),
        maxBoundsViscosity: 1,

        // Europe's center
        zoom: 4,
        center: latLng(54.5260, 15.2551) 
    };

    constructor() {
        this.mapBoundsSubject
            .pipe(
                filter(x => x !== null),
                debounceTime(1000),
                distinctUntilChanged((prev, curr) => prev.equals(curr)),
                switchMap(bounds => {
                    const southWest = bounds.getSouthWest();
                    const northEast = bounds.getNorthEast();
                    
                    return this.microcontrollerQueryService
                        .getMicrocontrollersMap(southWest.lat, southWest.lng, northEast.lat, northEast.lng);
                }))
            .subscribe(microcontrollers => {
                this.mapLayers.set(microcontrollers.map(microcontroller => {
                    const mcMarker = marker([microcontroller.latitude, microcontroller.longitude], {
                        icon: this.markerIcon,
                    });

                    const markerTooltip = tooltip({
                        content: microcontroller.sensorTypes.map(x => x.name).join('<br/>'),
                    });
                    
                    mcMarker.bindTooltip(markerTooltip);

                    return mcMarker;
                }));
            });
    }

    onMapMoveEnd(map: Map) {
        const mapBounds = map.getBounds();
        this.mapBoundsSubject.next(mapBounds);
    }

    onMapReady(map: Map) {
        const mapBounds = map.getBounds();
        this.mapBoundsSubject.next(mapBounds);
    }
}