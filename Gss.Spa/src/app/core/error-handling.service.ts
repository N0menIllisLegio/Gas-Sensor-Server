import { HttpErrorResponse } from "@angular/common/http";
import ErrorModel from "./error.model";
import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export default class ErrorHandlingService {
    convertError(error: HttpErrorResponse): ErrorModel {
        if (error.status === 0) {
            console.error('An error occurred:', error.error);

            return new ErrorModel('Client error');
        } else {
            console.error(`Backend returned code ${error.status}, body was: `, error.error);

            return new ErrorModel('Backend returned: ' + error.status);
        }
    }
}