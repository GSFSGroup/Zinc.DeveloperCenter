import { NO_ERRORS_SCHEMA } from '@angular/core';
import { TestBed, waitForAsync } from '@angular/core/testing';
import { SharedServices } from '@gsfsgroup/kr-shell-services';

import { MaterialModule } from '~/core/material-module/material.module';

import { ErrorHandlerService } from './error-handler.service';

describe('ErrorHandlerService', () => {
    let sharedServices: SharedServices;

    beforeEach(
        waitForAsync(() => {
            TestBed.configureTestingModule({
                declarations: [],
                imports: [MaterialModule],
                providers: [SharedServices],
                schemas: [NO_ERRORS_SCHEMA]
            }).compileComponents();
        })
    );

    beforeEach(() => {
        sharedServices = TestBed.inject(SharedServices);
    });

    it('should be created', () => {
        const service: ErrorHandlerService = TestBed.inject(ErrorHandlerService);

        expect(service).toBeTruthy();
    });

    it('should notifiy when an error occurred', () => {
        const error: Error = new Error('ERROR');
        const service: ErrorHandlerService = TestBed.inject(ErrorHandlerService);
        spyOn(sharedServices.notification, 'showError');
        spyOn(console, 'error');
        service.handleError(error);

        expect(sharedServices.notification.showError).toHaveBeenCalledWith('Uh oh, something went wrong. Please try again.');
        expect(console.error).toHaveBeenCalledWith(error);
    });
});
