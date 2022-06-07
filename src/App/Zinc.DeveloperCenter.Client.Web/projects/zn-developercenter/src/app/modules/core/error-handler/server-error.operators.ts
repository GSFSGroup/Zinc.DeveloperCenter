import { HttpErrorResponse } from '@angular/common/http';
import { EMPTY, Observable, OperatorFunction, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

/**
 * Catches a server error and stops propagating emissions.
 *
 * @param status The server status to catch.
 * @param handle The work to do when the error is caught.
 * @returns A RxJS operator.
 */
export function catchServerError(status: number | number[], handle: (error: HttpErrorResponse) => void): OperatorFunction<any, any> {
    return catchError(err => {
        if (!isMatchingError(err, status)) {
            return throwError(err);
        }

        handle(err);

        return EMPTY;
    });
}

/**
 * Creates a side-effect when a certain server error is caught, and rethrows the server error.
 *
 * @param status The server status to catch.
 * @param handle The work to do when the error is caught.
 * @returns A RxJS operator.
 */
export function tapServerError(status: number | number[], handle: (error: HttpErrorResponse) => void): OperatorFunction<any, any> {
    return catchError(err => {
        if (isMatchingError(err, status)) {
            handle(err);
        }
        return throwError(err);
    });
}

/**
 * Catches a server error and transforms it according to the handle.
 *
 * @param status The server status to catch.
 * @param handle The work to do when the error is caught.
 * @returns A RxJS operator.
 */
export function mapServerError(status: number | number[], handle: (error: HttpErrorResponse) => Observable<any>): OperatorFunction<any, any> {
    return catchError(err => {
        if(isMatchingError(err, status)) {
            return handle(err);
        }
        return throwError(err);
    });
}

/**
 * Catches a server error and suppresses it.
 *
 * @param status The server status to catch.
 * @returns A RxJS operator.
 */
export function suppressServerError(status: number | number[]): OperatorFunction<any, any> {
    return catchServerError(status, () => { /* no op */ });
}

function isMatchingError(error: any, status: number | number[]): boolean {
    return error instanceof HttpErrorResponse && isMatchingStatus(error.status, status);
}

function isMatchingStatus(actualStatus: number, expectedStatus: number | number[]): boolean {
    return Array.isArray(expectedStatus) ? expectedStatus.some(s => s === actualStatus) : actualStatus === expectedStatus;
}
