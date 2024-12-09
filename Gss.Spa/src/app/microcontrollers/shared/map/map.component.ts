import { Component, input, output, signal } from '@angular/core';
import { LeafletModule } from '@bluehalo/ngx-leaflet';
import { icon, Icon, latLng, latLngBounds, Layer, MapOptions, tileLayer, Map, LatLng, marker, tooltip, LeafletMouseEvent } from 'leaflet';
import DisplayableMicrocontrollerModel from './displayable-microcontroller.model';
import { Observable } from 'rxjs';
import FlyToTargetModel from './fly-to-target.model';

@Component({
    selector: 'map',
    templateUrl: 'map.component.html',
    imports: [
        LeafletModule
    ]
})
export class MapComponent {
    private markerIcon = icon({
        ...Icon.Default.prototype.options,
        iconUrl: 'assets/marker-icon.png',
        iconRetinaUrl: 'assets/marker-icon-2x.png',
        shadowUrl: 'assets/marker-shadow.png'
    });

    private map: Map | undefined;
    mapLayers = signal<Layer[]>([]);

    zoom = input<number>(4);
    zoomChange = output<number>();
    center = input(latLng(54.5260, 15.2551));
    centerChange = output<LatLng>();
    leafletMapMoveEnd = output<Map>();
    leafletMapReady = output<Map>();
    leafletDoubleClick = output<LeafletMouseEvent>();

    microcontrollerAdded = input<Observable<DisplayableMicrocontrollerModel | null>>();
    microcontrollersAdded = input<Observable<DisplayableMicrocontrollerModel[] | null>>();
    flyToTarget = input<Observable<FlyToTargetModel | null>>();

    initMapOptions: MapOptions = {
        layers: [
            tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
            }),
        ],

        maxZoom: 18,
        center: latLng(10, 10),
        zoom: 1,

        // Prevent moving out of the world bounds
        minZoom: 3,
        maxBounds: latLngBounds(latLng(-90, -180), latLng(90, 180)),
        maxBoundsViscosity: 1,
    };

    onMapReady(map: Map) {
        this.map = map;
        this.leafletMapReady.emit(map);
    }

    ngOnInit() {
        this.microcontrollerAdded()?.subscribe(microcontroller => {
            if (!microcontroller)
                return;

            const mcMarker = marker([microcontroller.latitude, microcontroller.longitude], {
                icon: this.markerIcon,
            });

            if (microcontroller.sensorTypeNames)
            {
                const markerTooltip = tooltip({
                    content: microcontroller.sensorTypeNames.length > 0
                        ? microcontroller.sensorTypeNames.join('<br/>')
                        : 'No sensors connected',
                });

                mcMarker.bindTooltip(markerTooltip);
            }

            this.mapLayers.set([mcMarker]);
        });

        this.microcontrollersAdded()?.subscribe(microcontrollers => {
            if (!microcontrollers)
                return;

            const markers = microcontrollers.map(microcontroller => {
                const mcMarker = marker([microcontroller.latitude, microcontroller.longitude], {
                    icon: this.markerIcon,
                });

                if (microcontroller.sensorTypeNames)
                {
                    const markerTooltip = tooltip({
                        content: microcontroller.sensorTypeNames.length > 0
                            ? microcontroller.sensorTypeNames.join('<br/>')
                            : 'No sensors connected',
                    });

                    mcMarker.bindTooltip(markerTooltip);
                }

                return mcMarker;
            });

            this.mapLayers.set(markers);
        });

        this.flyToTarget()?.subscribe(target => {
            if (!target || !this.map)
                return;

            this.map.flyTo(latLng(target.latitude, target.longitude), target.zoom, { animate: true });
        });

        const mapResizeObserver = new ResizeObserver((entry) => {
            // without it, map renders without half of tiles
            const mapElement = entry[0];

            if (mapElement.contentRect.height > 0) {
                this.map!.invalidateSize();
            }
        })

        mapResizeObserver.observe(document.querySelector('#leaflet-map')!);
    }
}