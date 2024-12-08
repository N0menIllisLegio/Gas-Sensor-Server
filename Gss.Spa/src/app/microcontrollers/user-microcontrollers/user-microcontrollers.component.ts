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
import { BehaviorSubject, catchError, debounceTime, distinctUntilChanged, filter, of, switchMap } from 'rxjs';
import SortOptionModel from '../../core/sort-option.model';
import { EmptyPlaceholderPipe } from '../../shared/empty-placeholder.pipe';
import AuthService from '../../core/auth.service';
import { MicrocontrollersQueryService } from '../core/microcontrollers-query.service';
import MicrocontrollerModel from '../core/microcontroller.model';
import { guid } from '../../core/guid';
import { CoordinatesPipe } from "../../shared/coordinates.pipe";
import { DateTimePipe } from "../../shared/date-time.pipe";
import { Router } from '@angular/router';

@Component({
    selector: 'user-microcontrollers',
    templateUrl: 'user-microcontrollers.component.html',
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
        CoordinatesPipe,
        DateTimePipe
    ]
})
export class UserMicrocontrollersComponent {
    authService = inject(AuthService);
    private snackBar = inject(MatSnackBar);
    private router = inject(Router);

    private errorHandlingService = inject(ErrorHandlingService);
    private pagedRequestSubject = new BehaviorSubject<PagedRequestModel | null>(null);
    private microcontrollersQueryService = inject(MicrocontrollersQueryService);

    displayedColumns = ['name', 'coordinates', 'lastResponseTime', 'sensorsCount'];
    isTableLoading = signal<boolean>(false);
    dataSource = signal<MicrocontrollerModel[]>([]);
    userId = input<guid>();

    totalMicrocontrollers = signal<number>(0);
    pageNumber = 0;
    pageSize = 10;
    searchString = '';
    sortOptions: SortOptionModel[] = [];

    constructor() {
        this.pagedRequestSubject
            .pipe(
                filter(x => x !== null),
                debounceTime(800),
                distinctUntilChanged((prev, curr) => {
                    return prev?.equals(curr) ?? false }),
                switchMap(x => {
                    this.isTableLoading.set(true);

                    return this.microcontrollersQueryService.getUserPublicMicrocontrollers(this.userId()!, x)
                        .pipe(catchError(err => {
                            const error = this.errorHandlingService.convertError(err);

                            this.snackBar.open(error.message, undefined, {
                                horizontalPosition: 'right',
                                verticalPosition: 'bottom',
                                duration: 5000,
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
                this.totalMicrocontrollers.set(data.totalItemsCount);
            });
    }

    ngOnInit() {
        if (!this.userId()) {
            // TODO: 401 handle
            return;
        }

        this.loadMicrocontrollers();
    }

    loadMicrocontrollers() {
        this.pagedRequestSubject.next(
            new PagedRequestModel(this.pageNumber + 1, this.pageSize, this.searchString, this.sortOptions));
    }

    onPageChanged(pageNumber: number, pageSize: number) {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;

        this.loadMicrocontrollers();
    }

    sortData(sortOption: Sort) {
        this.sortOptions = [...this.sortOptions.filter(x => x.propertyName !== sortOption.active)];

        if (sortOption.direction !== '')
            this.sortOptions.push(new SortOptionModel(sortOption.direction === 'asc', sortOption.active));

        this.loadMicrocontrollers();
    }

    onRowClicked(microcontroller: MicrocontrollerModel) {
        this.router.navigateByUrl(`/microcontrollers/${microcontroller.id}`);
    }

    onCreateMicrocontroller() {
        this.router.navigateByUrl('/microcontrollers/create');
    }
}