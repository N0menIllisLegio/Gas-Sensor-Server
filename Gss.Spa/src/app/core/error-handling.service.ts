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

            if (error.status === 401) {
                return new ErrorModel('Unauthorized access! Please login!');
            }

            if (error.status === 403) {
                return new ErrorModel('Forbidden! You don\'t have permissions to access this action!');
            }

            if (error.status === 403) {
                return new ErrorModel('Forbidden! You don\'t have permissions to access this action!');
            }

            if (error.status === 422) {
                const validationError = error as unknown as { error: { errors: { [key: string]: string[] } } };

                if (validationError) {
                    return new ErrorModel(Object.values(validationError.error.errors).flatMap(x => x).join('\n'));
                }

                return new ErrorModel('Validation error occured please check data you entered!');
            }

            return new ErrorModel('Backend returned: ' + error.status);
        }
    }
}