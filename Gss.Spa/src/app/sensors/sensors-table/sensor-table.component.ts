import { Component, inject, input, signal } from '@angular/core';
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
import { BehaviorSubject, catchError, debounceTime, filter, of, switchMap } from 'rxjs';
import SortOptionModel from '../../core/sort-option.model';
import { EmptyPlaceholderPipe } from '../../shared/empty-placeholder.pipe';
import { MatDialog } from '@angular/material/dialog';
import DialogResultModel from '../../core/dialog-result.model';
import { SensorsQueryService } from '../core/sensor-query.service';
import SensorModel from '../core/sensor.model';
import { EditSensorDialogComponent } from '../edit-sensor-dialog/edit-sensor-dialog.component';
import AuthService from '../../core/auth.service';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { SelectionModel } from '@angular/cdk/collections';
import { guid } from '../../core/guid';

@Component({
    selector: 'sensor-table',
    templateUrl: 'sensor-table.component.html',
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
        FormsModule,
        MatCheckboxModule
    ]
})
export class SensorsTableComponent {
    authService = inject(AuthService);
    private snackBar = inject(MatSnackBar);
    private dialog = inject(MatDialog);

    private errorHandlingService = inject(ErrorHandlingService);
    private pagedRequestSubject = new BehaviorSubject<PagedRequestModel | null>(null);
    private sensorTypesQueryService = inject(SensorsQueryService);

    displayedColumns = ['select', 'icon', 'units', 'name'];
    isTableLoading = signal<boolean>(false);
    dataSource = signal<SensorModel[]>([]);

    totalSensorTypes = signal<number>(0);
    pageNumber = 0;
    pageSize = 10;
    searchString = '';
    sortOptions: SortOptionModel[] = [];

    isSelectionEnabled = input<boolean>(false);
    selection = input<SelectionModel<guid>>(new SelectionModel<guid>(true, []));

    constructor() {
        this.pagedRequestSubject
            .pipe(
                filter(x => x !== null),
                debounceTime(800),
                switchMap(x => {
                    this.isTableLoading.set(true);

                    return this.sensorTypesQueryService.getSensors(x)
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

        this.loadSensors();
    }

    ngOnInit() {
        if (!this.isSelectionEnabled())
            this.displayedColumns.shift();
    }

    loadSensors() {
        this.pagedRequestSubject.next(
            new PagedRequestModel(this.pageNumber + 1, this.pageSize, this.searchString, this.sortOptions));
    }

    onPageChanged(pageNumber: number, pageSize: number) {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;

        this.loadSensors();
    }

    sortData(sortOption: Sort) {
        this.sortOptions = [...this.sortOptions.filter(x => x.propertyName !== sortOption.active)];

        if (sortOption.direction !== '')
            this.sortOptions.push(new SortOptionModel(sortOption.direction === 'asc', sortOption.active));

        this.loadSensors();
    }

    onRowClicked(sensorType: SensorModel) {
        if (this.isSelectionEnabled()) {
            this.selection().toggle(sensorType.id);
            return;
        }

        if (!this.authService.isAdmin) {
            // TODO: handle gracefully
            console.log('Insuficient permissions!');

            return;
        }

        const dialogRef = this.dialog.open(EditSensorDialogComponent, {
            panelClass: 'w-2/5',
            data: sensorType,
            disableClose: true
        });

        dialogRef.afterClosed()
            .subscribe(x => {
                const result = x as DialogResultModel;

                if (!result || result.action === 'cancel')
                    return;

                this.loadSensors();
            });
    }

    onCreateType() {
        const dialogRef = this.dialog.open(EditSensorDialogComponent, {
            panelClass: 'w-2/5',
            data: null,
            disableClose: true
        });

        dialogRef.afterClosed()
            .subscribe(x => {
                const result = x as DialogResultModel;

                if (!result || result.action === 'cancel')
                    return;

                this.loadSensors();
            });
    }
}