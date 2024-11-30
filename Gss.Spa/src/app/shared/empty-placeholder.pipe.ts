import { Pipe, PipeTransform } from '@angular/core';

@Pipe({  name: 'emptyPlaceholder' })
export class EmptyPlaceholderPipe implements PipeTransform {
    transform(value: any): any {
        return value ?? '-/-';
    }
}
