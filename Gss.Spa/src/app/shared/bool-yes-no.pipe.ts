import { inject, Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { marker as _ } from '@colsen1991/ngx-translate-extract-marker';

@Pipe({  name: 'boolYesNo' })
export class BoolYesNoPipe implements PipeTransform {
    readonly translate = inject(TranslateService)

    transform(value: boolean): string {
        return value
            ? this.translate.instant(_('common.yes'))
            : this.translate.instant(_('common.no'));
    }
}
