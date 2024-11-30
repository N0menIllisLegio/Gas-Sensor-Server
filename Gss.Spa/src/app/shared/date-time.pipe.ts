import { Pipe, PipeTransform } from '@angular/core';

@Pipe({  name: 'dateTime' })
export class DateTimePipe implements PipeTransform {
    transform(value: Date | null | undefined): string | null {
        return value?.toLocaleString() ?? null;
    }
}
