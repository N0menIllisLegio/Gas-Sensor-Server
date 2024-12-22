import { Component, effect, inject, input, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ApexAxisChartSeries, NgApexchartsModule, ApexAnnotations } from "ng-apexcharts";
import { watchingPeriod } from './watching-period.type';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { guid } from '../../../core/guid';
import { HttpClient } from '@angular/common/http';
import { formatDate } from '@angular/common';
import { BehaviorSubject, catchError, debounceTime, filter, of, switchMap } from 'rxjs';
import dictionary from '../../../core/dictionary.type';
import { ChartOptions } from './chart-options.model';
import WatchingDateResponseModel from './watching-date-response.model';
import WatchingDateRequestModel from './watching-date-request.model';
import { SpinnerComponent } from '../../../shared/spinner/spinner.component';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { marker as _ } from '@colsen1991/ngx-translate-extract-marker';

@Component({
    selector: 'data-chart',
    templateUrl: 'data-chart.component.html',
    providers: [provideNativeDateAdapter()],
    imports: [
        NgApexchartsModule,
        MatSelectModule,
        MatFormFieldModule,
        MatInputModule,
        MatDatepickerModule,
        MatButtonModule,
        MatChipsModule,
        MatIconModule,
        FormsModule,
        SpinnerComponent,
        TranslateModule
    ]
})
export class DataChartComponent {
    private translate = inject(TranslateService);
    private snackBar = inject(MatSnackBar);
    private httpClient = inject(HttpClient);
    private watchingDateSubject = new BehaviorSubject<WatchingDateRequestModel | null>(null);

    readonly isLoadingData = signal(false);

    chartOptions: Partial<ChartOptions> = {
        chart: {
            height: 450,
            type: "line",
            selection: {
                enabled: false,
            },
            zoom: {
                enabled: false
            }
        },
        stroke: {
            curve: "smooth"
        },
        xaxis: {
            type: "datetime"
        },
        tooltip: {
            enabled: true,
        },
        dataLabels: {
            enabled: true
        }
    };

    annotations = signal<ApexAnnotations | null>(null);
    chartData = signal<ApexAxisChartSeries>([]);

    watchingDates = signal<Date[]>([]);
    selectedDate = signal<Date | null>(null);
    period = signal<watchingPeriod>('Day');

    criticalValue = input<number | null>(null);
    microcontrollerSensorId = input.required<guid>();

    constructor() {
        effect(() => {
            if (this.criticalValue())
                this.annotations.set({
                    yaxis: [{
                        y: this.criticalValue(),
                        borderColor: 'var(--mat-sys-error)',
                        label: {
                            text: this.translate.instant(_('data-chart.label.critical-value')),
                            borderColor: 'var(--mat-sys-error)',
                            style: {
                                color: 'white',
                                background: 'var(--mat-sys-error)'
                            },
                        }
                    }]
                });
        });

        this.watchingDateSubject
            .pipe(
                filter(x => x !== null && x.watchingDates.length > 0),
                debounceTime(500),
                switchMap(request => {
                    this.isLoadingData.set(true);

                    return this.httpClient
                        .post<dictionary<WatchingDateResponseModel[]>>('/api/SensorsData/GetSensorData', request)
                        .pipe(catchError(() => of(null)))
                }))
            .subscribe(response => {
                this.isLoadingData.set(false);

                if (response === null)
                    return;

                const result: ApexAxisChartSeries = [];

                for (let [key, value] of Object.entries(response)) {
                    result.push({
                        name: key,
                        data: value.map(watchingDate => ({
                            x: watchingDate.readTime,
                            y: watchingDate.averageValue
                        }))
                    });
                }

                this.chartData.set(result);
            });
    }

    addWatchingDate() {
        if (!this.selectedDate()) {
            this.snackBar.open(this.translate.instant(_('data-chart.snack-bar.select-watching-date')),
                undefined, {
                    duration: 3000,
                    horizontalPosition: 'right',
                    verticalPosition: 'bottom',
                    panelClass: 'whitespace-pre'
                });
            return;
        }

        if (this.watchingDates().find(x => x.getTime() === this.selectedDate()!.getTime()) !== undefined) {
            this.snackBar.open(this.translate.instant(_('data-chart.snack-bar.date-already-added')),
                undefined, {
                    duration: 3000,
                    horizontalPosition: 'right',
                    verticalPosition: 'bottom',
                    panelClass: 'whitespace-pre'
                });
            return;
        }

        if (this.watchingDates().length > 4) {
            this.snackBar.open(this.translate.instant(_('data-chart.snack-bar.you-can-watch-only-count-series'), { count: 5 }),
                undefined, {
                    duration: 3000,
                    horizontalPosition: 'right',
                    verticalPosition: 'bottom',
                    panelClass: 'whitespace-pre'
                });
            return;
        }

        this.watchingDates.set([...this.watchingDates(), this.selectedDate()!]);

        this.getData();
    }

    removeWatchingDate(removingDate: Date) {
        this.watchingDates.set(
            this.watchingDates().filter(x => x.getTime() !== removingDate.getTime()));

        this.chartData.set(this.chartData().filter(x => x.name !== formatDate(removingDate, 'yyyy-MM-dd', 'en-US')));
    }

    getData() {
        this.watchingDateSubject.next(new WatchingDateRequestModel(
            this.microcontrollerSensorId(),
            this.period(),
            this.watchingDates().map(x => formatDate(x, 'yyyy-MM-dd', 'en-US'))));
    }
}