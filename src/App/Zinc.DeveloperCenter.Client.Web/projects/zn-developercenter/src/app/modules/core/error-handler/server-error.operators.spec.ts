import { HttpErrorResponse } from '@angular/common/http';
import { EMPTY, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { catchServerError, tapServerError, mapServerError } from '~/core/error-handler/server-error.operators';

describe('RxJS Operators', () => {
    describe('catchServerError', () => {
        it('should ignore client errors', () => {
            throwError({ status: 500 }).pipe(
                catchServerError(500, () => fail('error should have been ignored')),
                catchError(() => EMPTY)
            ).subscribe();
        });

        it('should ignore errors of the wrong status', () => {
            throwError(new HttpErrorResponse({ status: 404 })).pipe(
                catchServerError(500, () => fail('error should have been ignored')),
                catchError(() => EMPTY)
            ).subscribe();
        });

        it('should call handle on matching error', done => {
            throwError(new HttpErrorResponse({ status: 500 })).pipe(
                catchServerError(500, () => done()),
                catchError(() => {
                    fail('error should have been handled');
                    return EMPTY;
                })
            ).subscribe();
        });

        it('should call handle on matching errors', done => {
            throwError(new HttpErrorResponse({ status: 500 })).pipe(
                catchServerError([404, 500], () => done()),
                catchError(() => {
                    fail('error should have been handled');
                    return EMPTY;
                })
            ).subscribe();
        });
    });

    describe('tapServerError', () => {
        it('should ignore client errors on tap', () => {
            throwError({ status: 500 }).pipe(
                tapServerError(500, () => fail('error should have been ignored')),
                catchError(() => EMPTY)
            ).subscribe();
        });

        it('should ignore errors of the wrong status on tap', () => {
            throwError(new HttpErrorResponse({ status: 404 })).pipe(
                tapServerError(500, () => fail('error should have been ignored')),
                catchError(() => EMPTY)
            ).subscribe();
        });

        it('should call handle on matching error on tap', done => {
            throwError(new HttpErrorResponse({ status: 500 })).pipe(
                tapServerError(500, () => done()),
                catchError(() => EMPTY)
            ).subscribe();
        });

        it('should propagate on matching error', done => {
            throwError(new HttpErrorResponse({ status: 500 })).pipe(
                tapServerError(500, () => { }),
                catchError(() => {
                    done();
                    return EMPTY;
                })
            ).subscribe();
        });
    });

    describe('mapServerError', () => {
        it('should ignore client errors on map', () => {
            throwError({ status: 500 }).pipe(
                mapServerError(500, () => {
                    fail('error should have been ignored');
                    return of();
                }),
                catchError(() => EMPTY)
            ).subscribe();
        });

        it('should ignore errors of the wrong status on map', () => {
            throwError(new HttpErrorResponse({ status: 404 })).pipe(
                mapServerError(500, () => {
                    fail('error should have been ignored');
                    return of();
                }),
                catchError(() => EMPTY)
            ).subscribe();
        });

        it('should map observable on matching error', () => {
            throwError(new HttpErrorResponse({ status: 500 })).pipe(
                mapServerError(500, () => of(3)),
                catchError(() => {
                    fail('should have recovered from the error');
                    return EMPTY;
                })
            ).subscribe(num => {
                expect(num).toBe(3);
            });
        });
    });
});
