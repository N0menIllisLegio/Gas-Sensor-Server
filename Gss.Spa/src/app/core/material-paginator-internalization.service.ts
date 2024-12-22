import { Injectable } from "@angular/core";
import { MatPaginatorIntl } from "@angular/material/paginator";
import { TranslateService } from "@ngx-translate/core";
import { Subject } from "rxjs";
import { marker as _ } from '@colsen1991/ngx-translate-extract-marker';

@Injectable({ providedIn: 'root' })
export class MaterialPaginatorIntl implements MatPaginatorIntl {
  changes = new Subject<void>();

  constructor(private translate: TranslateService) {
    this.translate.onLangChange
        .subscribe(() => {
            this.changes.next();
            this.setLocalizedLabels()
        });

    this.setLocalizedLabels();
  }

  firstPageLabel = '';
  itemsPerPageLabel = '';
  lastPageLabel = '';
  nextPageLabel = '';
  previousPageLabel = '';

  getRangeLabel(page: number, pageSize: number, length: number): string {
    if (length === 0) {
      return this.translate.instant(_('common.paginator.range'), { currentPage: 1, totalPages: 1 });
    }

    const amountPages = Math.ceil(length / pageSize);

    return this.translate.instant(_('common.paginator.range'), { currentPage: page + 1, totalPages: amountPages });
  }

  setLocalizedLabels(): void {
    this.itemsPerPageLabel = this.translate.instant(_('common.paginator.items-per-page'));
    this.lastPageLabel = this.translate.instant(_('common.paginator.last-page'));
    this.firstPageLabel = this.translate.instant(_('common.paginator.first-page'));
    this.nextPageLabel = this.translate.instant(_('common.paginator.next-page'));
    this.previousPageLabel = this.translate.instant(_('common.paginator.previous-page'));
  }
}
