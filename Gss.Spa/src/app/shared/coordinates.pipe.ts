import { Pipe, PipeTransform } from '@angular/core';

@Pipe({  name: 'coordinates' })
export class CoordinatesPipe implements PipeTransform {
    transform(coords: { latitude: number | null, longitude: number | null }): string | null {
        return coords.latitude === null || coords.longitude === null
            ? null
            : `${coords.latitude}, ${coords.longitude}`;
    }
}
