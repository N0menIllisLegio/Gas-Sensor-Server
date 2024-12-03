import { Pipe, PipeTransform } from '@angular/core';

@Pipe({  name: 'boolYesNo' })
export class BoolYesNoPipe implements PipeTransform {
    transform(value: boolean): string {
        return value ? 'Yes' : 'No';
    }
}
