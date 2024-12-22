import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { SpinnerComponent } from '../../shared/spinner/spinner.component';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import ErrorHandlingService from '../../core/error-handling.service';
import PagedRequestModel from '../../core/paged-request.model';
import { BehaviorSubject, catchError, debounceTime, distinctUntilChanged, filter, of, switchMap } from 'rxjs';
import SortOptionModel from '../../core/sort-option.model';
import { SensorTypesQueryService } from '../core/sensor-types-query.service';
import SensorTypeModel from '../core/sensor-type.model';
import { EmptyPlaceholderPipe } from '../../shared/empty-placeholder.pipe';
import { EditSensorTypeDialogComponent } from '../edit-sensor-type-dialog/edit-sensor-type-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import DialogResultModel from '../../core/dialog-result.model';

@Component({
    selector: 'sensor-types-table',
    templateUrl: 'sensor-types-table.component.html',
    imports: [
        MatCardModule,
        EmptyPlaceholderPipe,
        MatTableModule,
        MatCardModule,
        MatPaginatorModule,
        MatSortModule,
        MatInputModule,
        MatFormFieldModule,
        MatButtonModule,
        SpinnerComponent,
        FormsModule
    ]
})
export class SensorTypesTableComponent {
    private snackBar = inject(MatSnackBar);
    private dialog = inject(MatDialog);

    private errorHandlingService = inject(ErrorHandlingService);
    private pagedRequestSubject = new BehaviorSubject<PagedRequestModel | null>(null);
    private sensorTypesQueryService = inject(SensorTypesQueryService);

    displayedColumns = ['icon', 'units', 'name'];
    isTableLoading = signal<boolean>(false);
    dataSource = signal<SensorTypeModel[]>([]);

    totalSensorTypes = signal<number>(0);
    pageNumber = 0;
    pageSize = 10;
    searchString = '';
    sortOptions: SortOptionModel[] = [];

    constructor() {
        this.pagedRequestSubject
            .pipe(
                filter(x => x !== null),
                debounceTime(800),
                switchMap(x => {
                    this.isTableLoading.set(true);

                    return this.sensorTypesQueryService.getSensorTypes(x)
                        .pipe(catchError(err => {
                            const error = this.errorHandlingService.convertError(err);

                            this.snackBar.open(error.message, undefined, {
                                horizontalPosition: 'right',
                                verticalPosition: 'bottom',
                                duration: 5000,
                                panelClass: 'whitespace-pre'
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
                this.totalSensorTypes.set(data.totalItemsCount);
            });

        this.loadSensorTypes();
    }

    loadSensorTypes() {
        this.pagedRequestSubject.next(
            new PagedRequestModel(this.pageNumber + 1, this.pageSize, this.searchString, this.sortOptions));
    }

    onPageChanged(pageNumber: number, pageSize: number) {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;

        this.loadSensorTypes();
    }

    sortData(sortOption: Sort) {
        this.sortOptions = [...this.sortOptions.filter(x => x.propertyName !== sortOption.active)];

        if (sortOption.direction !== '')
            this.sortOptions.push(new SortOptionModel(sortOption.direction === 'asc', sortOption.active));

        this.loadSensorTypes();
    }

    onRowClicked(sensorType: SensorTypeModel) {
        const dialogRef = this.dialog.open(EditSensorTypeDialogComponent, {
            panelClass: 'gss-dialog',
            data: sensorType,
            disableClose: true
        });

        dialogRef.afterClosed()
            .subscribe(x => {
                const result = x as DialogResultModel;

                if (!result || result.action === 'cancel')
                    return;

                this.loadSensorTypes();
            });
    }

    onCreateType() {
        const dialogRef = this.dialog.open(EditSensorTypeDialogComponent, {
            panelClass: 'gss-dialog',
            data: null,
            disableClose: true
        });

        dialogRef.afterClosed()
            .subscribe(x => {
                const result = x as DialogResultModel;

                if (!result || result.action === 'cancel')
                    return;

                this.loadSensorTypes();
            });
    }
}